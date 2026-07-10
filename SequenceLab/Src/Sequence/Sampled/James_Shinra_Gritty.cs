namespace Beater.Sampled;

using static Beater.SoundExtensions;

/// <summary>
/// "K    K S  K    K   S"
/// </summary>
public class James_Shinra_Gritty
{
    internal static SequenceDesign GetSequence()
    {
        var sequence = new SequenceDesign(nameof(James_Shinra_Gritty))
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 1880 },
                Followers = [Chain(
                    new Kick  { },
                    new Kick  { DelayAfterLeader = 350 },
                    new Snare { DelayAfterLeader = 115 },
                    new Kick  { DelayAfterLeader = 210 },
                    new Kick  { DelayAfterLeader = 470 },
                    new Snare { DelayAfterLeader = 240 }
                )],
            }
        };

        return sequence;
    }
}