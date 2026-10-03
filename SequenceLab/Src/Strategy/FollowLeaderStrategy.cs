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

    public override GeneratedSequence ApplyStrategy(SoundDesign currentSound)
    {
        currentSound.Generated.Timestamp = DelayAfterLeader;

        // at this point, timestamp is relative to the leader (and will be shifted according to the leader's absolute position later down the flow)
        currentSound.Sequence!.AutoDuration = currentSound.Generated.Timestamp;

        if (currentSound.Injected.HasItems())
        {
            var lastInjectedSound = currentSound.Injected.Last();
            var buffer = currentSound.Generated.Timestamp - lastInjectedSound.Timestamp;
            var isInjectedFireAndForget = currentSound.Injected.All(x => x.SoundDesign.Strategy.FireAndForget);
            if (buffer < MinBufferThreshold && !isInjectedFireAndForget)
            {
                // increase delay of the current sound by giving some extra buffer due to injected sounds that came too close before it
                var delayedBy = buffer + MinBufferAfterInjectedSounds;
                currentSound.Generated.DelayedBy = delayedBy;
                currentSound.Generated.Comment += $"(delayed by {delayedBy})"; // delayed due to injected sounds
                currentSound.Generated.Timestamp += delayedBy;
                currentSound.Sequence.AutoDuration += delayedBy;
            }
        }

        return [currentSound.Generated];
    }
}