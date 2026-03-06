namespace Beater;

/// <summary>
/// This is a system sound used as a spacing between sequences: to wait for the original sequence to finish, only then start playing the next sequence.
/// </summary>
public record SequenceEnd : NoSound
{
    public SequenceEnd(SequenceDesign sequence)
    {
        FriendlyName = $"sequence-end-{sequence.Name}";

        if (sequence.Duration > 0)
        {
            // if sequence has explicit duration: we will use it as a signal to end:
            Strategy = new PlayOnceStrategy { DelayAfterLeader = sequence.Duration };
            return;
        }
        // if duration of a sequence is unkown: current ending will be appended to the last sound
        Strategy = new FollowPreviousSoundStrategy();
    }

    public override string? ToString() => Format(FriendlyName);
}