namespace Beater;

public class Sound
{
    public Sound(string name)
    {
        Name = name;
    }

    public string Name;
    public Strategy Strategy;
    public List<Sound> Followers = new();

    override public string ToString() => Name;
}

