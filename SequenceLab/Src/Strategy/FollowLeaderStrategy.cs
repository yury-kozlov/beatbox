namespace Beater;

public class FollowLeaderStrategy : AbstractStrategy
{
    /// <summary>
    /// In milliseconds.
    /// </summary>
    public int MinBufferAfterInjectedSounds = 200;

    public override GeneratedSequence ApplyStrategy(SoundDesign leader)
    {
        leader.Generated.Timestamp = DelayAfterLeader;

        // at this point, timestamp is relative to the sequence-start (and will be shifted according to the sequence leader position later down the flow)
        leader.Sequence.AutoDuration = leader.Generated.Timestamp;


        if (leader.Injected.HasItems())
        {
            // increase delay of the current sound by total duration of injected followers + some extra buffer
            var exceedingDuration = leader.Injected.Last().Timestamp - leader.Generated.Timestamp;
            if (exceedingDuration > 0)
            {
                leader.Generated.Timestamp += exceedingDuration + MinBufferAfterInjectedSounds;
                leader.Sequence.AutoDuration += exceedingDuration + MinBufferAfterInjectedSounds;
            }
        }

        return [leader.Generated];
    }
}