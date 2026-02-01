namespace Beater;

public class Metro_Boomin_Humming_Bird
{
    internal static SequenceDesign GetSequence1()
    {
        return new SequenceDesign(nameof(Metro_Boomin_Humming_Bird))
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 5890 },
                Followers = [
                    new Kick { Strategy = new FollowPreviousSoundStrategy() },
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 710 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 400 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 730 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 560 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 540 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 165 }},

                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 390 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 545 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 370 }},
                ]
            }
        };
    }

    internal static SequenceDesign GetSequence2()
    {
        return new SequenceDesign(nameof(GetSequence2))
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 5890 },
                Followers = [
                    new Kick { Strategy = new FollowPreviousSoundStrategy() },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 175 }},

                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 1290 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 370 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 180 }},
                    
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 900 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 195 }},
                    
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 1270 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 370 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 350 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},
                ]
            },
        };
    }
}