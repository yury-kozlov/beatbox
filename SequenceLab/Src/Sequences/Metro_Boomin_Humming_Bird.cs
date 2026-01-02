namespace Beater;

public class Metro_Boomin_Humming_Bird
{
    internal static Sequence GetSequence1()
    {
        return new Sequence
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 5890 },
                Followers = new()
                {
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("s") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 710 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 400 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 730 }},
                    new Sound("s") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    
                    new Sound("s") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 560 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 540 }},
                    new Sound("s") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 165 }},

                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 390 }},
                    new Sound("s") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 545 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Sound("s") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 370 }},
                },
            },

        };
    }

    internal static Sequence GetSequence2()
    {
        return new Sequence
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 5890 },
                Followers = new()
                {
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 175 }},

                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 1290 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 370 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 180 }},
                    
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 900 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 195 }},
                    
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 1270 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 370 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Sound("k") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                },
            },

        };
    }
}