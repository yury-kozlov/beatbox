namespace Beater;

/// <summary>
/// This strategy is mainly for convenience: it allows to specify all subsequent followers as a flat list
/// (instead of nesting each sound as a single follower of another follower reursively).
/// WARNING: Injecting a sound with this strategy in the middle of another sequence can break the timing of subsequent sounds.
/// The injected sound becomes the new "previous sound" for everything after it,
/// shifting all subsequent delays by the injected sound's timestamp offset.
/// </summary>
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
    public override GeneratedSequence ApplyStrategy(SoundDesign leader)
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

        leader.Generated.Timestamp = delay;

        if (leader is SequenceStart)
        {
            // sync timestamp of SequenceStart because current leader was cloned from it while timestamp of the original sound is not initialized
            // (we do this initialization only here because SequenceStart has FollowPreviousSoundStrategy)
            leader.Sequence.Leader.Generated.Timestamp = leader.Generated.Timestamp;
        }
        else
        {
            // at this point, timestamp is relative to the sequence-start (and will be shifted according to the sequence leader position later down the flow)
            leader.Sequence.AutoDuration = leader.Generated.Timestamp;
        }

        return [leader.Generated];
    }

    private GeneratedSound? GetPreviousSound(SoundDesign currentSound)
    {
        bool canBeFollowed(GeneratedSound previousSound)
        {
            if (ShouldFollowSameSequence && previousSound.SoundDesign.Sequence != currentSound.Sequence)
            {
                // a sound from another sequence was injected directly before the current sound
                return false;
            }
            // a sound with FireAndForget strategy can't be followed unless it's direct leader of the current sound
            return !previousSound.SoundDesign.Strategy.FireAndForget || previousSound.SoundDesign == currentSound.Leader;
        }
        return currentSound.PreviousSounds?.LastOrDefault(canBeFollowed);
    }

    internal AbstractStrategy ToFollowLeaderStrategy() => new FollowLeaderStrategy().CopyBasePropertiesFrom(this);
}