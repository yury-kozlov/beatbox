using Beater.Sampled;

namespace Beater;

public static partial class ConsoleKeyPlayerExample
{
    public static void Test()
    {
        var player = new ConsoleKeyPlayer();
        player.Listen();
        player.PrintFormatted();
        player.SaveSequence();
        player.SaveCode();
        player.PlayRepeated().Wait();
        
        // OR
        player.PlayRepeated("/music/samples/sequences/seq2025-03-01-2114.json").Wait();

        // OR
        player.PlayRepeated(James_Shinra_Poppin.GetSequence()).Wait();

        // OR
        player.GenerateCodeFrom("/music/samples/sequences/seq2025-04-05-1719.json");
    }
}