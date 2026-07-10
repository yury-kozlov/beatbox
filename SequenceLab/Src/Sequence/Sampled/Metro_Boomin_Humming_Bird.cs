using static Beater.SoundExtensions;

namespace Beater.Sampled;

public class Metro_Boomin_Humming_Bird
{
    internal static SequenceDesign GetSequence1()
    {
        return new SequenceDesign(nameof(Metro_Boomin_Humming_Bird))
        {
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 4, Interval = 5890 },
                Followers = [Chain(
                    new Kick  { },
                    new Snare { DelayAfterLeader = 710 },
                    new Kick  { DelayAfterLeader = 400 },
                    new Kick  { DelayAfterLeader = 730 },
                    new Snare { DelayAfterLeader = 350 },

                    new Snare { DelayAfterLeader = 560 },
                    new Kick  { DelayAfterLeader = 200 },
                    new Kick  { DelayAfterLeader = 540 },
                    new Snare { DelayAfterLeader = 165 },

                    new Kick  { DelayAfterLeader = 390 },
                    new Snare { DelayAfterLeader = 545 },
                    new Kick  { DelayAfterLeader = 200 },
                    new Snare { DelayAfterLeader = 350 },
                    new Kick  { DelayAfterLeader = 370 }
                )],
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
                Followers = [Chain(
                    new Kick { },
                    new Kick { DelayAfterLeader = 175  },

                    new Kick { DelayAfterLeader = 1290 },
                    new Kick { DelayAfterLeader = 370  },
                    new Kick { DelayAfterLeader = 180  },

                    new Kick { DelayAfterLeader = 900  },
                    new Kick { DelayAfterLeader = 195  },

                    new Kick { DelayAfterLeader = 1270 },
                    new Kick { DelayAfterLeader = 370  },
                    new Kick { DelayAfterLeader = 200  },

                    new Kick { DelayAfterLeader = 350  },
                    new Kick { DelayAfterLeader = 200  },
                    new Kick { DelayAfterLeader = 200  }
                )],
            },
        };
    }
}