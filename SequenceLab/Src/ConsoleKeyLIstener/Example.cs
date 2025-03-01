using Beater;

namespace KeyBeatListener;

public static partial class Example
{
    public static void Test()
    {
        var player = new ConsoleKeyPlayer();
        player.Listen();
        player.PrintFormatted();
        player.PlayRepeated().Wait();
    }
}