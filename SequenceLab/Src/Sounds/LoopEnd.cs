namespace Beater;

/// <summary>
/// Closes sequence with empty sound (acting as a spacer) so that any sound appended as a follower
// to this sequence later will continue only after current sequence ends.
/// </summary>
public record LoopEnd : NoSound
{
    public string FriendlyName => "end of loop";

    public override string? ToString() => $"{FriendlyName}: {Comment}";
}
