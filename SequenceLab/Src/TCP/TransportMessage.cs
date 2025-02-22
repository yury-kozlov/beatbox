using System.Text;

namespace Beater;

public class TransportMessage
{
    public byte[] Message;

    public TransportMessage(string? message = null)
    {
        if (!message.IsNullOrEmpty())
        {
            message = message.EndsWith(";") ? message : message + ";";
            Message = Encoding.ASCII.GetBytes(message);
        }
    }
}


public static class Samples
{
    public static TransportMessage B1 = new("b1");
    public static TransportMessage B2 = new("b2");
}
