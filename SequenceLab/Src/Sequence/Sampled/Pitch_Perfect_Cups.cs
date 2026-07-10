using static Beater.SoundExtensions;

namespace Beater.Sampled;

/// <summary>
/// S  S  K K K  K  K   S    K  K  K  K  K  K  S
/// </summary>
public class Pitch_Perfect_Cups
{
    internal static SequenceDesign GetSequence()
    {
        // NOTE: this sequence doesn't have clear borders, so when listening from the middle, the pattern may be unrecognizable.
        // in order to put accent to its start, need to add some distinguishable sound at the beginning of each loop
        return new SequenceDesign(nameof(Pitch_Perfect_Cups))
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3700 },
                Followers = [Chain(
                    new Sound("ts2") { },
                    new Sound("ts2") { DelayAfterLeader = 235 },
                    new Kick  { DelayAfterLeader = 210 },
                    new Kick  { DelayAfterLeader = 80  },
                    new Kick  { DelayAfterLeader = 130 },
                    new Snare { DelayAfterLeader = 270 },
                    new Snare { DelayAfterLeader = 215 },
                    new Sound("ts2") { DelayAfterLeader = 200 },

                    new Kick  { DelayAfterLeader = 470 },
                    new Kick  { DelayAfterLeader = 240 },
                    new Kick  { DelayAfterLeader = 215 },
                    new Kick  { DelayAfterLeader = 240 },
                    new Snare { DelayAfterLeader = 225 },
                    new Snare { DelayAfterLeader = 220 },
                    new Sound("ts2") { DelayAfterLeader = 255 }
                )]
            },
        };
    }
}
