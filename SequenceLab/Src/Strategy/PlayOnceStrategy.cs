namespace Beater;

public class PlayOnceStrategy : AbstractStrategy
{
    protected override Sequence GenerateSequenceFor(Sound leader, Sequence? previousSounds = null)
    {
        leader.Timestamp = DelayAfterLeader;

        var sequence = new Sequence() { leader };

        AddFollowers(leader, sequence);

        return sequence;
    }
}