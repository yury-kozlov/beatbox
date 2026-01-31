namespace Beater;

public static partial class Program
{
    public static void Main(params string[] args)
    {
        try
        {
            var player = new ConsoleKeyPlayer();

            player.Listen();
            player.PrintFormatted();
            player.SaveSequence();
            player.SaveCode();
            player.PlayRepeated().Wait();

            using (var test = new SequencePlayerExample().Init())
            {
                test.Run().Wait();
            }
            Console.WriteLine("done");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}