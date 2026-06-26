namespace Beater;

public class Sequence : List<Sound>
{
    /// <summary>
    /// Number of sounds in the sequence after it was initialized (assigned as followers to a leader).
    /// </summary>
    public int InitialLength;

    public Sequence()
    { }

    public Sequence(IEnumerable<Sound> source) : base(source)
    { }

    public void SetLeader(Sound leader)
    {
        foreach (var sound in this)
        {
            sound.Leader = leader;
        }
    }

    public Sequence Mix(Sequence followers)
    {
        AddRange(followers);
        return SequenceSoundSorter.SortByTimestamp(this);
    }

    /// <summary>
    /// Detects sounds that were injected into the sequence after it was already initialized.
    /// </summary>
    internal void DetectInjectedSounds()
    {
        if (Count == InitialLength)
        {
            // no changes
            return;
        }

        // sequence has new sounds since it was initialized
        var initialSounds = this[..InitialLength];
        var newSounds = this[InitialLength..];

        var first = initialSounds.FirstOrDefault();
        if (first?.Strategy is FollowPreviousSoundStrategy)
        {
            // check if any sound is injected before the very first sound wth FollowPreviousSoundStrategy
            // (check only first one because usually all sounds in a sequence go with this strategy, not just specific one)
            var injectedBeforeFirst = newSounds.Where(s => first.Strategy.DelayAfterLeader > s.Strategy.DelayAfterLeader).ToList();
            first.InjectedSounds = injectedBeforeFirst.HasItems() ? injectedBeforeFirst : first.InjectedSounds;
        }

        foreach (var initialSound in initialSounds.Where(s => s.Strategy is not FollowPreviousSoundStrategy))
        {
            // check if any sound that is not following previous one has injected sounds before it
            // (ignore FollowPreviousSoundStrategy because we already checked it on the previous step)
            var injectedBeforeCurrent = newSounds.Where(s => initialSound.Strategy.DelayAfterLeader > s.Strategy.DelayAfterLeader).ToList();
            initialSound.InjectedSounds = injectedBeforeCurrent.HasItems() ? injectedBeforeCurrent : initialSound.InjectedSounds;
        }
    }
}
