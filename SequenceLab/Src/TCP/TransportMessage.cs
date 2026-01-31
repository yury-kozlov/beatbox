using System.Text;

namespace Beater;

public class TransportMessage
{
    public byte[]? Message;
    public string? SoundName;

    public TransportMessage(string? soundName = null)
    {
        SoundName = soundName;
        if (soundName.HasValue())
        {
            Message = Encoding.ASCII.GetBytes(soundName.TrimEnd(';') + ';');
        }
    }

    internal TransportMessage ToPlayMessage()
    {
        return new TransportMessage($"play {SoundName};");
    }
}

public class TransportBatchMessage
{
    class BatchItem
    {
        public int PreDelay;
        public string? SoundName;
        public override string ToString() => $"{PreDelay} {SoundName}";
    }

    private List<BatchItem> _items = new();

    internal void Add(string? soundName, int? preDelay)
    {
        _items.Add(new BatchItem
        {
            SoundName = soundName ?? Sound.NoSound, // if no sound should be played - delay still must be applied
            PreDelay = preDelay ?? 0,
        });
    }

    internal TransportMessage ToTransportMessage()
    {
        var sb = new StringBuilder();
        sb.Append($"seq clear;"); // clear any previous sequences that were played before

        for (int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            if (item.SoundName == Sound.NoSound)
            {
                if (item.PreDelay == 0 || i == _items.Count - 1)
                {
                    continue; // skip empty sounds if they have no effect on others
                }
            }

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
    public static TransportMessage K = new(Sound.KickSound);
    public static TransportMessage S = new(Sound.SnareSound);
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
