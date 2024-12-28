namespace Beater;

public class Minimal
{
    internal static Sequence TechnoBeat1()
    {
        return new Sequence
        {
            Leader = new Sound("b1")
            {
                Strategy = new RepeatStrategy { Count = 32, Interval = 500 },
                Followers = new() {
                  new Sound("ts1") {
                      Strategy = new RepeatStrategy { DelayAfterLeader = 150, Count = 2, Interval = 80 },
                  },
                  new Sound("ts2") { Strategy = new PlayOnceStrategy { PlayEveryX = 4 } },
                  new Sound("b2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 250, PlayEveryX = 4 } },
                  new Sound("ts3") { Strategy = new RepeatStrategy { DelayAfterLeader = 80, Count = 4, Interval = 80, LinearIncrement = -10, PlayEveryX = 8 } },
                  // new Sound("b1") { Strategy = new RepeatStrategy { DelayAfterLeader = 80, Count = 4, Interval = 80, LinearIncrement = -10, PlayEveryX = 16 } },
               },
            },
        };
    }
}
