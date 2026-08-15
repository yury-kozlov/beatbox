using System.Net;
using System.Net.Sockets;
using System.Text;

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
    private const int OutputPort = 3312;
    private const int InputPort = 3311;

    private static readonly UdpClient _client = new UdpClient(OutputPort);
    private bool _isDisposed;

    public SendMode SendMode { get; set; } = SendMode.AllAtOnce;

    public TcpTransport()
    {
        Task.Run(StartListener);
    }

    /// <summary>
    /// Starts listening for incoming messages sent back from remote peer.
    /// </summary>
    private async Task StartListener()
    {
        try
        {
            while (true)
            {
                var result = await _client.ReceiveAsync();
                var message = Encoding.UTF8.GetString(result.Buffer);
                var transportMessage = BatchItem.Parse(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("TcpTransport error: " + ex);
        }
    }

    public void Dispose()
    {
        Send(Encoding.UTF8.GetBytes("seq stop;")); // stop currently playing sequence
        _client?.Dispose();
        _isDisposed = true;
    }

    public static void Close() => _client?.Dispose();

    public void Send(TransportMessage message)
    {
        Send(message.Message);
    }

    public void Send(byte[] message)
    {
        if (!_isDisposed)
        {
            _client.Send(message, message.Length, new IPEndPoint(IPAddress.Loopback, InputPort));
        }
    }

    public async Task SendScheduled(GeneratedSequence sequence)
    {
        switch (SendMode)
        {
            case SendMode.DelayedMessages:
                await SendDelayedMessages(sequence);
                break;
            case SendMode.AllAtOnce:
                SendAllAtOnce(sequence);
                break;
            default:
                Console.WriteLine($"Unable to send generated messages. Unknown send mode: {SendMode}");
                break;
        }
    }

    private void SendAllAtOnce(GeneratedSequence sequence)
    {
        var startedAt = DateTime.Now;
        var batch = new TransportBatchMessage();
        GeneratedSound? previous = null;
        foreach (var sound in sequence)
        {
            var preDelay = sound.Timestamp - previous?.Timestamp;
            if (preDelay < 0)
            {
                Console.WriteLine("Predelay can't be negative: check that sequence has properly configured loop interval");
            }

            batch.Add(sound.Name, preDelay);
            Logger.Log(sound, startedAt, sound.Timestamp);
            previous = sound;
        }
        Send(batch.ToTransportMessage());
    }

    private async Task SendDelayedMessages(GeneratedSequence sequence)
    {
        var startedAt = DateTime.Now;
        GeneratedSound? previous = null;
        foreach (GeneratedSound sound in sequence)
        {
            if (previous is not null)
            {
                var delay = sound.Timestamp - previous.Timestamp;
                if (delay > 0)
                {
                    await Task.Delay(delay);
                }
            }
            previous = sound;

            Logger.Log(sound, startedAt);
            if (sound.IsSilenced)
            {
                continue;
            }
            Send(new TransportMessage(sound.Name));
        }
    }
}
