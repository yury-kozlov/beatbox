namespace Beater;

public record SequenceStart : NoSound
{
    public SequenceStart(string sequenceName)
    {
        FriendlyName = $"sequence-start-{sequenceName}";
    }

    public override string? ToString() => FriendlyName;
}