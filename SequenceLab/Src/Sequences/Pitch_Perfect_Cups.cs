namespace Beater;

/// <summary>
/// S  S  K K K  K  K   S    K  K  K  K  K  K  S
/// </summary>
public class Pitch_Perfect_Cups
{
    internal static Sequence GetSequence()
    {
        // NOTE: this sequence doesn't have clear borders, so when listening from the middle, the pattern may be unrecognizable.
        // in order to put accent to its start, need to add some distinguishable sound at the beginning of each loop
        return new Sequence
        {
            Leader = new Sound("")
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3600 },
                Followers = new()
                {
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 240 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 220 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 80 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 80 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 225 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 210 }},
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 210 }},

                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 470 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 240 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 215 }},
                    new Sound("b1") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 235 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 225 }},
                    new Sound("b2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 220 }},
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 250 }},
                },
            },

        };
    }
}
