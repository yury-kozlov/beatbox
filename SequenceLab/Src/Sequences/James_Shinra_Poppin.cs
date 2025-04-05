namespace Beater;

public class James_Shinra_Poppin
{
    internal static Sequence GetSequence()
    {
        var sequence = new Sequence
        {
            Leader = new Sound("")
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3620 },
                Followers = new() {
                    new Sound("b1")
                    {
                        Strategy = new PlayOnceStrategy(),
                        Followers = new ()
                        {
                            new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 680 } },
                            new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 1340 } },
                            new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 460 } },
                            new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 460 } },
                            new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 340 } },
                            new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 100 } },
                        }
                    },
                    new Sound("b2")
                    {
                        // note: this sound (b2) has a constant loop, although its logic leader (b1) is played at more irregular intervals
                        Strategy = new RepeatStrategy() { DelayAfterLeader = 450, Count = 4, Interval = 915 },
                    },
                }
            }
        };

        return sequence;
    }
}