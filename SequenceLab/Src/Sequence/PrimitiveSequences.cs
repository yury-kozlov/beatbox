namespace Beater;

public class PrimitiveSequences
{
    public class Repeat : SequenceDesign
    {
        private readonly RepeatStrategy _repeatStrategy;

        public Repeat(Sound sound, string name = "repeat") : base(name)
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

        public Sequence RepeatedFollowers
        {
            get => FirstSound?.Followers ?? [];
            set { FirstSound?.Followers = value; }
        }
    }

    public class Square<TSound> : SequenceDesign where TSound : Sound, new()
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

    public class Trapezoid<TSound> : SequenceDesign where TSound : Sound, new()
    {
        public Trapezoid(string name = "trapezoid") : base(name)
        {
            Leader = new TSound()
            {
                Strategy = new FollowLeaderStrategy { },
                Followers = [
                    new TSound { Strategy = new FollowPreviousSoundStrategy() },
                    new TSound { Strategy = new FollowPreviousSoundStrategy() },
                    new TSound { Strategy = new FollowPreviousSoundStrategy() },
                ],
            };
        }

        public required int XInterval
        {
            get;
            set
            {
                field = value;
                FirstSound?.Followers.FirstOrDefault()?.DelayAfterLeader = value;
                // Duration = XInterval*2 + YInterval*2
                Duration = (value * 2) + ((FirstSound?.Followers.LastOrDefault()?.Strategy.DelayAfterLeader ?? 0) * 2);
            }
        }

        public required int YInterval
        {
            get;
            set
            {
                field = value;
                FirstSound?.Followers.SecondOrDefault()?.DelayAfterLeader = value;
                FirstSound?.Followers.LastOrDefault()?.DelayAfterLeader = value;
                // Duration = XInterval*2 + YInterval*2
                Duration = (value * 2) + ((FirstSound?.Followers.FirstOrDefault()?.Strategy.DelayAfterLeader ?? 0) * 2);
            }
        }
    }
}