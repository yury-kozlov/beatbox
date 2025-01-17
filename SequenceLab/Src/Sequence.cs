using System.Net.Sockets;
using System.Text;

namespace Beater;

public class Sequence
{
    public Sound Leader;

    public List<SequenceMessage> Generate()
    {
        var sequence = Leader.Strategy.GenerateSequence(Leader);
        return sequence;
    }
}

public class SequenceMessage
{
    public SequenceMessage(Sound? sound)
    {
        Sound = sound;
        Leads = sound.IsLeader();
        Message = Encoding.ASCII.GetBytes($"{Sound?.Name} 1;"); // note: second argument is not yet supported
        Name = Sound?.Name ?? "";
    }

    public Sound? Sound;

    public byte[] Message;
    public string Name;
    public string? Comment;
    public bool Leads;
    public int Timestamp; // from the beginning of sequence

    override public string ToString()
    {
        return $"{Timestamp:0000} {Name}";
    }
}

public static class SequencePlayer
{
    public static async Task Play(this NetworkStream? channel, List<SequenceMessage> sequenceMessages)
    {
        if (channel is null)
        {
            return;
        }

        SequenceMessage? previous = null;
        foreach (var msg in sequenceMessages)
        {
            if (previous is not null)
            {
                var delay = msg.Timestamp - previous.Timestamp;
                if (delay > 0)
                {
                    await Task.Delay(delay);
                }
            }
            previous = msg;

            Logger.Log(msg);
            channel.Write(msg.Message, 0, msg.Message.Length);
        }
    }

}