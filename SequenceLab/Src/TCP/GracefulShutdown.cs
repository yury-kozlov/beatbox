using System.Runtime.InteropServices;

namespace Beater;

public static class GracefulShutdown
{
    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate? handler, bool add);
    private delegate bool ConsoleCtrlDelegate(int sig);

    // must be stored in a field to prevent GC from collecting it before the callback fires
    private static ConsoleCtrlDelegate? _consoleCtrlHandler;

    public static void Subscribe(Action onShutdown)
    {
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            onShutdown();
        };

        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            onShutdown();
        };

        _consoleCtrlHandler = sig =>
        {
            if (sig == 2) // CTRL_CLOSE_EVENT
            {
                onShutdown();
            }
            return false;
        };

        SetConsoleCtrlHandler(_consoleCtrlHandler, add: true);
    }
}
