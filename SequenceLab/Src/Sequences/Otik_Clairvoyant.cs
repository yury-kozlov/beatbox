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
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 360 },
                        Followers = new () {
                            new Sound("b2") { Strategy = new PlayOnceStrategy() { DelayAfterLeader = 100 } },
                            new Sound("b1") { Strategy = new PlayOnceStrategy() { DelayAfterLeader = 340 } },
                        },
                    },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 205 } },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 100 } },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 230 } },
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 100 } },
                }
            }
        };

        return sequence;
    }
}