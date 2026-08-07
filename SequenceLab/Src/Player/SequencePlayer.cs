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
