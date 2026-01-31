
namespace Beater;

public class Minimal
{
    internal static SequenceDesign TechnoBeat1()
    {
        return new SequenceDesign
        {
            Leader = new Kick
            {
                Strategy = new RepeatStrategy { Count = 16, Interval = 500 },
                Followers = [
                  new Sound("ts1") { Strategy = new RepeatStrategy { DelayAfterLeader = 150, Count = 2, Interval = 80 }},
                  new Sound("ts2") { Strategy = new PlayOnceStrategy { PlayEveryX = 4 } },
                  new Snare { Strategy = new PlayOnceStrategy { DelayAfterLeader = 250, PlayEveryX = 4 } },
                  new Sound("ts3") { Strategy = new RepeatStrategy { DelayAfterLeader = 80, Count = 4, Interval = 80, LinearIncrement = -10, PlayEveryX = 8 } },
               ]
            },
        };
    }

    internal static SequenceDesign TechnoBeat2()
    {
        return new SequenceDesign
        {
            Leader = new Kick
            {
                Strategy = new RepeatStrategy { Count = 16, Interval = 500, SilenceEveryXSoundOutOf = "3/4" },
                Followers = [
                    new Snare { Strategy = new PlayOnceStrategy { PlayEveryXOutOf = "3/4" } },
                    new Sound("ts1") { Strategy = new RepeatStrategy { DelayAfterLeader = 100, Interval = 80, Count = 2, PlayEveryXOutOf="1/4" } },
                    new Kick { Strategy = new RepeatStrategy { PlayEveryXOutOf = "1/4", DelayAfterLeader = 100, Interval = 100, Count = 2 } },
                    new Sound("ts2") { Strategy = new RepeatStrategy { DelayAfterLeader = 100, Interval=100, Count=2, PlayEveryXOutOf="2/4" } },
                    new Sound("ts2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 100, PlayEveryXOutOf="4/4" } },
                    new Sound("ts3") { Strategy = new RepeatStrategy { DelayAfterLeader = 50, Count = 4, Interval = 80, LinearIncrement = -10, PlayEveryXOutOf = "2/4" } },
               ]
            },
        };
    }

    internal static SequenceDesign BrokenBeat1()
    {
        return new SequenceDesign
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy { Count = 4, Interval = 2000 },
                Followers = [
                    new Kick
                    {
                        Strategy = new RepeatStrategy { Count = 2, Interval = 250 },
                        Followers = [
                            new Snare {
                                Strategy = new PlayOnceStrategy { DelayAfterLeader = 250, PlayEveryX = 2 },
                                Followers = [
                                    new Kick
                                    {
                                        Strategy = new RepeatStrategy { DelayAfterLeader = 585, Count = 2, Interval = 125 },
                                        Followers = [
                                            new Snare { Strategy = new PlayOnceStrategy { DelayAfterLeader = 250, PlayEveryX = 2 } }
                                        ]
                                    }
                                ]
                            }
                        ]
                    },
                    new Sound("ts1") { Strategy = new RepeatStrategy { DelayAfterLeader = 85, Interval = 500, Count = 4 },
                        Followers = [
                            new Sound("ts2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 125, PlayEveryXOutOf = "2/4" } },
                        ]
                    },
                ]
            },
        };
    }

