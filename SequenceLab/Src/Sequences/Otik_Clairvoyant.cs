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
            Leader = new Sound("")
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 1900, PlayEveryX = 1 },
                Followers = new()
            {
                new Sound("b1")
                {
                    Strategy = new PlayOnceStrategy() { PlayEveryX = 1 },
                    Followers = new ()
                    {
                        new Sound("b1")
                        {
                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 360, PlayEveryX = 1 },
                            Followers = new ()
                            {
                                new Sound("b2")
                                {
                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 100, PlayEveryX = 1 },
                                },
                                new Sound("b1")
                                {
                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 340, PlayEveryX = 1 },
                                    Followers = new ()
                                    {
                                        new Sound("b1")
                                        {
                                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 205, PlayEveryX = 1 },
                                            Followers = new ()
                                            {
                                                new Sound("b1")
                                                {
                                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 100, PlayEveryX = 1 },
                                                    Followers = new ()
                                                    {
                                                        new Sound("b1")
                                                        {
                                                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 230, PlayEveryX = 1 },
                                                            Followers = new ()
                                                            {
                                                                new Sound("b2")
                                                                {
                                                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 100, PlayEveryX = 1 },
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