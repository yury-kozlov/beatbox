namespace Beater;

/// <summary>
/// Noe Solange - Home (DJ Counselling remix) beat pattern.
/// </summary>
public class Noe_Solange_Home_DJCounselling
{
    internal static SequenceDesign GetSequence_Combined()
    {
        var kicks = new SequenceDesign("kicks")
        {
            Duration = 1900,
            Leader = new Kick()
            {
                Followers = [
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 480 } },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 350 } },
                ]
            }
        };
        var snares = new PrimitiveSequences.Square<Snare1>() { DelayAfterLeader = 235, Interval = 470 };
        var main = new SequenceDesign(nameof(Noe_Solange_Home_DJCounselling))
        {
            Strategy = new RepeatStrategy { Count = 4 }
        };

        return main.Combine(kicks).Combine(snares);
    }

    internal static SequenceDesign GetSequence()
    {
        var sequence = new SequenceDesign(nameof(Noe_Solange_Home_DJCounselling))
        {
            Duration = 1900,
            Strategy = new RepeatStrategy { Count = 4 },
            Leader = new Kick()
            {
                Followers = [

                    new Snare1 { DelayAfterLeader = 235, Strategy = new RepeatStrategy { FireAndForget = true, Interval = 475, Count = 4 } },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 475 } },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 355 } },
                ]
            }
        };

        return sequence;
    }
}
