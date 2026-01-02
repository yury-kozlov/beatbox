namespace Beater;

/// <summary>
/// 1        1   2      1    1  1    1   2
/// </summary>
public class Otik_Clairvoyant
{
    internal static Sequence GetSequence()
    {
        var sequence = new Sequence
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 1900 },
                Followers = new()
                {
                    new Kick { Strategy = new FollowPreviousSoundStrategy() },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 360 },
                        Followers = new () {
                            new Snare { Strategy = new PlayOnceStrategy() { DelayAfterLeader = 100 } },
                            new Kick { Strategy = new PlayOnceStrategy() { DelayAfterLeader = 340 } },
                        },
                    },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 205 } },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 100 } },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 230 } },
                    new Snare { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 100 } },
                }
            }
        };

        return sequence;
    }
}