namespace Beater;

public class PrimitiveSequences
{
    public class Square<TSound> : SequenceDesign where TSound : Sound, new()
    {
        private readonly RepeatStrategy _repeatStrategy;

        public Square(string name = "square") : base(name)
        {
            Leader = new TSound() { Strategy = _repeatStrategy = new RepeatStrategy { Count = 4 } };
        }

        public required int Interval
        {
            get => _repeatStrategy.Interval;
            set { _repeatStrategy.Interval = value; Duration = value * 4; }
        }
    }
}