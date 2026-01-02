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
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3700 },
                Followers = [
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() },
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 235 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 210 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 80 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 130 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 270 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 215 }},
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200 }},

                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 470 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 240 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 215 }},
                    new Kick { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 240 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 225 }},
                    new Snare { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 220 }},
                    new Sound("ts2") { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 255 }},
                ]
            },
        };
    }
}
