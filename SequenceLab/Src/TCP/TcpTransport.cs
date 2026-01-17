using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace Beater;

public enum SendMode
{
    /// <summary>
    /// Sends each message with delay, awaiting each message in dotnet.
    /// Delays are not consistent/precise because of unpredictable nature of timing operations with dotnet itself.
    /// </summary>
    DelayedMessages,

    /// <summary>
    /// Sends all messages at once with information about delays attached of each individual message - so that scheduling will happen outside of dotnet framework.
    /// </summary>
    AllAtOnce,
}

public class TcpTransport
{
    private readonly TcpClient _client;
    private readonly NetworkStream _channel;

    public SendMode SendMode { get; set; } = SendMode.AllAtOnce;

    public TcpTransport()
    {
        _client = new TcpClient("localhost", 3311);
        _channel = _client.GetStream();
    }

    public void Dispose()
    {
        Send(Encoding.UTF8.GetBytes("seq stop;")); // stop currently playing sequence
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

    public async Task SendScheduled(List<Sound> sequenceMessages)
    {
        switch (SendMode)
        {
            case SendMode.DelayedMessages:
                await SendDelayedMessages(sequenceMessages);
                break;
            case SendMode.AllAtOnce:
                SendAllAtOnce(sequenceMessages);
                break;
            default:
                Console.WriteLine($"Unable to send generated messages. Unknown send mode: {SendMode}");
                break;
        }
    }

    private void SendAllAtOnce(List<Sound> sequenceMessages)
    {
        var startedAt = DateTime.Now;
        var batch = new TransportBatchMessage();
        Sound? previous = null;
        foreach (var msg in sequenceMessages)
        {
            var preDelay = msg.Timestamp - previous?.Timestamp;
            if (preDelay < 0)
            {
                Console.WriteLine("Predelay can't be negative: check that sequence has properly configured loop interval");
            }

            batch.Add(msg.Name, preDelay);
            Logger.Log(msg, startedAt, msg.Timestamp);
            previous = msg;
        }
        Send(batch.ToTransportMessage());
    }

    private async Task SendDelayedMessages(List<Sound> sequenceMessages)
    {
        var startedAt = DateTime.Now;
        Sound? previous = null;
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
            if (msg.IsSilenced)
            {
                continue;
            }
            Send(new TransportMessage(msg.Name));
        }
    }
}
