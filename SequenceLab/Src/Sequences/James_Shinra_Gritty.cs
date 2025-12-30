namespace Beater;

/// <summary>
/// "K    K S  K    K   S"
/// </summary>
public class James_Shinra_Gritty
{
    internal static Sequence GetSequence()
    {
        var sequence = new Sequence
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 1880, PlayEveryX = 1 },
                Followers = new() {
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 350 } },
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 115 } },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 210 } },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 470 } },
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 240 } },
                }
            }
        };

        return sequence;
    }
}