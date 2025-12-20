
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
            Leader = new Sound()
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

    internal static Sequence SlowBeat1()
    {
        return new Sequence
        {
            Leader = new Sound()
            {
                Strategy = new RepeatStrategy { Count = 4, Interval = 2550 },
                Followers = new()
                {
                    new Sound("b1")
                    {
                        Strategy = new RepeatStrategy { Count = 2, Interval = 330 },
                        Followers = new() {
                            new Sound("b2") {
                                Strategy = new PlayOnceStrategy { DelayAfterLeader = 330, PlayEveryX = 2 },
                                Followers = new()
                                {
                                    new Sound("b1")
                                    {
                                        Strategy = new PlayOnceStrategy { DelayAfterLeader = 950, PlayEveryXOutOf="1/2" },
                                        Followers = new() { new Sound("b2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 330 }}},
                                    },
                                    new Sound("b1")
                                    {
                                        Strategy = new RepeatStrategy { DelayAfterLeader = 500, PlayEveryXOutOf="2/2", Count = 2, Interval = 450 },
                                        Followers = new() {new Sound("b2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 330,  PlayEveryX = 2 } }},
                                    }
                                }
                            },
                       },
                    },
                },
            },
        };
    }

    /// <summary>
    /// K         K   S      S   S   K      K      S           
    /// </summary>
    internal static Sequence SlowBeat2()
    {
        return new Sequence
        {
            Leader = new Sound()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 4700 },
                Followers = new()
                {
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 900 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 600 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 600 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 600 }},
                },
            },

        };
    }

    /// <summary>
    /// K   K  K   S   K        K    S      S      K          
    /// </summary>
    internal static Sequence SlowBeat3()
    {
        return new Sequence
        {
            Leader = new Sound()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 4400 },
                Followers = new()
                {
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {PlayEveryX = 1}},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 800 }},
                    // those two percussions need more accent from some other sound:
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 500 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 600 }},
                },
            },
        };
    }

    /// <summary>
    /// K    K    S         S   K      K   K   S         S   K       
    /// </summary>
    internal static Sequence SlowBeat4()
    {
        return new Sequence
        {
            Leader = new Sound()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 2400 },
                Followers = new()
                {
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 900 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                },
            },
        };
    }
}
