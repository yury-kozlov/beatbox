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
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 360 },
                        Followers = new () {
                            new Sound("s") { Strategy = new PlayOnceStrategy() { DelayAfterLeader = 100 } },
                            new Sound("k") { Strategy = new PlayOnceStrategy() { DelayAfterLeader = 340 } },
                        },
                    },
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 205 } },
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 100 } },
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 230 } },
                    new Sound("s") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 100 } },
                }
            }
        };

        return sequence;
    }
}