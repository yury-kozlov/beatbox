namespace Beater;

public class Minimal
{
    internal static Sequence TechnoBeat1()
    {
        return new Sequence
        {
            Leader = new Sound("b1")
            {
                Strategy = new RepeatStrategy { Count = 16, Interval = 500 },
                Followers = new() {
                  new Sound("ts1") { Strategy = new RepeatStrategy { DelayAfterLeader = 150, Count = 2, Interval = 80 }},
                  new Sound("ts2") { Strategy = new PlayOnceStrategy { PlayEveryX = 4 } },
                  new Sound("b2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 250, PlayEveryX = 4 } },
                  new Sound("ts3") { Strategy = new RepeatStrategy { DelayAfterLeader = 80, Count = 4, Interval = 80, LinearIncrement = -10, PlayEveryX = 8 } },
               },
            },
        };
    }

    internal static Sequence TechnoBeat2()
    {
        return new Sequence
        {
            Leader = new Sound("b1")
            {
                Strategy = new RepeatStrategy { Count = 16, Interval = 500, SilenceEveryXSoundOutOf = "3/4" },
                Followers = new() {
                    new Sound("b2") { Strategy = new PlayOnceStrategy { PlayEveryXOutOf = "3/4" } },
                    new Sound("ts1") { Strategy = new RepeatStrategy { DelayAfterLeader = 100, Interval = 80, Count = 2, PlayEveryXOutOf="1/4" } },
                    new Sound("b1") { Strategy = new RepeatStrategy { PlayEveryXOutOf = "1/4", DelayAfterLeader = 100, Interval = 100, Count = 2 } },
                    new Sound("ts2") { Strategy = new RepeatStrategy { DelayAfterLeader = 100, Interval=100, Count=2, PlayEveryXOutOf="2/4" } },
                    new Sound("ts2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 100, PlayEveryXOutOf="4/4" } },
                    new Sound("ts3") { Strategy = new RepeatStrategy { DelayAfterLeader = 50, Count = 4, Interval = 80, LinearIncrement = -10, PlayEveryXOutOf = "2/4" } },
               },
            },
        };
    }

    internal static Sequence BrokenBeat1()
    {
        return new Sequence
        {
            Leader = new Sound("")
            {
                Strategy = new RepeatStrategy { Count = 4, Interval = 2000 },
                Followers = new()
                {
                    new Sound("b1")
                    {
                        Strategy = new RepeatStrategy { Count = 2, Interval = 250 },
                        Followers = new() {
                            new Sound("b2") {
                                Strategy = new PlayOnceStrategy { DelayAfterLeader = 250, PlayEveryX = 2 },
                                Followers = new()
                                {
                                    new Sound("b1")
                                    {
                                        Strategy = new RepeatStrategy { DelayAfterLeader = 585, Count = 2, Interval = 125 },
                                        Followers = new() {
                                            new Sound("b2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 250, PlayEveryX = 2 } }
                                        },
                                    }
                                }
                            },
                       },
                    },
                    new Sound("ts1") { Strategy = new RepeatStrategy { DelayAfterLeader = 85, Interval = 500, Count = 4 },
                        Followers = new () {
                            new Sound("ts2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 125, PlayEveryXOutOf = "2/4" } },
                        }
                    },
                },
            },
        };
    }
}
