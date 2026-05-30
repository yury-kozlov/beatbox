namespace Beater;

public class FollowPreviousSoundStrategy : AbstractStrategy
{
    /// <summary>
    /// Indicates that current sound should only follow previous sounds of the same sequence.
    /// If sounds from other sequences are injected directly before it, they will be ignored
    /// (delay before the current sound will be relative only to the previous sound of the same sequence).
    /// </summary>
    public bool ShouldFollowSameSequence { get; set; } = true;

    /// <summary>
    /// Generate sequence relatively to the previous sound.
    /// So that delays will be calculated based on position of the previous sound (rather than position of the leader).
    /// </summary>
    public override Sequence ApplyStrategy(Sound leader)
    {
        var delay = DelayAfterLeader;
        var previousSound = GetPreviousSound(leader);
        if (previousSound is not null)
        {
            // make delay relative to the previous sound (instead of being relative to the leader)
            // for example, if leader's timestamp is 1000 and the previous sound is 1200 and delay is 100, the final timestamp will be 1300 (instead of 1100)
            // note: previous message is considered the last one in the list (if previous messages were generated as part of nested loop with multiple followers, the last one will be taken without any sorting)
            delay += previousSound.Timestamp;
        }

        leader.Timestamp = delay;

        if (leader is SequenceStart)
        {
            // sync timestamp of SequenceStart because current leader was cloned from it while timestamp of the original sound is not initialized
            // (we do this initialization only here because SequenceStart has FollowPreviousSoundStrategy)
            leader.Sequence.Leader.Timestamp = leader.Timestamp;
        }
        else
        {
            // at this point, timestamp is relative to the sequence-start (and will be shifted according to the sequence leader position later down the flow)
            leader.Sequence.AutoDuration = leader.Timestamp;
        }

        return new Sequence() { leader };
    }

    private Sound? GetPreviousSound(Sound currentSound)
    {
        bool canBeFollowed(Sound previousSound)
        {
            if (ShouldFollowSameSequence && previousSound.Sequence != currentSound.Sequence)
    {
                return false;
            }
        // a sound with FireAndForget strategy can't be followed unless it's direct leader of the current sound
            return !previousSound.Strategy.FireAndForget || previousSound == currentSound.Leader;
        }
        return currentSound.PreviousSounds?.LastOrDefault(canBeFollowed);
    }

    internal AbstractStrategy ToPlayOnceStrategy() => new PlayOnceStrategy().CopyBasePropertiesFrom(this);
}