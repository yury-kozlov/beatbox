namespace Beater;

/// <summary>
/// A system sound indicating an abrubtly trimmed loop.
/// </summary>
public record LoopEndTrimmed : LoopEnd
{
    public LoopEndTrimmed(LoopEnd loopEnd)
    {
        Comment = $"{loopEnd.Timestamp} {loopEnd.Comment}";
        Sequence = loopEnd.Sequence;
        Strategy = loopEnd.Strategy;
        IsSequenceLoop = loopEnd.IsSequenceLoop;
        FriendlyName = $"{loopEnd.FriendlyName}-trimmed";
        FireAndForget = loopEnd.FireAndForget;
    }

    public override string? ToString() => Format(FriendlyName);
}