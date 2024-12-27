namespace Beater;

public static partial class Program
{
    public static void Main(params string[] args)
    {
        try
        {
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
}