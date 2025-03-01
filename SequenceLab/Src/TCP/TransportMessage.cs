using System.Text;

namespace Beater;

public class TransportMessage
{
    public byte[]? Message;
    public string? SoundName;

    public TransportMessage(string? soundName = null)
    {
        SoundName = soundName;
        if (!soundName.IsNullOrEmpty())
        {
            soundName = soundName.EndsWith(";") ? soundName : soundName + ";";
            Message = Encoding.ASCII.GetBytes(soundName);
        }
    }
}


public static class Samples
{
    public static TransportMessage B1 = new("b1");
    public static TransportMessage B2 = new("b2");
}
