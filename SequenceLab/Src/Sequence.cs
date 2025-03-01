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

public class SequenceMessage : TransportMessage
{
    public SequenceMessage(Sound? sound)
        : base(sound?.Name)
    {
        Sound = sound;
        Leads = sound.IsLeader();
        Name = Sound?.Name ?? "";
    }

    public Sound? Sound;

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
    public static async Task Play(this TcpTransport? transport, List<SequenceMessage> sequenceMessages)
    {
        if (transport is not null)
        {
            await PlaySequence(transport, sequenceMessages);
        }
    }

    public static async Task PlayRepeated(this TcpTransport? transport, List<SequenceMessage> sequenceMessages)
    {
        if (transport is not null)
        {
            await Repeat(() => PlaySequence(transport, sequenceMessages));
        }
    }

    private static async Task PlaySequence(TcpTransport transport, List<SequenceMessage> sequenceMessages)
    {
        var startedAt = DateTime.Now;
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

            Logger.Log(msg, startedAt);
            if (msg.Sound is null || msg.Sound.IsSilenced || msg.Message is null)
            {
                continue;
            }
            transport.Send(msg);
        }
    }

    private static async Task Repeat(Func<Task> callback)
    {
        char keyChar;
        do
        {
            await callback();
            Console.WriteLine("Repeat - press y,    Exit - press esc");
        }
        while ((keyChar = Console.ReadKey().KeyChar) == 'y');
        
        if (keyChar == (char)ConsoleKey.Escape)
        {
            Environment.Exit(0);
        }
    }
}