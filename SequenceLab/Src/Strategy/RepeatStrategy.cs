namespace Beater;

public class RepeatStrategy : AbstractStrategy
{
    public int Count;

    /// <summary>
    /// Interval in milliseconds between each repetition.
    /// </summary>
    public int Interval;

    public int LinearIncrement;

    /// <summary>
    /// Determines whether to omit repeated sounds if they exceed duration of parent loop.
    /// </summary>
    public bool TrimIfExceedsParentLoop = true;

    /// <summary>
    /// Will replace current sound with an empty sound preserving the same followers, for example:
    ///  3/4 - each 3rd time out of every 4 will be silenced.
    /// NOTE: this counter is related to every sound within the strategy (not to the strategy itself).
    /// </summary>
    public string? SilenceEveryXSoundOutOf;

    private int _previousIterval;

    public override Sequence ApplyStrategy(Sound leader)
    {
        var originalSound = leader;
        var repeatedSound = leader;
        var sequence = new Sequence();
        for (int i = 0; i < Count; i++)
        {
            repeatedSound = originalSound.DeepClone();
            if (Numbers.IsXOutOf(SilenceEveryXSoundOutOf, i + 1))
            {
                repeatedSound.IsSilenced = true;
            }

            repeatedSound.Timestamp = DelayAfterLeader + CalculateInterval(i);
            repeatedSound.Iteration = $"{i + 1}";
            repeatedSound.Comment = $"#{repeatedSound.Iteration}";

            if (repeatedSound is SequenceStart sequenceStart)
            {
                AdjustSequenceEndDelay(sequenceStart);
            }

            if (repeatedSound.Leader?.Strategy is RepeatStrategy leaderLoop
                && TrimIfExceedsParentLoop && repeatedSound.Timestamp > leaderLoop.Interval)
            {
                // sound is positioned outside parent's loop, ignore it
                Console.WriteLine($"Trimming sound '{repeatedSound.Name}' at {repeatedSound.Timestamp}ms as it exceeds parent loop interval of {leaderLoop.Interval}ms");
                continue;
            }

            sequence.Add(repeatedSound);
        }

        // close sequence with empty sound (acting as a spacer) so that any sound appended as a follower
        // to the current sequence later will continue only after current sequence ends
        sequence.Add(GetEndingMessage(repeatedSound));

        return sequence;
    }

    private void AdjustSequenceEndDelay(SequenceStart sequenceStart)
    {
        // adjust end of sequence delay to only include duration of the current iteration (instead of duration of the whole loop which includes all iterations)
        var sequenceEnd = sequenceStart.GetSequenceEnd();
        sequenceEnd.DelayAfterLeader = DelayAfterLeader + Interval; // NOTE: Interval is not multipled here by loop Count
        sequenceEnd.Comment = sequenceStart.Comment;
        /// NOTE: Iteration is not set here — it will be assigned during propagation in <see cref="SequenceGenerator.GenerateFollowersSequence"/>
    }

    private int CalculateInterval(int i)
    {
        if (LinearIncrement == 0)
        {
            return i * Interval;
        }

        if (i == 0)
        {
            // this is the first increment
            // reset interval because current instance of repeat strategy may be called several times (as part of another repeat strategy)
            return _previousIterval = 0;
        }

        return _previousIterval += Interval + (i - 1) * LinearIncrement;
    }

    private Sound GetEndingMessage(Sound repeatedSound)
    {
        var calledTimesText = CheckedTimes == CalledTimes ? $"call #{CheckedTimes}" : $"call #{CalledTimes}, check #{CheckedTimes}";

        var timestamp = DelayAfterLeader + (Interval * Count);

        var trimmedText = "";

        // check if current loop falls out of leader's loop (e.g. due to incorrect delay of one of the followers which increases total time):
        if (repeatedSound.Leader?.Strategy is RepeatStrategy leaderLoop && timestamp > leaderLoop.Interval)
        {
            // current loop ends outside parent's loop interval
            if (TrimIfExceedsParentLoop)
            {
                // end current loop together with leader's loop (to fit the loop interval)
                timestamp = leaderLoop.Interval;
                trimmedText = ", trimmed to fit parent loop";
            }
        }

        return new LoopEnd(repeatedSound)
        {
            Timestamp = timestamp,
            Sequence = repeatedSound.Sequence,
            Comment = $"{(repeatedSound is Metronome ? "metronome" : repeatedSound.Name)} repeat x{Count} ends, {calledTimesText}{trimmedText}",
            FireAndForget = repeatedSound.Strategy.FireAndForget,
        };
    }

    public override string ToString() => $"{base.ToString()}: x{Count} every {Interval}ms";
}