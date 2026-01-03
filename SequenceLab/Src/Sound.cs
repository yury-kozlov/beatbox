namespace Beater;

public record Sound
{
    public const string NoSound = "no-sound";
    public const string KickSound = "k";
    public const string SnareSound = "s";

    public Sound(string? name)
    {
        Name = name;
    }

    public Sound(string? name, string simultaneousSound)
    {
        Name = name;

        // both sounds will be played at the same time (without any delay between them)
        Followers.Add(new Sound(simultaneousSound) { Strategy = new FollowPreviousSoundStrategy() });
    }

    public string? Name;
    public AbstractStrategy Strategy = new PlayOnceStrategy();
    public List<Sound> Followers = new();
    public Sound? Leader;
    public bool IsSilenced;

    /// <summary>
    /// Indicates that starting from the current sound all its followers belong to the same tag.
    /// Tags are used for searching a sound within a sequence.
    /// </summary>
    public List<string>? Tags;

    public int DelayAfterLeader { set { Strategy.DelayAfterLeader = value; } }

    override public string? ToString() => Name;

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
}

/// <summary>
/// Acts like a loop grid without any sound.
/// </summary>
public record Metronome : NoSound
{ }

public record NoSound : Sound
{
    public NoSound() : base(NoSound)
    { }
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
