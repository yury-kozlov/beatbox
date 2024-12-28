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
        var sequence = Minimal.TechnoBeat1();

        var sequenceMessages = sequence.Generate();
        await _channel.Play(sequenceMessages);
    }
}
