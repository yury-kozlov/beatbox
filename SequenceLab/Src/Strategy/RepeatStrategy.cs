namespace Beater;

public class RepeatStrategy : AbstractStrategy
{
    public int Count;
    public int Interval;
    public int LinearIncrement;

    private int _previousIterval;

    protected override List<SequenceMessage> GenerateSequenceFor(Sound sound)
    {
        var sequence = new List<SequenceMessage>();
        for (int i = 0; i < Count; i++)
        {
            var msg = new SequenceMessage(sound.Name, $"#{i + 1}")
            {
                Timestamp = DelayAfterLeader + CalculateInterval(i),
            };
            sequence.Add(msg);

            if (sound.Followers.Count > 0)
            {
                sequence.AddRange(GenerateFollowersSequence(sound, msg.Timestamp));
            }
        }

        // close sequence with empty sound so that any other sequence that goes afterwards will continue only after this one ends
        sequence.Add(new SequenceMessage("", $"{sound.Name} repeat x{Count} ends") { Timestamp = DelayAfterLeader + (Interval * Count) });

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
}
