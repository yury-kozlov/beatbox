namespace Beater;

public class FollowLeaderStrategy : AbstractStrategy
{
    /// <summary>
    /// In milliseconds.
    /// </summary>
    public int MinBufferAfterInjectedSounds = 200;

    public override GeneratedSequence ApplyStrategy(Sound leader)
    {
        leader.Timestamp = DelayAfterLeader;

        // at this point, timestamp is relative to the sequence-start (and will be shifted according to the sequence leader position later down the flow)
        leader.Sequence.AutoDuration = leader.Timestamp;


        if (leader.Injected.HasItems())
        {
            // increase delay of the current sound by total duration of injected followers + some extra buffer
            var exceedingDuration = leader.Injected.Last().Timestamp - leader.Timestamp;
            if (exceedingDuration > 0)
            {
                leader.Timestamp += exceedingDuration + MinBufferAfterInjectedSounds;
                leader.Sequence.AutoDuration += exceedingDuration + MinBufferAfterInjectedSounds;
            }
        }

        return new GeneratedSequence() { leader };
    }
}