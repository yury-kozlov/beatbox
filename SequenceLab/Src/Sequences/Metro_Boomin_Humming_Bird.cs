namespace Beater;

public class Metro_Boomin_Humming_Bird
{
    internal static Sequence GetSequence1()
    {
        return new Sequence
        {
            Leader = new Sound()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 5890 },
                Followers = new()
                {
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 710 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 400 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 730 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 560 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 540 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 165 }},

                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 390 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 545 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 370 }},
                },
            },

        };
    }

    internal static Sequence GetSequence2()
    {
        return new Sequence
        {
            Leader = new Sound()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 5890 },
                Followers = new()
                {
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 175 }},

                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 1290 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 370 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 180 }},
                    
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 900 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 195 }},
                    
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 1270 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 370 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                },
            },

        };
    }
}