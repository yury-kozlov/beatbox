namespace Beater;

public class PlayOnceStrategy : AbstractStrategy
{
    protected override List<SequenceMessage> GenerateSequenceFor(Sound sound, List<SequenceMessage>? previousMessages = null)
    {
        var msg = new SequenceMessage(sound)
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