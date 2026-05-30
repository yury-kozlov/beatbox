namespace Beater;

/// <summary>
/// This is a system sound used as a spacing between sequences: to wait for the original sequence to finish, only then start playing the next sequence.
/// </summary>
public record SequenceEnd : NoSound
{
    private string _friendlyName;

    public SequenceEnd()
    {
        // a default ctor is supposed to be used only for JSON deserialization purposes
        _friendlyName = "sequence-end";
        InitStrategy();
    }

    public SequenceEnd(SequenceDesign sequence)
    {
        _friendlyName = $"sequence-end-{sequence.Name}";
        Sequence = sequence;
        InitStrategy();
    }

    public override string? FriendlyName
    {
        get
        {
            // for example: "sequence-end-slow-beat-1: 2000 ms"
            var durationStr = Sequence?.AutoDuration > 0 ? $": {Sequence.AutoDuration} ms" : "";
            return $"{_friendlyName}{durationStr}";
        }
    }

    public void InitStrategy()
    {
        if (Sequence?.Duration > 0)
        {
            // if sequence has explicit duration: we will use it as a signal to end:
            /// NOTE: in sequence loops, DelayAfterLeader will be adjusted to inlcude only current iteration in <see cref="RepeatStrategy.AdjustSequenceEndDelay"/>
            Strategy = new PlayOnceStrategy { DelayAfterLeader = Sequence.Duration };
            return;
        }
        // if duration of a sequence is unkown: current ending will be appended to the last sound
        Strategy = new FollowPreviousSoundStrategy() { ShouldFollowSameSequence = false /* end of sequence may not necessary be limited to a single sequence */ };
    }

    public override string? ToString() => Format(FriendlyName);
}