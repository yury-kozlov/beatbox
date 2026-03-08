namespace Beater;

/// <summary>
/// A system sound indicating an abrubtly trimmed sequence (for example, due to exceeding duration of a base sequence where current sequence was appended to).
/// </summary>
public record SequenceEndTrimmed : NoSound
{
    public SequenceEndTrimmed(SequenceEnd sequenceEnd)
    {
        Sequence = sequenceEnd.Sequence;
        FriendlyName = $"sequence-trimmed-{Sequence.Name}";
        Strategy = new FollowPreviousSoundStrategy();
    }

    public override string? ToString() => Format(FriendlyName);
}