    internal static SequenceDesign SlowBeat1WithRepeats()
    {
        return new SequenceDesign
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy { Count = 4, Interval = 2550 },
                Followers = [
                    new Kick
                    {
                        Strategy = new RepeatStrategy { Count = 2, Interval = 330 },
                        Followers = [
                            new Snare {
                                Strategy = new PlayOnceStrategy { DelayAfterLeader = 330, PlayEveryX = 2 },
                                Followers = [
                                    new Kick
                                    {
                                        Strategy = new PlayOnceStrategy { DelayAfterLeader = 950, PlayEveryXOutOf="1/2" },
                                        Followers = [ new Snare { Strategy = new PlayOnceStrategy { DelayAfterLeader = 330 }}],
                                    },
                                    new Kick
                                    {
                                        Strategy = new RepeatStrategy { DelayAfterLeader = 500, PlayEveryXOutOf="2/2", Count = 2, Interval = 450 },
                                        Followers = [ new Snare { Strategy = new PlayOnceStrategy { DelayAfterLeader = 330,  PlayEveryX = 2 } }],
                                    }
                                ]
                            },
                       ]
                    },
                ]
            }
        };
    }

    internal static SequenceDesign SlowBeat1WithoutRepeats()
    {
        return new SequenceDesign
        {
            Duration = 5100,
            Leader = new Kick()
            {
                Tags = ["group-1"],
                Followers = [
                    new Kick { DelayAfterLeader = 330 },
                    new Snare { DelayAfterLeader = 330*2 },

                    new Kick { DelayAfterLeader = 330*4 + 290, Tags = ["group-2"],
                        Followers = [new Snare { DelayAfterLeader = 330 }]
                    },

                    new Kick { DelayAfterLeader = 2550, Tags = ["group-3"],
                        Followers = [
                            new Kick { DelayAfterLeader = 330 },
                            new Snare { DelayAfterLeader = 330*2 },

                            new Kick { DelayAfterLeader = 330*3 + 170, Tags = ["group-4"],
                                Followers = [
                                    new Kick { DelayAfterLeader = 450 },
                                    new Snare { DelayAfterLeader = 450+330 },
                                ]
                            }
                        ]
                    },
                ]
            }
        };
    }

    /// NOTE: this sequence is identical to <see cref="SlowBeat1WithoutRepeats"/> , but uses k1,s1 instead of k,s to be able to play different sampler sets together
    internal static SequenceDesign SlowBeat1WithoutRepeatsK1S1()
    {
        return new SequenceDesign
        {
            Duration = 5100,
            Name = "sb1",
            Leader = new Sound("k1")
            {
                Tags = ["group-1"],
                Followers = [
                    new Sound("k1") { DelayAfterLeader = 330 },
                    new Sound("s1") { DelayAfterLeader = 330*2 },

                    new Sound("k1") { DelayAfterLeader = 330*4 + 290, Tags = ["group-2"],
                        Followers = [new Sound("s1") { DelayAfterLeader = 330 }]
                    },

                    new Sound("k1") { DelayAfterLeader = 2550, Tags = ["group-3"],
                        Followers = [
                            new Sound("k1") { DelayAfterLeader = 330 },
                            new Sound("s1") { DelayAfterLeader = 330*2 },

                            new Sound("k1") { DelayAfterLeader = 330*3 + 170, Tags = ["group-4"],
                                Followers = [
                                    new Sound("k1") { DelayAfterLeader = 450 },
                                    new Sound("s1") { DelayAfterLeader = 450+330 },
                                ]
                            }
                        ]
                    },
                ]
            }
        };
    }

    /// <summary>
    /// K         K   S      S   S   K      K      S           
    /// </summary>
    internal static SequenceDesign SlowBeat2()
    {
        return new SequenceDesign
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 4700 },
                Followers =
                [
                    new Kick { Strategy = new FollowPreviousSoundStrategy() },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 900 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 600 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 600 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 600 }},
                ]
            },

        };
    }

    /// <summary>
    /// K   K  K   S   K        K    S      S      K          
    /// </summary>
    internal static SequenceDesign SlowBeat3()
    {
        return new SequenceDesign
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 4400 },
                Followers = [
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {PlayEveryX = 1}},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 800 }},
                    // those two percussions need more accent from some other sound:
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 500 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 600 }},
                ]
            },
        };
    }

    /// <summary>
    /// K    K    S         S   K      K   K   S         S   K       
    /// </summary>
    internal static SequenceDesign SlowBeat4()
    {
        return new SequenceDesign
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 2400 },
                Followers =
                [
                    new Kick { Strategy = new FollowPreviousSoundStrategy() },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 900 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 300 }},
                ]
            },
        };
    }
}
