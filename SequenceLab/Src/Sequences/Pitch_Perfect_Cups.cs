namespace Beater;

/// <summary>
/// S  S  K K K  K  K   S    K  K  K  K  K  K  S
/// </summary>
public class Pitch_Perfect_Cups 
{
    internal static Sequence GetSequence()
    {
        return new Sequence
        {
            Leader = new Sound("")
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3600, PlayEveryX = 1 },
                Followers = new ()
                {
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() {PlayEveryX = 1}},
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 240, PlayEveryX = 1}},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 220, PlayEveryX = 1}},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 80, PlayEveryX = 1}},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 80, PlayEveryX = 1}},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 225, PlayEveryX = 1}},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 210, PlayEveryX = 1}},
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 210, PlayEveryX = 1}},

                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 470, PlayEveryX = 1}},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 240, PlayEveryX = 1}},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 215, PlayEveryX = 1}},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 235, PlayEveryX = 1}},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 225, PlayEveryX = 1}},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 220, PlayEveryX = 1}},
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 250, PlayEveryX = 1}},
                },
            },

        };
    }
}
