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
            Leader = new Sound("")
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3940, PlayEveryX = 1 },
                Followers = new()
            {
                new Sound("b1")
                {
                    Strategy = new PlayOnceStrategy() { PlayEveryX = 1 },
                    Followers = new ()
                    {
                        new Sound("b1")
                        {
                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 260, PlayEveryX = 1 },
                            Followers = new ()
                            {
                                new Sound("b2")
                                {
                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 230, PlayEveryX = 1 },
                                    Followers = new ()
                                    {
                                        new Sound("b1")
                                        {
                                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 750, PlayEveryX = 1 },
                                            Followers = new ()
                                            {
                                                new Sound("b2")
                                                {
                                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 220, PlayEveryX = 1 },
                                                    Followers = new ()
                                                    {
                                                        new Sound("b1")
                                                        {
                                                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 500, PlayEveryX = 1 },
                                                            Followers = new ()
                                                            {
                                                                new Sound("b1")
                                                                {
                                                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 250, PlayEveryX = 1 },
                                                                    Followers = new ()
                                                                    {
                                                                        new Sound("b2")
                                                                        {
                                                                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 250, PlayEveryX = 1 },
                                                                            Followers = new ()
                                                                            {
                                                                                new Sound("b1")
                                                                                {
                                                                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 400, PlayEveryX = 1 },
                                                                                    Followers = new ()
                                                                                    {
                                                                                        new Sound("b1")
                                                                                        {
                                                                                            Strategy = new PlayOnceStrategy() { DelayAfterLeader = 350, PlayEveryX = 1 },
                                                                                            Followers = new ()
                                                                                            {
                                                                                                new Sound("b2")
                                                                                                {
                                                                                                    Strategy = new PlayOnceStrategy() { DelayAfterLeader = 250, PlayEveryX = 1 },
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