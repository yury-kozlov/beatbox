using System.Text;

namespace Beater;

public class TransportMessage
{
    public byte[]? Message;
    public string? SoundName;

    public TransportMessage(string? soundName = null)
    {
        SoundName = soundName;
        if (!soundName.IsNullOrEmpty())
        {
            Message = Encoding.ASCII.GetBytes(soundName.TrimEnd(';') + ';');
        }
    }
}

public class TransportBatchMessage
{
    class BatchItem
    {
        public int PreDelay;
        public string? SoundName;
    }

    private List<BatchItem> _items = new();

    internal void Add(string? soundName, int? preDelay)
    {
        _items.Add(new BatchItem
        {
            SoundName = soundName ?? "null", // null indicates that no sound should be played, but delay still must be applied
            PreDelay = preDelay ?? 0,
        });
    }

    internal TransportMessage ToTransportMessage()
    {
        var sb = new StringBuilder();
        sb.Append($"seq clear;"); // clear any previous sequences that were received before

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
    public static TransportMessage B1 = new("b1");
    public static TransportMessage B2 = new("b2");
}
