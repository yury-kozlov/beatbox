namespace Beater;

/// <summary>
/// A system sound indicating an abrubtly trimmed loop.
/// </summary>
public record LoopEndTrimmed : LoopEnd
{
    public LoopEndTrimmed(LoopEnd loopEnd)
    {
        Sequence = loopEnd.Sequence;
        Strategy = loopEnd.Strategy;
        IsSequenceLoop = loopEnd.IsSequenceLoop;
        FriendlyName = $"{loopEnd.FriendlyName}-trimmed";
    }

    public override string? ToString() => Format(FriendlyName);
}