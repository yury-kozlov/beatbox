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

    public async Task SendScheduled(List<SequenceMessage> sequenceMessages)
    {
        var startedAt = DateTime.Now;
        SequenceMessage? previous = null;
        foreach (var msg in sequenceMessages)
        {
            if (previous is not null)
            {
                var delay = msg.Timestamp - previous.Timestamp;
                if (delay > 0)
                {
                    await Task.Delay(delay);
                }
            }
            previous = msg;

            Logger.Log(msg, startedAt);
            if (msg.Sound is null || msg.Sound.IsSilenced || msg.Message is null)
            {
                continue;
            }
            Send(msg);
        }
    }
}
