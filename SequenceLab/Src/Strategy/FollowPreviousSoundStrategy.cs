namespace Beater;

public class FollowPreviousSoundStrategy : AbstractStrategy
{
    /// <summary>
    /// Generate sequence relatively to the previous sound.
    /// So that delays will be calculated based on position of the previous sound (rather than position of the leader).
    /// </summary>
    protected override Sequence GenerateSequenceFor(Sound leader, Sequence? previousSounds = null)
    {
        var delay = DelayAfterLeader;
        if (previousSounds?.Count > 0)
        {
            // make delay relative to the previous sound (instead of being relative to the leader)
            // for example, if leader's timestamp is 1000 and the previous sound is 1200 and delay is 100, the final timestamp will be 1300 (instead of 1100)
            // note: previous message is considered the last one in the list (if previous messages were generated as part of nested loop with multiple followers, the last one will be taken without any sorting)
            delay += previousSounds.Last().Timestamp;
        }

        leader.Timestamp = delay;

        var sequence = new Sequence() { leader };

        AddFollowers(leader, sequence);

        return sequence;
    }
}