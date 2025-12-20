namespace Beater;

/// <summary>
/// "K  K   S       K   S     K   K   S    K    K  S"
/// </summary>
public class Scsi9_Nebula_Hotel
{
    internal static Sequence GetSequence()
    {
        var sequence = new Sequence
        {
            Leader = new Sound()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3940 },
                Followers = new()
                {
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 260 } },
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 230 } },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 750 } },
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 220 } },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 500 } },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 250 } },
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 250 } },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 400 } },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 350 } },
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 250 } },
                }
            }
        };

        return sequence;
    }
}