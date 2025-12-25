namespace Beater;

public static partial class Program
{
    public static void Main(params string[] args)
    {
        try
        {
            var player = new ConsoleKeyPlayer();
            Console.CancelKeyPress += OnShutdown(player);
            player.Listen();
            player.PrintFormatted();
            player.SaveSequence();
            player.SaveCode();
            player.PlayRepeated().Wait();

            using (var test = new BeatBox().Init())
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

    private static ConsoleCancelEventHandler OnShutdown(ConsoleKeyPlayer player) => (object? sender, ConsoleCancelEventArgs e) =>
    {
        player?.Dispose();
        Console.WriteLine("Shutting down..");
        Environment.Exit(0);
    };
}