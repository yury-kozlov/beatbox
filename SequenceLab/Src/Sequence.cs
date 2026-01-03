using System.Text;

namespace Beater;

public class Sequence
{
    public Sound Leader = new NoSound();

    public List<SequenceMessage> Generate()
    {
        var sequence = Leader.Strategy.GenerateSequence(Leader);
        return sequence;
    }

    public static Sequence? FromJson(string json) => Serialization.FromJson<Sequence>(json);
}

public class SequenceMessage : TransportMessage
{
    public SequenceMessage(Sound? sound)
        : base(sound?.Name)
    {
        Sound = sound;
        Leads = sound.IsLeader();
        Name = (Sound?.Name).IsNullOrEmpty() ? Sound.NoSound : Sound.Name;
    }

    public Sound? Sound;

    public string Name;
    public string? Comment;
    public bool Leads;

    /// <summary>
    /// In the final sequence, represents absolute position of the sound from the beginning of the whole sequence.
    /// If the sound is an X iteration inside a loop, position will still be calculated from the very beginning (including all previous iterations).
    /// NOTE: during sequence generation, this value is calculated relatively to the current leader and then shifted according to leader's position (becomes absolute).
    /// </summary>
    public int Timestamp;

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
            await transport.SendScheduled(sequenceMessages);
        }
    }

    public static async Task PlayRepeated(this TcpTransport? transport, List<SequenceMessage> sequenceMessages)
    {
        if (transport is not null)
        {
            await Repeat(() => transport.SendScheduled(sequenceMessages), transport.Dispose);
        }
    }

    public static string ToString(this List<SequenceMessage> sequenceMessages)
    {
        SequenceMessage? previous = null;
        var str = new StringBuilder();
        foreach (var msg in sequenceMessages)
        {
            if (previous is not null)
            {
                var delay = msg.Timestamp - previous.Timestamp;
                if (delay > 0)
                {
                    var spacesCount = (int)(delay / 100.0);
                    for (var i = 0; i < spacesCount; i++)
                    {
                        str.Append(' ');
                    }
                }
            }
            previous = msg;
            if (msg.Sound is not null && !msg.Sound.IsSilenced && msg.Message is not null)
            {
                str.Append(msg.Name);
            }
        }
        return str.ToString();
    }

    private static async Task Repeat(Func<Task> repeatAction, Action onExit)
    {
        char keyChar;

        await repeatAction();
        Console.WriteLine("Repeat - press [y] or [r],    Exit - press [esc],    Continue - press [Enter]");

        for (; ; )
        {
            keyChar = Console.ReadKey().KeyChar;
            if (keyChar == (char)ConsoleKey.Escape)
            {
                onExit();
                Environment.Exit(0);
            }
            if (keyChar == 'y' || keyChar == 'r')
            {
                await repeatAction();
            }
            if (keyChar == (char)ConsoleKey.Enter)
            {
                break;
            }
        }
    }
}