using System.Text;

namespace Beater;

public class Sequence
{
    private int JoinsCounter;

    public Sound Leader { get; set => field = value.WithSequenceIfMissing(this); } = new NoSound();

    /// <summary>
    /// In milliseconds (represents full loop of a sequence including ending space).
    /// Duration of sequence should be known ahead for each predefined sequence if it's going to be played in loop
    /// (otherwise we will not be able to place next iteration at correct timing).
    /// </summary>
    public int Duration;

    /// <summary>
    /// Name of the current sequence.
    /// Used for logging purposes.
    /// </summary>
    public string? Name;

    /// <summary>
    /// Last sequence that was appended to the current one.
    /// </summary>
    public Sequence? LastAppendedSequence;

    public List<Sound> Generate()
    {
        var sequence = Leader.Strategy.GenerateSequence(Leader);
        return sequence;
    }

    public static Sequence? FromJson(string json) => Serialization.FromJson<Sequence>(json);

    /// <summary>
    /// Appends new sequence to the end of the current one.
    /// NOTE: duration of the current sequence is increased after adding the new one.
    /// </summary>
    internal Sequence Append(Sequence next)
    {
        if (Leader.Followers.Count == 0)
        {
            // this is the first sequence
            Leader.Followers = [next.Leader];
            Duration = next.Duration;
            LastAppendedSequence = next;
            return this;
        }

        JoinsCounter++;
        Leader.Followers.Add(new Joint()
        {
            JoinsCounter = JoinsCounter,
            DelayAfterLeader = Duration, // wait for the original sequence to finish, only then start playing the next one
            Followers = [next.Leader],
            PreviousSequence = LastAppendedSequence!,
            NextSequence = next,
        });
        Duration += next.Duration;
        LastAppendedSequence = next;
        return this;
    }

    public override string ToString() => Name ?? base.ToString() ?? "";
}

public static class SequencePlayer
{
    public static async Task Play(this TcpTransport? transport, List<Sound> sequenceMessages)
    {
        if (transport is not null)
        {
            await transport.SendScheduled(sequenceMessages);
        }
    }

    public static async Task PlayRepeated(this TcpTransport? transport, List<Sound> sequenceMessages)
    {
        if (transport is not null)
        {
            await Repeat(() => transport.SendScheduled(sequenceMessages), transport.Dispose);
        }
    }

    public static string ToString(this List<Sound> sequenceMessages)
    {
        Sound? previous = null;
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
            if (!msg.IsSilenced)
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