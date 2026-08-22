using System.Collections.Concurrent;
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

    /// <summary>
    /// Same network behavior as AllAtOnce (one message, delays attached), but paces how fast lines are
    /// printed to the console: the first window's worth of lines is printed before sending, then remaining
    /// lines are printed only as earlier ones get acked back, so a line never scrolls out of view before
    /// its "done playing" mark can be applied (which otherwise causes the console to auto-scroll/flicker).
    /// </summary>
    Batches,
}

public class TcpTransport
{
    private const int OutputPort = 3312;
    private const int InputPort = 3311;

    private static readonly UdpClient _client = new UdpClient(OutputPort);
    private readonly ConcurrentQueue<LogPosition> _pendingRows = new();

    /// <summary>
    /// Set only during a SendMode.Batches send; paces that send's printing against incoming acks.
    /// </summary>
    private PlaybackSync? _playbackSync;
    private bool _isDisposed;

    public SendMode SendMode { get; set; } = SendMode.Batches;

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
                if (transportMessage is not null && _pendingRows.TryDequeue(out var position))
                {
                    Logger.MarkLine(position.Row, position.Column, '✓', ConsoleColor.Green);
                    _playbackSync?.OnAck();
                }
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
                await SendDelayedMessages(sequence); // NOT IN USE
                break;
            case SendMode.AllAtOnce:
                SendAllAtOnce(sequence);
                break;
            case SendMode.Batches:
                await SendInPagedBatches(sequence);
                break;
            default:
                Console.WriteLine($"Unable to send generated messages. Unknown send mode: {SendMode}");
                break;
        }
    }

    /// <summary>
    /// Logs one entry, and if it's an item the remote player will actually echo back, queues its position
    /// so StartListener can mark it once acked.
    /// </summary>
    private void PrintEntry(PlaybackSync playbackSync, int index, DateTime startedAt)
    {
        var sound = playbackSync.Sequence[index];
        var position = Logger.Log(sound, startedAt, sound.Timestamp);
        if (playbackSync.IsPlayableSound(index))
        {
            // remote player echoes back sent items in the same order it plays them
            _pendingRows.Enqueue(position);
        }
    }

    private static int WindowHeight() => Math.Max(1, Console.WindowHeight - 5); // keep spare rows so pacing itself never needs a scroll

    private void SendAllAtOnce(GeneratedSequence sequence)
    {
        var playbackSync = new PlaybackSync(sequence);
        var startedAt = DateTime.Now;

        for (var i = 0; i < playbackSync.Sequence.Count; i++)
        {
            PrintEntry(playbackSync, i, startedAt);
        }

        Send(playbackSync.Batch.ToTransportMessage());
    }

    /// <summary>
    /// Prints the first window's worth of lines, sends the batch (same single-message wire format as
    /// AllAtOnce), then prints the rest paced by acks already received - so the number of unacked printed
    /// lines never exceeds what fits on screen, and marking a line never requires auto-scrolling.
    /// </summary>
    private async Task SendInPagedBatches(GeneratedSequence sequence)
    {
        _playbackSync = new PlaybackSync(sequence);
        var startedAt = DateTime.Now;

        var currentLine = 0;

        // print the first page before sending - the earliest acks can arrive almost immediately
        // after "seq play;", so those lines must already be on screen and tracked
        var firstPage = Math.Min(_playbackSync.Sequence.Count, WindowHeight());
        for (; currentLine < firstPage; currentLine++)
        {
            PrintEntry(_playbackSync, currentLine, startedAt);
        }

        Send(_playbackSync.Batch.ToTransportMessage());

        for (; currentLine < _playbackSync.Sequence.Count; currentLine++)
        {
            await _playbackSync.WaitForRoomAsync(currentLine);
            PrintEntry(_playbackSync, currentLine, startedAt);
        }

        _playbackSync = null;
    }

    /// <summary>
    /// A sequence flattened into the wire batch to send plus which of its entries actually get sent/echoed
    /// (system markers with no effect get filtered out - see TransportBatchMessage.GetSentItems). Also
    /// throttles how far ahead printing is allowed to run past the last acked line, so unacked lines never
    /// exceed one console window's height. StartListener calls OnAck() as replies arrive; the print loop
    /// calls WaitForRoomAsync before printing each entry.
    /// </summary>
    private sealed class PlaybackSync
    {
        private readonly HashSet<int> _playableSoundIndexes = [];

        /// <summary>
        /// Blocks prining until next ack of a played sound arrives.
        /// </summary>
        private readonly SemaphoreSlim _gate = new(0);

        /// <summary>
        /// Number of printed lines confirmed as finished playing so far (confirmation has been received from the sampler).
        /// </summary>
        private int _playedCount;

        public TransportBatchMessage Batch { get; } = new();
        public GeneratedSequence Sequence { get; }

        public PlaybackSync(GeneratedSequence sequence)
        {
            Sequence = sequence;
            GeneratedSound? previousReal = null;

            for (var index = 0; index < sequence.Count; index++)
            {
                var sound = sequence[index];

                if (sound.Name is null or NoSound.Name)
                {
                    // system/no-op marker - never sent over the wire
                    continue;
                }

                var preDelay = sound.Timestamp - previousReal?.Timestamp;
                if (preDelay < 0)
                {
                    Console.WriteLine("Predelay can't be negative: check that sequence has properly configured loop interval");
                }

                Batch.Add(sound.Name, preDelay);
                _playableSoundIndexes.Add(index);
                previousReal = sound;
            }
        }

        public bool IsPlayableSound(int i) => _playableSoundIndexes.Contains(i);

        /// <summary>
        /// Fired when confirmation of a played sound arrives, allowing the print loop to continue.
        /// </summary>
        public void OnAck() => _gate.Release();

        /// <summary>
        /// Wait until sounds sent to the sampler are actually played, so we can print to 
        /// console only after we receive confirmation that a sound get played instead of 
        /// just flushing everything to the output and potentially exceeding console's visible hight.
        /// </summary>
        public async Task WaitForRoomAsync(int currentLine)
        {
            if (currentLine - _playedCount >= WindowHeight())
            {
                // each played sound allows one more line to be printed
                // (so that console will be focused on currently playing sounds)
                await _gate.WaitAsync();
                _playedCount++;
            }
        }
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
