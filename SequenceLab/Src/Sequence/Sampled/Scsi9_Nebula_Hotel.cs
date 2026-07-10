using static Beater.SoundExtensions;

namespace Beater.Sampled;

/// <summary>
/// "K  K   S       K   S     K   K   S    K    K  S"
/// </summary>
public class Scsi9_Nebula_Hotel
{
    internal static SequenceDesign GetSequence()
    {
        var sequence = new SequenceDesign(nameof(Scsi9_Nebula_Hotel))
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 3940 },
                Followers = [Chain(
                    new Kick  { },
                    new Kick  { DelayAfterLeader = 260 },
                    new Snare { DelayAfterLeader = 230 },
                    new Kick  { DelayAfterLeader = 750 },
                    new Snare { DelayAfterLeader = 220 },
                    new Kick  { DelayAfterLeader = 500 },
                    new Kick  { DelayAfterLeader = 250 },
                    new Snare { DelayAfterLeader = 250 },
                    new Kick  { DelayAfterLeader = 400 },
                    new Kick  { DelayAfterLeader = 350 },
                    new Snare { DelayAfterLeader = 250 }
                )]
            }
        };

        return sequence;
    }
}