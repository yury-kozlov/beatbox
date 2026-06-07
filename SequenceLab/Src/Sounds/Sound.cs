
namespace Beater;

public record Sound
{
    public Sound(string name)
    {
        Name = name.IsNullOrEmpty() ? NoSound.Name : name;
    }

    public Sound(string name, string simultaneousSound)
        : this(name)
    {
        // both sounds will be played at the same time (without any delay between them)
        Followers.Add(new Sound(simultaneousSound) { Strategy = new FollowPreviousSoundStrategy() });
    }

    public string? Name;
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
    public Sequence Followers = new(); /// TODO: should this be <see cref="SequenceDesign"/> instead? Should each Sound here be a SoundDesign instead?

    public Sound? Leader;

    /// <summary>
    /// In the final sequence, represents absolute position of the sound from the beginning of the whole sequence.
    /// If the sound is an X iteration inside a loop, position will still be calculated from the very beginning (including all previous iterations).
    /// NOTE: during sequence generation, this value is calculated relatively to the current leader and then shifted according to leader's position (becomes absolute).
    /// </summary>
    public int Timestamp;
    public string? Comment;

    /// <summary>
    /// Hierarchical iteration path (starting from 1) of the current sound in nested loops, e.g. "1", "2.3", "1.2.1", etc.
    /// Specified in the following format: "{OuterLoopIteration}.{...}.{InnerLoopIteration}".
    /// NOTE: if current sound is not part of any loop, Iteration will be null.
    /// This field is used for sorting sequence sounds.
    /// </summary>
    public string? Iteration;

    /// <summary>
    /// Original sequence current sound belongs to.
    /// Used for logging purposes.
    /// NOTE: when appending one sequence to another, the original sequence still remains in place here.
    /// All followers will automatically get this name assigned when a leader sound is added to a sequence.
    /// NOTE: Sequence may be null when deserializing a sound from json.
    /// </summary>
    public SequenceDesign Sequence;

    public bool IsSilenced;

    /// <summary>
    /// Indicates that starting from the current sound all its followers belong to the same tag.
    /// Tags are used for searching a sound within a sequence.
    /// </summary>
    public List<string>? Tags;

    /// <summary>
    /// Assigns delay to the underlying strategy.
    /// </summary>
    public int? DelayAfterLeader
    {
        get; set
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
    /// </summary>
    internal Sequence? PreviousSounds { get; set; }

    public override string? ToString() => Format(Name);

    protected string Format(string? friendlyName)
    {
        return $"{friendlyName}: {Timestamp:0000}";
    }

    /// <summary>
    /// Finds recursively and returns the first sound that has the specified tag.
    /// </summary>
    internal Sound? FindByTag(string tag)
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
    public Sound WithSequenceIfMissing(SequenceDesign sequence)
    {
        Sequence ??= sequence;

        foreach (var follower in Followers)
        {
            follower.WithSequenceIfMissing(Sequence);
        }

        return this;
    }

    public Sound WithFollower(Sound follower)
    {
        Followers.Add(follower);
        return this;
    }

    public Sound MoveFollowerToTheEnd(Sound follower)
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
    /// </summary>
    public Sound DeepClone()
    {
        var clone = this with { Followers = new Sequence() };
        foreach (var follower in Followers)
        {
            clone.Followers.Add(follower.DeepClone());
        }
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
    public static bool IsLeader(this Sound? sound)
    {
        return sound is not null && sound.Leader is null;
    }
}
