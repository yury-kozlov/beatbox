using static Beater.SoundExtensions;

namespace Beater;

public class PrimitiveSequences
{
    public class Repeat : SequenceDesign
    {
        private readonly RepeatStrategy _repeatStrategy;

        public Repeat(SoundDesign sound, string name = "repeat") : base(name)
        {
            Leader = sound;
            Leader.Strategy = _repeatStrategy = new RepeatStrategy();
        }

        public required int Count
        {
            get => _repeatStrategy.Count;
            set { _repeatStrategy.Count = value; Duration = value * Interval; }
        }

        public new required int Interval
        {
            get => _repeatStrategy.Interval;
            set { _repeatStrategy.Interval = value; Duration = value * Count; }
        }

        public FollowersDesign RepeatedFollowers
        {
            get => FirstSound?.Followers ?? [];
            set { FirstSound?.Followers = value; }
        }
    }

    public class Square<TSound> : SequenceDesign where TSound : SoundDesign, new()
    {
        private readonly RepeatStrategy _repeatStrategy;

        public Square(string name = "square") : base(name)
        {
            Leader = new TSound() { Strategy = _repeatStrategy = new RepeatStrategy { Count = 4 } };
        }

        public required new int Interval
        {
            get => _repeatStrategy.Interval;
            set { _repeatStrategy.Interval = value; Duration = value * 4; }
        }
    }

    public class Trapezoid<TSound> : SequenceDesign where TSound : SoundDesign, new()
    {
        private readonly TSound _s2;
        private readonly TSound _s3;
        private readonly TSound _s4;

        public Trapezoid(string name = "trapezoid") : base(name)
        {
            Leader = Chain(
                    new TSound(),
                    _s2 = new TSound(),
                    _s3 = new TSound(),
                    _s4 = new TSound()
            );
        }

        public required int XInterval
        {
            get;
            set
            {
                field = value;
                _s2?.DelayAfterLeader = value;
                // Duration = XInterval*2 + YInterval*2
                Duration = (value * 2) + (_s4?.DelayAfterLeader * 2 ?? 0);
            }
        }

        public required int YInterval
        {
            get;
            set
            {
                field = value;
                _s3?.DelayAfterLeader = value;
                _s4?.DelayAfterLeader = value;
                // Duration = XInterval*2 + YInterval*2
                Duration = (value * 2) + (_s2?.DelayAfterLeader * 2 ?? 0);
            }
        }
    }
}