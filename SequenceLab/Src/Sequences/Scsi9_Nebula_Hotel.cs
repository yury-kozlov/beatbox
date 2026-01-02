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
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3940 },
                Followers = new()
                {
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 260 } },
                    new Sound("s") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 230 } },
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 750 } },
                    new Sound("s") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 220 } },
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 500 } },
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 250 } },
                    new Sound("s") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 250 } },
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 400 } },
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 350 } },
                    new Sound("s") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 250 } },
                }
            }
        };

        return sequence;
    }
}