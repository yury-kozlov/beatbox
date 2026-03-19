namespace Beater;

/// <summary>
/// Closes sequence with empty sound (acting as a spacer) so that any sound appended as a follower
// to this sequence later will continue only after current sequence ends.
/// </summary>
public record LoopEnd : NoSound
{
    public LoopEnd(Sound? repeatedSound = null)
    {
        IsSequenceLoop = repeatedSound is SequenceStart;
        FriendlyName = IsSequenceLoop ? "end-of-sequence-loop" : "end-of-loop";
    }

    public bool IsSequenceLoop { get; }

    public override string? ToString() => $"{Format(FriendlyName)}: {Comment}";
}
