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
    public Strategy Strategy;
    public List<Sound> Followers = new();

    [Obsolete]
    public byte[] Message;


    override public string ToString() => Name;
}

