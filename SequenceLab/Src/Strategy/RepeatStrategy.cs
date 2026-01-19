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
    /// Indicates whether duration of inner loop may exceed duration of the parent loop (making leader's loop longer than it's defined).
    /// By default false = don't expand inner loops (make them fit parent loop).
    /// </summary>
    public bool ExpandLeaderLoop;

    /// <summary>
    /// Will replace current sound with an empty sound preserving the same followers, for example:
    ///  3/4 - each 3rd time out of every 4 will be silenced.
    /// NOTE: this counter is related to every sound within the strategy (not to the strategy itself).
    /// </summary>
    public string? SilenceEveryXSoundOutOf;

    private int _previousIterval;

    protected override List<Sound> GenerateSequenceFor(Sound sound, List<Sound>? previousMessages = null)
    {
        var originalSound = sound;
        var sequence = new List<Sound>();
        for (int i = 0; i < Count; i++)
        {
            sound = originalSound with { /* clone*/ };
            if (!SilenceEveryXSoundOutOf.IsNullOrEmpty() && IsXOutOf(SilenceEveryXSoundOutOf, i + 1))
            {
                sound = sound with { IsSilenced = true };
            }

            sound.Timestamp = DelayAfterLeader + CalculateInterval(i);
            sound.Comment = $"#{i + 1}";
            sequence.Add(sound);

            AddFollowers(sound, sequence);
        }

        // close sequence with empty sound (acting as a spacer) so that any sound appended as a follower
        // to the current sequence later will continue only after current sequence ends
        sequence.Add(GetEndingMessage(sound));

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

    private Sound GetEndingMessage(Sound sound)
    {
        var calledTimesText = CheckedTimes == CalledTimes ? $"call #{CheckedTimes}" : $"call #{CalledTimes}, check #{CheckedTimes}";

        var timestamp = DelayAfterLeader + (Interval * Count);

        // check if current loop falls out of leader's loop (e.g. due to incorrect delay of one of the followers which increases total time):
        if (!ExpandLeaderLoop && sound.Leader?.Strategy is RepeatStrategy leaderLoop && timestamp > leaderLoop.Interval)
        {
            // current loop is longer than the leader's loop, make it equal:
            timestamp = leaderLoop.Interval;
        }

        return new LoopEnd()
        {
            Timestamp = timestamp,
            Comment = $"{(sound is Metronome ? "metronome" : sound.Name)} repeat x{Count} ends, {calledTimesText}",
        };
    }
}
