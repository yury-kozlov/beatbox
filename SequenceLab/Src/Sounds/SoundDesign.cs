using Newtonsoft.Json;

namespace Beater;

public record SoundDesign
{
    public string? Name;

    public Tags? Tags;

    /// <summary>
    /// Original sequence current sound belongs to.
    /// Used for logging purposes.
    /// NOTE: when appending one sequence to another, the original sequence still remains in place here.
    /// All followers will automatically get this name assigned when a leader sound is added to a sequence.
    /// NOTE: Sequence may be null when deserializing a sound from json.
    /// </summary>
    public SequenceDesign? Sequence;

    /// <summary>
    /// Unique Id of a sound.
    /// NOTE: within a loop a sound is cloned on each iteration, however the same ID is retained between iterations.
    /// Id is required to compare sounds for equality outside the scope of a loop.
    /// </summary>
    public Guid Id = Guid.NewGuid();

    [JsonIgnore]
    public GeneratedSound Generated { get; init; }

    public SoundDesign(string name)
    {
        Name = name.IsNullOrEmpty() ? NoSound.Name : name;
        Generated = new GeneratedSound(this);
    }

    public SoundDesign(string name, string simultaneousSound)
        : this(name)
    {
        // both sounds will be played at the same time (without any delay between them)
        Followers.Add(new SoundDesign(simultaneousSound) { Strategy = new FollowPreviousSoundStrategy() });
    }

    public virtual string? FriendlyName { get; set; }

    public AbstractStrategy Strategy
    {
        get; set
        {
            field = value;
            OnStrategyChange();
        }
    } = new FollowLeaderStrategy();

    /// <summary>
    /// NOTE: Followers of <see cref="SequenceDesign.Leader"/> must not be overridden because the leader in this case is
    /// a sequence-start and overriding its followers will break the actual first sound and sequence-end.
    /// </summary>
    public FollowersDesign Followers
    {
        get; set
        {
            // capture initial length of the followers in order to keep track of any changes for example when a new sound is injected
            (field = value).InitialLength = value.Count;
        }
    } = [];

    /// <summary>
    /// Sequence of injected sounds with already rendered timestamps.
    /// Those injected sounds were not originally part of design of the current sequence, but were injected later.
    /// </summary>
    public GeneratedSequence? Injected;

    public SoundDesign? Leader;

    /// <summary>
    /// A proxy of underlying strategy's PlayEveryX.
    /// </summary>
    public int? PlayEveryX { get => Strategy.PlayEveryX; set => Strategy.PlayEveryX = value ?? 0; }

    /// <summary>
    /// Assigns delay to the underlying strategy.
    /// </summary>
    public int? DelayAfterLeader
    {
        get => field ?? Strategy.DelayAfterLeader;
        set
        {
            var isChanged = field != value;
            field = value;
            if (isChanged)
            {
                InitStrategyDelay();
            }
        }
    }

    /// <summary>
    /// Previous sounds that were generated before the current sound (for the same leader).
    /// NOTE: those sounds have already rendered timestamps (as part of generated sequence).
    /// </summary>
    internal GeneratedSequence? PreviousSounds { get; set; }

    /// <summary>
    /// Example returned value: "k, 1200 k, 1200 k, 600 k, 600 k".
    /// </summary>
    public string DebuggerDisplay => SequenceDebuggerDisplay.Get(this);

    public override string? ToString() => Format(Name);

    protected string Format(string? friendlyName)
    {
        if (Tags.HasItems())
        {
            return $"{friendlyName}: {Generated.Timestamp:0000} {Tags}";
        }
        return $"{friendlyName}: {Generated.Timestamp:0000}";
    }

    /// <summary>
    /// Finds recursively and returns the first sound that has the specified tag.
    /// </summary>
    internal SoundDesign? FindByTag(string tag)
    {
        if (Tags.ContainsSafe(tag))
        {
            return this;
        }

        foreach (var follower in Followers)
        {
            var sound = follower.FindByTag(tag);
            if (sound is not null)
            {
                return sound;
            }
        }

        return null;
    }

    /// <summary>
    /// Make sure that current sound and all its followers have initialized sequence if it's missing.
    /// </summary>
    public SoundDesign WithSequenceIfMissing(SequenceDesign sequence)
    {
        Sequence ??= sequence;

        foreach (var follower in Followers)
        {
            follower.WithSequenceIfMissing(Sequence);
        }

        return this;
    }

    public SoundDesign WithFollower(SoundDesign follower)
    {
        Followers.Add(follower);
        return this;
    }

    public SoundDesign MoveFollowerToTheEnd(SoundDesign follower)
    {
        Followers.Remove(follower);
        return WithFollower(follower);
    }

    /// <summary>
    /// Clones current sound.
    /// NOTE: The name can't be Clone because it will conflict with Record's built-in method.
    /// NOTE: One of the reasons to use deep clone is because when we use RepeatStrategy for a sequence
    ///   SequenceStart should be copied together with corresponding SequenceEnd sound (they always go together)
    ///   and delay of SequenceEnd should be adjusted on each iteration of SequenceStart.
    ///   Otherwise SequenceEnd will have incorrect timing.
    /// NOTE: Generated is always re-created so each iteration gets its own independent generation-time state.
    /// </summary>
    public SoundDesign DeepClone()
    {
        var clone = this with
        {
            Followers = [.. Followers.Select(f => f.DeepClone())],
            Generated = new GeneratedSound(this),
        };
        clone.Followers.InitialLength = Followers.InitialLength;
        clone.Generated.SoundDesign = clone;

        return clone;
    }

    internal void InitStrategyDelay()
    {
        if (DelayAfterLeader.HasValue)
        {
            Strategy.DelayAfterLeader = DelayAfterLeader.Value;
        }
    }

    internal void OnStrategyChange()
    {
        if (Strategy.DelayAfterLeader == 0)
        {
            // change delay of the new strategy only if strategy delay is empty
            InitStrategyDelay();
        }
    }
}

public static class SoundExtensions
{
    public static bool IsLeader(this SoundDesign? sound)
    {
        return sound is not null && sound.Leader is null;
    }

    public static SoundDesign Chain(params SoundDesign[] sounds)
    {
        for (int i = sounds.Length - 1; i > 0; i--)
        {
            sounds[i - 1].WithFollower(sounds[i]);
            sounds[i - 1].Followers.InitialLength++;
        }
        return sounds.First();
    }
}
