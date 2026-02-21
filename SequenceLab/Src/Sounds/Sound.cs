
namespace Beater;

public record Sound
{
    public const string NoSound = "no-sound";
    public const string KickSound = "k";
    public const string SnareSound = "s";

    public Sound(string name)
    {
        Name = name.IsNullOrEmpty() ? NoSound : name;
    }

    public Sound(string name, string simultaneousSound)
        : this(name)
    {
        // both sounds will be played at the same time (without any delay between them)
        Followers.Add(new Sound(simultaneousSound) { Strategy = new FollowPreviousSoundStrategy() });
    }

    public string? Name;
    public string? FriendlyName;

    public AbstractStrategy Strategy = new PlayOnceStrategy();
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
    /// Original sequence current sound belongs to.
    /// Used for logging purposes.
    /// NOTE: when appending one sequence to another, the original sequence still remains in place here.
    /// All followers will automatically get this name assigned when a leader sound is added to a sequence.
    /// </summary>
    public SequenceDesign? Sequence;

    public bool IsSilenced;

    /// <summary>
    /// Indicates that starting from the current sound all its followers belong to the same tag.
    /// Tags are used for searching a sound within a sequence.
    /// </summary>
    public List<string>? Tags;

    /// <summary>
    /// Assigns delay to the underlying strategy.
    /// NOTE: if sound strategy property is initialized inside the same block after delay is set, the current delay value will be ignored.
    /// </summary>
    public int DelayAfterLeader { set { Strategy.DelayAfterLeader = value; } }

    public override string? ToString() => Name;

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
        if (Sequence is null)
        {
            Sequence = sequence;
        }

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
}

public static class SoundExtensions
{
    public static bool IsLeader(this Sound? sound)
    {
        return sound is not null && sound.Leader is null;
    }
}
