using System.Text;
using System.Text.RegularExpressions;

namespace Beater;

public class TransportMessage
{
    public byte[] Message = [];
    public string? SoundName;

    public TransportMessage(string? soundName = null)
    {
        SoundName = soundName;
        if (soundName.HasValue())
        {
            Message = Encoding.ASCII.GetBytes(soundName.TrimEnd(';') + ';');
        }
    }

    internal static TransportMessage ToPlayMessage(string soundName)
    {
        return new TransportMessage($"play {soundName};");
    }
}

public class BatchItem
{
    public int PreDelay;
    public string? SoundName;

    internal static BatchItem? Parse(string message)
    {
        var match = Regex.Match(message, @"^(?<preDelay>\d+) (?<soundName>[^ ]+);$");
        if (match.Success)
        {
            return new BatchItem
            {
                PreDelay = int.Parse(match.Groups["preDelay"].Value),
                SoundName = match.Groups["soundName"].Value,
            };
        }
        return null;
    }

    public override string ToString() => $"{PreDelay} {SoundName}";
}


/// <summary>
/// Only real (non NoSound) items get added - callers are responsible for filtering those out beforehand
/// (see PlaybackSync.Build), so every item added here always ends up in the outbound message.
/// </summary>
public class TransportBatchMessage
{
    private List<BatchItem> _items = new();

    internal void Add(string soundName, int? preDelay)
    {
        _items.Add(new BatchItem
        {
            SoundName = soundName,
            PreDelay = preDelay ?? 0,
        });
    }

    internal TransportMessage ToTransportMessage()
    {
        var sb = new StringBuilder();
        sb.Append($"seq clear;"); // clear any previous sequences that were played before

        foreach (var item in _items)
        {
            // add keyword "seq" to allow receiver to recognize this message as a sequence
            sb.Append($"seq {item.PreDelay} {item.SoundName};");
        }

        sb.Append($"seq play;"); // play immediately after the last sound received

        var batchMessage = Encoding.ASCII.GetBytes(sb.ToString());
        return new TransportMessage { SoundName = "batch", Message = batchMessage };
    }
}

public static class Samples
{
    public static TransportMessage K = new(Kick.Name);
    public static TransportMessage S = new(Snare.Name);
    public static TransportMessage TS1 = new("ts1");
    public static TransportMessage TS2 = new("ts2");
    public static TransportMessage TS3 = new("ts3");
    public static TransportMessage TS4 = new("ts4");
    public static TransportMessage TS5 = new("ts5");
    public static TransportMessage TS6 = new("ts6");
    public static TransportMessage TS7 = new("ts7");
    public static TransportMessage TS8 = new("ts8");
    public static TransportMessage TS9 = new("ts9");
}
