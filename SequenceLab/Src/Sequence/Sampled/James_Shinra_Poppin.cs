using static Beater.SoundExtensions;

namespace Beater.Sampled;

public class James_Shinra_Poppin
{
    internal static SequenceDesign GetSequence()
    {
        var sequence = new SequenceDesign(nameof(James_Shinra_Poppin))
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3620 },
                Followers = [
                    Chain(new Kick { },
                          new Kick { DelayAfterLeader = 680 },
                          new Kick { DelayAfterLeader = 1340 },
                          new Kick { DelayAfterLeader = 460 },
                          new Kick { DelayAfterLeader = 460 },
                          new Kick { DelayAfterLeader = 340 },
                          new Kick { DelayAfterLeader = 100 }),
                    new Snare
                    {
                        // note: this sound (s) has a constant loop, although its logic leader (k) is played at more irregular intervals
                        Strategy = new RepeatStrategy() { DelayAfterLeader = 450, Count = 4, Interval = 915 },
                    },
                ]
            }
        };

        return sequence;
    }
}