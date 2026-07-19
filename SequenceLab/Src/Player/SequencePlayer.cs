using System.Text;

namespace Beater;

public static class SequencePlayer
{
    public static async Task Play(this TcpTransport? transport, GeneratedSequence sequence)
    {
        if (transport is not null)
        {
            await transport.SendScheduled(sequence);
        }
    }

    public static async Task PlayRepeated(this TcpTransport? transport, GeneratedSequence sequence)
    {
        if (transport is not null)
        {
            await Repeat(() => transport.SendScheduled(sequence), transport.Dispose);
        }
    }

    public static string ToString(this Sequence sequence)
    {
        Sound? previous = null;
        var str = new StringBuilder();
        foreach (var sound in sequence)
        {
            if (previous is not null)
            {
                var delay = sound.Timestamp - previous.Timestamp;
                if (delay > 0)
                {
                    var spacesCount = (int)(delay / 100.0);
                    for (var i = 0; i < spacesCount; i++)
                    {
                        str.Append(' ');
                    }
                }
            }
            previous = sound;
            if (!sound.IsSilenced)
            {
                str.Append(sound.Name);
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
