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
        FriendlyName = GetFriendlyName(repeatedSound);
    }

    public bool IsSequenceLoop { get; protected set; }

    public bool FireAndForget { get => Strategy.FireAndForget; set => Strategy.FireAndForget = value; }

    public override string? ToString() => $"{Format(FriendlyName)}: {Comment}";

    private string GetFriendlyName(Sound? repeatedSound)
    {
        if (IsSequenceLoop)
        {
            return repeatedSound?.Sequence is null ? "end-of-sequence-loop" : $"end-of-sequence-loop-{repeatedSound.Sequence.Name}";
        }
        return "end-of-loop";
    }
}
