
namespace Beater;

public class SequenceDesignInitializer
{
    private readonly SequenceDesign _design;

    public SequenceDesignInitializer(SequenceDesign design)
    {
        _design = design;
    }

    public Sound SetLeader(Sound leader)
    {
        if (leader is SequenceStart)
        {
            // if it's initial call from ctor the value is already SequenceStart, no need to enforce it:
            return leader;
        }
        var newLeader = EnforceSequenceStartLeader(leader);
        UpdateLoopDuration(leader.Strategy); // if leader is a loop, we can use it's interval to calculate total sequence duration
        return newLeader;
    }

    public void SetStrategy(AbstractStrategy strategy)
    {
        var initialDuration = _design.Duration;
        _design.Leader.Strategy = strategy;
        UpdateLoopInterval();

        if (initialDuration > 0 && strategy is RepeatStrategy outer && outer.Count > 0)
        {
            // Inner loop duration already established; scale by outer loop count
            _design.Duration = initialDuration * outer.Count;
        }
        else if (initialDuration == 0 && strategy is RepeatStrategy self && self.IsInitialized)
        {
            // No inner loop; set duration directly from strategy's own Interval * Count
            _design.Duration = self.Interval * self.Count;
        }
        else
        {
            UpdateLoopDuration(strategy);
        }
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
    private void UpdateLoopDuration(AbstractStrategy leaderStrategy)
    {
        if (leaderStrategy is RepeatStrategy repeatStrategy && repeatStrategy.IsInitialized)
        {
            // if sequence is repeated, it's duration should be multipled by number of iterations:
            var sequenceLoops = (_design.Strategy as RepeatStrategy)?.Count ?? 1;
            _design.Duration = repeatStrategy.Interval * repeatStrategy.Count * sequenceLoops;
        }
    }

    public void AppendDuration(int appendedDuration)
    {
        if (_design.IsEmpty && _design.Strategy is RepeatStrategy repeatStrategy && repeatStrategy.IsInitialized)
        {
            // if base sequence is empty but it has predefined loop intervals, we need to expand the loop interval to accomodate the appended sequence
            repeatStrategy.Interval = Math.Max(repeatStrategy.Interval, appendedDuration);
            _design.Duration = repeatStrategy.Interval * repeatStrategy.Count;
            return;
        }
        
        // accumulate total duration
        _design.Duration += appendedDuration;
        UpdateLoopDuration(_design.Strategy);
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
