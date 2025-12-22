using System.Net.Sockets;

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

    private void SendAllAtOnce(List<SequenceMessage> sequenceMessages)
    {
        var batch = new TransportBatchMessage();
        SequenceMessage? previous = null;
        foreach (var msg in sequenceMessages)
        {
            var preDelay = msg.Timestamp - previous?.Timestamp;
            batch.Add(msg.SoundName, preDelay);
            previous = msg;
        }
        Send(batch.ToTransportMessage());
    }

    private async Task SendDelayedMessages(List<SequenceMessage> sequenceMessages)
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
