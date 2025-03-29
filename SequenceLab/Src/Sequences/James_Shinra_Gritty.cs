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
            Leader = new Sound("")
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 1880, PlayEveryX = 1 },
                Followers = new()
            {
                new Sound("b1")
                {
                    Strategy = new PlayOnceStrategy() { PlayEveryX = 1 },
                    Followers = new ()
                    {
                        new Sound("b1")
                        {
                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 350, PlayEveryX = 1 },
                            Followers = new ()
                            {
                                new Sound("b2")
                                {
                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 115, PlayEveryX = 1 },
                                    Followers = new ()
                                    {
                                        new Sound("b1")
                                        {
                                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 210, PlayEveryX = 1 },
                                            Followers = new ()
                                            {
                                                new Sound("b1")
                                                {
                                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 470, PlayEveryX = 1 },
                                                    Followers = new ()
                                                    {
                                                        new Sound("b2")
                                                        {
                                                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 240, PlayEveryX = 1 },
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