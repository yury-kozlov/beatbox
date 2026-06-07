namespace Beater;

public class FollowLeaderStrategy : AbstractStrategy
{
    public override Sequence ApplyStrategy(Sound leader)
    {
        leader.Timestamp = DelayAfterLeader;

        // at this point, timestamp is relative to the sequence-start (and will be shifted according to the sequence leader position later down the flow)
        leader.Sequence.AutoDuration = leader.Timestamp;

        return new Sequence() { leader };
    }
}