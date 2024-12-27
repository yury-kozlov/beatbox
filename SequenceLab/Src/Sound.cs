using System.Text;

namespace Beater;

public class Sound
{
    public Sound(string name)
    {
        Name = name;
        Message = Encoding.ASCII.GetBytes($"{name} 1;");
    }

    public string Name;

    [Obsolete]
    public byte[] Message;

    public int PreDelay;
    public int PostDelay;
    public Sound? Next;

    public Strategy Strategy;
    public List<Sound> Followers = new();

    override public string ToString() => Name;
}

