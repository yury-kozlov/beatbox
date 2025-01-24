using System.Net.Sockets;

namespace Beater;

/// <summary>
/// based on https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient
/// </summary>
internal class BeatBox : IDisposable
{
    private TcpClient? _client;
    private NetworkStream? _channel;

    public BeatBox Init()
    {
        _client = new TcpClient("localhost", 3000);
        _channel = _client.GetStream();

        GC.Collect();
        return this;
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    public async Task Run()
    {
        var seq0 = Minimal.TechnoBeat1().Generate();
        await _channel.Play(seq0);


        var seq1 = Minimal.TechnoBeat2().Generate();
        await _channel.PlayRepeated(seq1);

        // TODO 1:   implement different sequences with the same sounds and then try to connect them together as an evolution of one sequence into another
        // TODO 1.1: learn how to implement evolution of sequences using the same sounds by re-sampling existing tracks and layering them in sequence

        Console.WriteLine("");
    }
}
