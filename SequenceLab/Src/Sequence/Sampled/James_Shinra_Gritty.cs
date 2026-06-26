namespace Beater.Sampled;

/// <summary>
/// "K    K S  K    K   S"
/// </summary>
public class James_Shinra_Gritty
{
    internal static SequenceDesign GetSequence()
    {
        var sequence = new SequenceDesign(nameof(James_Shinra_Gritty))
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 1880 },
                Followers = [
                    new Kick { Strategy = new FollowPreviousSoundStrategy() },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 350 } },
                    new Snare { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 115 } },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 210 } },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 470 } },
                    new Snare { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 240 } },
                ]
            }
        };

        return sequence;
    }
}