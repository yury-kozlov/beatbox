using Beater;

namespace KeyBeatListener;

public static partial class Example
{
    public static void Test()
    {
        var player = new ConsoleKeyPlayer();
        player.Listen();
        player.PrintFormatted();
        player.SaveSequence();
        player.PlayRepeated().Wait();

        // OR
        player.PlayRepeated("/music/samples/sequences/seq2025-03-01-2114.json").Wait();
    }
}