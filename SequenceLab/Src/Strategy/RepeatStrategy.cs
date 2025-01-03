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
            var msg = new SequenceMessage(sound.Name)
            {
                Timestamp = DelayAfterLeader + CalculateInterval(i),
            };
            sequence.Add(msg);

            if (sound.Followers.Count > 0)
            {
                sequence.AddRange(GenerateFollowersSequence(sound, msg.Timestamp));
            }
        }

        _previousIterval = 0;

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
            return 0;
        }

        return _previousIterval += Interval + (i - 1) * LinearIncrement;
    }
}
