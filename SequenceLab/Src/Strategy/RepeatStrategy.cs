namespace Beater;

public class RepeatStrategy : AbstractStrategy
{
    public int Count;
    public int Interval;
    public int LinearIncrement;

    /// <summary>
    /// Will replace current sound with an empty sound preserving the same followers, for example:
    ///  3/4 - each 3rd time out of every 4 will be silenced.
    /// NOTE: this counter is related to every sound within the strategy (not to the strategy itself).
    /// </summary>
    public string? SilenceEveryXSoundOutOf;

    private int _previousIterval;

    protected override List<SequenceMessage> GenerateSequenceFor(Sound sound)
    {
        var originalSound = sound;
        var sequence = new List<SequenceMessage>();
        for (int i = 0; i < Count; i++)
        {
            sound = originalSound;
            if (!SilenceEveryXSoundOutOf.IsNullOrEmpty() && IsXOutOf(SilenceEveryXSoundOutOf, i + 1))
            {
                sound = sound with { IsSilenced = true };
            }

            var msg = new SequenceMessage(sound)
            {
                Timestamp = DelayAfterLeader + CalculateInterval(i),
                Comment = $"#{i + 1}",
            };
            sequence.Add(msg);

            if (sound.Followers.Count > 0)
            {
                sequence.AddRange(GenerateFollowersSequence(sound, msg.Timestamp));
            }
        }

        // close sequence with empty sound so that any other sequence that goes afterwards will continue only after this one ends
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

    private SequenceMessage GetEndingMessage(Sound sound)
    {
        var calledTimesText = CheckedTimes == CalledTimes ? $"call #{CheckedTimes}" : $"call #{CalledTimes}, check #{CheckedTimes}";
        return new SequenceMessage(null)
        {
            Timestamp = DelayAfterLeader + (Interval * Count),
            Comment = $"{sound.Name} repeat x{Count} ends, {calledTimesText}",
        };
    }
}
