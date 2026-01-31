namespace Beater;

public class James_Shinra_Poppin
{
    internal static MiniSequence GetSequence()
    {
        var sequence = new MiniSequence
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3620 },
                Followers = [
                    new Kick
                    {
                        Followers = [
                            new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 680 } },
                            new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 1340 } },
                            new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 460 } },
                            new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 460 } },
                            new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 340 } },
                            new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 100 } },
                        ]
                    },
                    new Snare
                    {
                        // note: this sound (s) has a constant loop, although its logic leader (k) is played at more irregular intervals
                        Strategy = new RepeatStrategy() { DelayAfterLeader = 450, Count = 4, Interval = 915 },
                    },
                ]
            }
        };

        return sequence;
    }
}