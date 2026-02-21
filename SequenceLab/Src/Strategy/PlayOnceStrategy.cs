namespace Beater;

public class PlayOnceStrategy : AbstractStrategy
{
    public override Sequence ApplyStrategy(Sound leader)
    {
        leader.Timestamp = DelayAfterLeader;

        return new Sequence() { leader };
    }
}