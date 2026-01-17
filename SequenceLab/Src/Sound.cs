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
    public AbstractStrategy Strategy = new PlayOnceStrategy();
    public List<Sound> Followers = new();
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
    public Sequence? Sequence;

    public bool IsSilenced;

    /// <summary>
    /// Indicates that starting from the current sound all its followers belong to the same tag.
    /// Tags are used for searching a sound within a sequence.
    /// </summary>
    public List<string>? Tags;

    public int DelayAfterLeader { set { Strategy.DelayAfterLeader = value; } }

    public override string? ToString() => Name;

    internal void SetLeader()
    {
        foreach (var follower in Followers)
        {
            follower.Leader = this;
        }
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
    /// Sets sequence to the current sound if it's missing.
    /// </summary>
    public Sound WithSequenceIfMissing(Sequence sequence)
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
}

/// <summary>
/// Acts like a loop grid without any sound.
/// The same as NoSound, used for better developer experience.
/// </summary>
public record Metronome : NoSound
{ }

public record NoSound : Sound
{
    public NoSound() : base(NoSound)
    { }
}

/// <summary>
/// Joins two sequences.
/// The same as NoSound, used for better debugging experience.
/// </summary>
public record Joint : NoSound
{
    /// <summary>
    /// Indicates number of joined sequences.
    /// Initialized when a sequence is appended to another sequence.
    /// </summary>
    public required int JoinsCounter;

    /// <summary>
    /// The original sequence to which a new one will be appended.
    /// </summary>
    public required Sequence PreviousSequence;

    /// <summary>
    /// Next sequence appended to the original one.
    /// </summary>
    public required Sequence NextSequence;
}

/// <summary>
/// Main beat sound.
/// </summary>
public record Kick : Sound
{
    public Kick() : base(KickSound)
    { }
}

/// <summary>
/// Snare sound.
/// </summary>
public record Snare : Sound
{
    public Snare() : base(SnareSound)
    { }
}

public static class SoundExtensions
{
    public static bool IsLeader(this Sound? sound)
    {
        return sound is not null && sound.Leader is null;
    }
}
