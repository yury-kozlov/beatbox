namespace Beater;

/// <summary>
/// A map of injected sounds with their sequences mapped to coresponding yielding sounds.
/// This information is built at different stages of sequence generation so we need to collect it on the fly and store separately.
/// </summary>
public class InjectionMap
{
    private class InjectionInfo
    {
        public required Sound InjectedSound;
        public required Sound YieldingSound;
        public Sequence? InjectedSequence;
    }

    private readonly List<InjectionInfo> _injections = [];

    /// <summary>
    /// Gets ordered followers if any injections were detected, or null otherwise.
    /// </summary>
    public readonly Sequence? Ordered;

    /// <summary>
    /// Detects sounds that were injected into the sequence after it was already initialized.
    /// </summary>
    internal InjectionMap(Sequence source)
    {
        if (source.Count == source.InitialLength)
        {
            // no injections
            return;
        }

        // sequence has new sounds since it was initialized
        var yielding = source[..source.InitialLength];
        var injected = source[source.InitialLength..];

        for (var i = 0; i < yielding.Count; i++)
        {
            var yieldingSound = yielding[i];
            if (i == 0 || yieldingSound.Strategy is not FollowPreviousSoundStrategy)
            {
                // check if any sound that is not following previous one has injected sounds before it
                // NOTE: in case of FollowPreviousSoundStrategy only check the first sound because
                // usually this strategy is assigned to all sequence and if we check the first it will be enough for the rest

                var injectedBeforeCurrent = injected.Where(s => yieldingSound.Strategy.DelayAfterLeader > s.Strategy.DelayAfterLeader).ToList();
                foreach (var injectedSound in injectedBeforeCurrent)
                {
                    _injections.Add(new InjectionInfo() { YieldingSound = yieldingSound, InjectedSound = injectedSound });
                }
            }
        }

        if (_injections.HasItems())
        {
            Ordered = OrderByInjections(source);
        }
    }

    /// <summary>
    /// Mmove injected followers before other sounds if their delay is less (because during injection they are placed at the end of the followers list).
    /// This is necessary because other sounds must know duration of any sequences injected before them so they can decide whether to change their behavior.
    private Sequence OrderByInjections(Sequence source)
    {
        var ordered = new Sequence(source);
        foreach (var injection in _injections)
        {
            // injected sound should go right before the yielding sound
            var iYielding = ordered.IndexOf(injection.YieldingSound);
            var iInjected = ordered.IndexOf(injection.InjectedSound);
            if (iYielding < iInjected)
            {
                ordered.MoveBefore(iYielding, iInjected);
            }
        }
        return ordered;
    }

    /// <summary>
    /// Stores generated sequence of injected sound per yielding sound that needs to know about any injections before it.
    /// NOTE: It's important that sequence of injected sounds is generated before the sound with injection
    ///       because sounds that go after injection need to know duration of injected sequences.
    /// </summary>
    public void SetInjectionSequence(Sound follower, Sequence injectedSequence)
    {
        foreach (var injection in _injections)
        {
            if (injection.InjectedSound?.Id == follower.Id)
            {
                injection.InjectedSequence = injectedSequence;
            }
        }
    }

    /// <summary>
    /// Returns generated sequence of injected sounds (with timestamps) for a yielding sound, or null if nothing is injected.
    /// If a follower has a sequence injected before it, it needs to know about such injection
    /// to recalculate position based on duration of the injected sequence.
    /// </summary>
    public Sequence? GetInjectedSequence(Sound yielding)
    {
        var injection = _injections.Find(i => i.InjectedSequence is not null && i.YieldingSound?.Id == yielding.Id);
        return injection?.InjectedSequence;
    }
}
