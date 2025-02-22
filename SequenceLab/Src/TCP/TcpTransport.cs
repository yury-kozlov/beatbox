using System.Net.Sockets;

namespace Beater;

public class TcpTransport
{
    private readonly TcpClient _client;
    private readonly NetworkStream _channel;

    public TcpTransport()
    {
        _client = new TcpClient("localhost", 3000);
        _channel = _client.GetStream();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    public void Send(TransportMessage message)
    {
        Send(message.Message);
    }

    public void Send(byte[] message)
    {
        _channel.Write(message, 0, message.Length);
    }
}
