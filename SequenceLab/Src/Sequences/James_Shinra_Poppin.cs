namespace Beater;

public class James_Shinra_Poppin
{
    internal static Sequence GetSequence()
    {

        var sequence = new Sequence
        {
            Leader = new Sound("")
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3620, PlayEveryX = 1 },
                Followers = new()
            {
                new Sound("b2")
                {
                    // note: this sound (b2) starts before the logical leader (b1)
                    // if we want to define the sequence using leader/follower relationship - how does the follower appearing before leader fit into this model?
                    Strategy = new RepeatStrategy() { DelayAfterLeader = 450, Count = 4, Interval = 915 },
                },
                new Sound("b1")
                {
                    Strategy = new PlayOnceStrategy() { PlayEveryX = 1 },
                    Followers = new ()
                    {
                        new Sound("b1")
                        {
                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 680, PlayEveryX = 1 },
                            Followers = new ()
                            {
                                new Sound("b1")
                                {
                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 1340, PlayEveryX = 1 },
                                    Followers = new ()
                                    {
                                        new Sound("b1")
                                        {
                                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 460, PlayEveryX = 1 },
                                            Followers = new ()
                                            {
                                                new Sound("b1")
                                                {
                                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 460, PlayEveryX = 1 },
                                                    Followers = new ()
                                                    {
                                                        new Sound("b1")
                                                        {
                                                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 340, PlayEveryX = 1 },
                                                            Followers = new ()
                                                            {
                                                                new Sound("b1")
                                                                {
                                                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 100, PlayEveryX = 1 },
                                                                    Followers = new ()
                                                                    {

                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            }

        };

        return sequence;
    }
}