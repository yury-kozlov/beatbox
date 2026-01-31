namespace Beater;

public class PlayOnceStrategy : AbstractStrategy
{
    public override Sequence GenerateSequenceFor(Sound leader, Sequence? previousSounds = null)
    {
        leader.Timestamp = DelayAfterLeader;

        return new Sequence() { leader };
    }
}