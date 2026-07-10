using static Beater.SoundExtensions;

namespace Beater.Sampled;

/// <summary>
/// 1        1   2      1    1  1    1   2
/// </summary>
public class Otik_Clairvoyant
{
    internal static SequenceDesign GetSequence()
    {
        var sequence = new SequenceDesign(nameof(Otik_Clairvoyant))
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 1900 },
                Followers = [ Chain(
                        new Kick  { },
                        new Kick  { DelayAfterLeader = 360 },
                        new Snare { DelayAfterLeader = 100 },
                        new Kick  { DelayAfterLeader = 240 },
                        new Kick  { DelayAfterLeader = 205 },
                        new Kick  { DelayAfterLeader = 100 },
                        new Kick  { DelayAfterLeader = 230 },
                        new Snare { DelayAfterLeader = 100 }
                )]
            }
        };

        return sequence;
    }
}