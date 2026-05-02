
namespace Beater;

public class SequenceDesignState
{
    private readonly SequenceDesign _sequenceDesign;

    public Sound Leader { get; private set; }
    public int Duration { get; private set; }
    public SequenceEnd SequenceEnd { get; }
    public List<SequenceDesign> Sequences { get; } = [];
    public bool IsEmpty => Leader.Followers.Count == 0;

    public SequenceDesignState(SequenceDesign sequenceDesign, string name)
    {
        _sequenceDesign = sequenceDesign;
        SequenceEnd = new SequenceEnd(sequenceDesign);
        Leader = new SequenceStart(name) { Sequence = sequenceDesign };
    }

    public void SetLeader(Sound leader)
    {
        if (leader is SequenceStart sequenceStart)
        {
            // during JSON deserialization, Leader is restored directly as an existing SequenceStart with followers intact:
            Leader = sequenceStart;
            return;
        }

        // force actual leader to be a follower of "SequenceStart" sound.
        Leader.WithFollower(leader.WithSequenceIfMissing(_sequenceDesign)).WithFollower(SequenceEnd);

        UpdateLoopDuration(leader.Strategy); // if leader is a loop, we can use its interval to calculate total sequence duration
    }

    public void SetStrategy(AbstractStrategy strategy)
    {
        Leader.Strategy = strategy;
        if (Duration > 0 && strategy is RepeatStrategy outer && outer.Count > 0)
        {
            // Inner loop duration already established; scale by outer loop count
            SetDuration(Duration * outer.Count);
        }
        else if (Duration == 0 && strategy is RepeatStrategy self && self.IsInitialized)
        {
            // No inner loop; set duration directly from strategy's own Interval * Count
            SetDuration(self.Interval * self.Count);
        }
    }

    public void SetDuration(int value)
    {
        Duration = value;
        SequenceEnd.InitStrategy(); // recalculate sequence end strategy because it depends on duration
        UpdateLoopInterval();
    }

    public void AppendDuration(int appendedDuration)
    {
        if (IsEmpty && Leader.Strategy is RepeatStrategy repeatStrategy && repeatStrategy.IsInitialized)
        {
            // if base sequence is empty but it has predefined loop intervals, we need to expand the loop interval to accommodate the appended sequence
            repeatStrategy.Interval = Math.Max(repeatStrategy.Interval, appendedDuration);
            SetDuration(repeatStrategy.Interval * repeatStrategy.Count);
            return;
        }

        // accumulate total duration; UpdateLoopDuration then scales by outer loop count if applicable
        SetDuration(Duration + appendedDuration);
        UpdateLoopDuration(Leader.Strategy);
    }

    /// <summary>
    /// Initializes interval of sequence loop when sequence strategy was changed or duration of the sequence was changed.
    /// NOTE: this is a shortcut path to omit specifying explicit Interval in repeated sequences (e.g. <see cref="PrimitiveSequences.Trapezoid{TSound}"/>)
    /// when duration of the underlying sequence is known and we just need to calculate total repetition time:
    /// in this case sequence loop interval will be automatically set as duration of the original sequence
    /// </summary>
    private void UpdateLoopInterval()
    {
        if (Leader.Strategy is RepeatStrategy sequenceLoop && Duration > 0)
        {
            // NOTE: sequence Duration includes all iterations of a loop
            sequenceLoop.Interval = sequenceLoop.Count > 0 ? Duration / sequenceLoop.Count : Duration;
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
            // if sequence is repeated, its duration should be multiplied by number of iterations:
            var sequenceLoops = (Leader.Strategy as RepeatStrategy)?.Count ?? 1;
            SetDuration(repeatStrategy.Interval * repeatStrategy.Count * sequenceLoops);
        }
    }
}
