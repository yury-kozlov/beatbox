namespace Beater;

public class PlayOnceStrategy : AbstractStrategy
{
    public Sequence ApplyStrategy(Sound leader)
    {
        leader.Timestamp = DelayAfterLeader;

        return new Sequence() { leader };
    }
}