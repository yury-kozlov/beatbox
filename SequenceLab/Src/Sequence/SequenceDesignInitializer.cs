namespace Beater;

public class SequenceDesignInitializer
{
    private readonly SequenceDesign _design;

    public SequenceDesignInitializer(SequenceDesign design)
    {
        _design = design;
    }

    public Sound SetLeader(Sound value)
    {
        if (value is SequenceStart)
        {
            // if it's initial call from ctor the value is already SequenceStart, no need to enforce it:
            return value;
        }
        var leader = EnforceSequenceStartLeader(value);
        UpdateLoopDuration(value.Strategy); // if leader is a loop, we can use it's interval to calculate total sequence duration
        return leader;
    }

    public void SetStrategy(AbstractStrategy value)
    {
        _design.Leader.Strategy = value;
        UpdateLoopInterval();
        UpdateLoopDuration(value);
    }

    public void OnDurationSet()
    {
        _design.SequenceEnd.InitStrategy(); // recalculate sequence end strategy because it depends on duration
        UpdateLoopInterval();
    }

    /// <summary>
    /// Initializes interval of sequence loop when sequence strategy was changed or duration of the sequence was changed.
    /// NOTE: this is a shortcut path to omit specifying explicit Interval in repeated sequences (e.g. <see cref="PrimitiveSequences.Trapezoid{TSound}"/>)
    /// when duration of the underlying sequence is known and we just need to calculate total repetition time:
    /// in this case sequence loop interval will be automatically set as duration of the original sequence
    /// </summary>
    public void UpdateLoopInterval()
    {
        if (_design.Strategy is RepeatStrategy sequenceLoop)
        {
            if (_design.Duration > 0)
            {
                // NOTE: sequence Duration includes all iterations of a loop
                sequenceLoop.Interval = sequenceLoop.Count > 0 ? _design.Duration / sequenceLoop.Count : _design.Duration;
            }
        }
    }

    /// <summary>
    /// Updates duration (as a result of strategy change, or leader initialization).
    /// NOTE: change in sequence duration also triggers re-calculation of loop interval.
    /// </summary>
    public void UpdateLoopDuration(AbstractStrategy leaderStrategy)
    {
        if (leaderStrategy is RepeatStrategy repeatStrategy && repeatStrategy.IsInitialized)
        {
            // if sequence is repeated, it's duration should be multipled by number of iterations:
            var sequenceLoops = (_design.Strategy as RepeatStrategy)?.Count ?? 1;
            _design.Duration = repeatStrategy.Interval * repeatStrategy.Count * sequenceLoops;
        }
    }

    public void SetDelayAfterLeader(int value)
    {
        _design.Strategy.DelayAfterLeader = value;
    }

    /// <summary>
    /// Force actual leader to be a follower of "SequenceStart" sound.
    /// </summary>
    private Sound EnforceSequenceStartLeader(Sound leader)
    {
        return _design.Leader
            .WithFollower(leader.WithSequenceIfMissing(_design))
            .WithFollower(_design.SequenceEnd);
    }
}
