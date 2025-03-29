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