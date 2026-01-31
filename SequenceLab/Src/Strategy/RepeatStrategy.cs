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

    public override Sequence GenerateSequenceFor(Sound leader, Sequence? previousSounds = null)
    {
        var originalSound = leader;
        var sequence = new Sequence();
        for (int i = 0; i < Count; i++)
        {
            leader = originalSound with { /* clone*/ };
            if (IsXOutOf(SilenceEveryXSoundOutOf, i + 1))
            {
                leader = leader with { IsSilenced = true };
            }

            leader.Timestamp = DelayAfterLeader + CalculateInterval(i);
            leader.Comment = $"#{i + 1}";

            if (leader.Leader?.Strategy is RepeatStrategy leaderLoop
                && TrimIfExceedsParentLoop && leader.Timestamp > leaderLoop.Interval)
            {
                // sound is positioned outside parent's loop, ignore it
                Console.WriteLine($"Trimming sound '{leader.Name}' at {leader.Timestamp}ms as it exceeds parent loop interval of {leaderLoop.Interval}ms");
                continue;
            }

            sequence.Add(leader);
        }

        // close sequence with empty sound (acting as a spacer) so that any sound appended as a follower
        // to the current sequence later will continue only after current sequence ends
        sequence.Add(GetEndingMessage(leader));

        return sequence;
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

    private Sound GetEndingMessage(Sound lastSound)
    {
        var calledTimesText = CheckedTimes == CalledTimes ? $"call #{CheckedTimes}" : $"call #{CalledTimes}, check #{CheckedTimes}";

        var timestamp = DelayAfterLeader + (Interval * Count);

        var trimmedText = "";

        // check if current loop falls out of leader's loop (e.g. due to incorrect delay of one of the followers which increases total time):
        if (lastSound.Leader?.Strategy is RepeatStrategy leaderLoop && timestamp > leaderLoop.Interval)
        {
            // current loop ends outside parent's loop interval
            if (TrimIfExceedsParentLoop)
            {
                // end current loop together with leader's loop (to fit the loop interval)
                timestamp = leaderLoop.Interval;
                trimmedText = ", trimmed to fit parent loop";
            }
        }

        return new LoopEnd()
        {
            Timestamp = timestamp,
            Comment = $"{(lastSound is Metronome ? "metronome" : lastSound.Name)} repeat x{Count} ends, {calledTimesText}{trimmedText}",
        };
    }
}