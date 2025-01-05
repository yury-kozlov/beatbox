namespace Beater;

public class Sound
{
    public Sound(string name)
    {
        Name = name;
    }

    public string Name;
    public AbstractStrategy Strategy;
    public List<Sound> Followers = new();
    public Sound? Leader;

    override public string ToString() => Name;

    internal void SetLeader()
    {
        foreach (var follower in Followers)
        {
            follower.Leader = this;
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

