namespace Beater;

public class FollowPreviousSoundStrategy : AbstractStrategy
{
    /// <summary>
    /// Generate sequence relatively to the previous sound.
    /// So that delays will be calculated based on position of the previous sound (rather than position of the leader).
    /// </summary>
    public Sequence ApplyStrategy(Sound leader)
    {
        var delay = DelayAfterLeader;
        var previousSound = leader.PreviousSounds?.LastOrDefault();
        if (previousSound is not null)
        {
            // make delay relative to the previous sound (instead of being relative to the leader)
            // for example, if leader's timestamp is 1000 and the previous sound is 1200 and delay is 100, the final timestamp will be 1300 (instead of 1100)
            // note: previous message is considered the last one in the list (if previous messages were generated as part of nested loop with multiple followers, the last one will be taken without any sorting)
            delay += previousSound.Timestamp;
        }

        leader.Timestamp = delay;

        return new Sequence() { leader };
    }
}