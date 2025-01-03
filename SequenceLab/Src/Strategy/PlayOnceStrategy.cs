namespace Beater;

public class PlayOnceStrategy : AbstractStrategy
{
    protected override List<SequenceMessage> GenerateSequenceFor(Sound sound)
    {
        var msg = new SequenceMessage(sound.Name)
        {
            Timestamp = DelayAfterLeader,
        };

        var sequence = new List<SequenceMessage>() { msg };
        if (sound.Followers.Count > 0)
        {
            sequence.AddRange(GenerateFollowersSequence(sound, msg.Timestamp));
        }

        return sequence;
    }
}