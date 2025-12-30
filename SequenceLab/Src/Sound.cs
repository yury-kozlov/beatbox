namespace Beater;

public record Sound
{
    public const string NoSound = "no-sound";

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
    public AbstractStrategy Strategy;
    public List<Sound> Followers = new();
    public Sound? Leader;
    public bool IsSilenced;

    override public string? ToString() => Name;

    internal void SetLeader()
    {
        foreach (var follower in Followers)
        {
            follower.Leader = this;
        }
    }
}

/// <summary>
/// Acts like a loop grid without any sound.
/// </summary>
public record Metronome : Sound
{
    public Metronome() : base(NoSound)
    {
    }
}

public static class SoundExtensions
{
    public static bool IsLeader(this Sound? sound)
    {
        return sound is not null && sound.Leader is null;
    }
}

