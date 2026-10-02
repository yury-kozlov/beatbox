namespace Beater;

public class FollowLeaderStrategy : AbstractStrategy
{
    /// <summary>
    /// In milliseconds.
    /// NOTE: right now it's hard coded, but in future it should be resolved dynamically based on injected sounds.
    /// </summary>
    public int MinBufferAfterInjectedSounds = 300;

    /// <summary>
    /// Minimum distance after injected sounds at which the buffer has to be increased.
    /// NOTE: right now it's hard coded, but in future it should be resolved dynamically based on injected sounds.
    /// </summary>
    public int MinBufferThreshold = 50;

    public override GeneratedSequence ApplyStrategy(SoundDesign leader)
    {
        leader.Generated.Timestamp = DelayAfterLeader;

        // at this point, timestamp is relative to the sequence-start (and will be shifted according to the sequence leader position later down the flow)
        leader.Sequence.AutoDuration = leader.Generated.Timestamp;

        if (leader.Injected.HasItems())
        {
            // increase delay of the current sound by total duration of injected followers + some extra buffer
            var exceedingDuration = leader.Injected.Last().Timestamp - leader.Generated.Timestamp;
            if (exceedingDuration > -MinBufferThreshold)
            {
                var delayedBy = exceedingDuration + MinBufferAfterInjectedSounds;
                leader.Generated.DelayedBy = delayedBy;
                leader.Generated.Comment += $"(delayed by {delayedBy})"; // delayed due to injected sounds
                leader.Generated.Timestamp += delayedBy;
                leader.Sequence.AutoDuration += delayedBy;
            }
        }

        return [leader.Generated];
    }
}