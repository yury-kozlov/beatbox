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

    override public string ToString() => Name;
}

