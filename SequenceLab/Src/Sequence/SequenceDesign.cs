
namespace Beater;

public class SequenceDesign
{
    public SequenceDesign(string name)
    {
        Name = name;
        Leader = new SequenceStart(name) { Sequence = this };
        SequenceEnd = new SequenceEnd(this);
    }

    /// <summary>
    /// Leader of a sequence design is always an instance of <see cref="nameof(SequenceStart)"/> type.
    /// Actual leader will be the first follower of <see cref="SequenceStart"/>.
    /// </summary>
    public Sound Leader { get; set => field = InitLeader(value); }

    public AbstractStrategy Strategy
    {
        get => Leader.Strategy;
        set { Leader.Strategy = value; TrySetDuration(value); }
    }

    private void TrySetDuration(AbstractStrategy strategy)
    {
        if (strategy is RepeatStrategy repeatStrategy)
        {
            var sequenceDuration = Duration;
            if (repeatStrategy.Interval == 0 && sequenceDuration > 0)
            {
                // automatically set sequence loop interval as the original duration of the sequence
                repeatStrategy.Interval = sequenceDuration;
            }
            if (repeatStrategy.Count > 0)
            {
                // automatically update sequence duration as full duration of the loop:
                Duration = repeatStrategy.Interval * repeatStrategy.Count;
            }
        }
    }

    /// <summary>
    /// Wrap actual leader with "SequenceStart" sound.
    /// </summary>
    private Sound InitLeader(Sound leader)
    {
        if (leader is SequenceStart)
        {
            // this is an init call (from ctor)
            return leader;
        }
        // "real" leader is assigned
        return Leader
            .WithFollower(leader.WithSequenceIfMissing(this))
            .WithFollower(SequenceEnd);
    }

    /// <summary>
    /// Assigns delay to the underlying strategy shifting the whole sequence.
    /// NOTE: if sound strategy property is initialized inside the same block after delay is set, the current delay value will be ignored.
    /// </summary>
    public int DelayAfterLeader { set { Strategy.DelayAfterLeader = value; } }

    /// <summary>
    /// In milliseconds (represents full loop of a sequence including ending space).
    /// Duration of sequence should be known ahead for each predefined sequence if it's going to be played in loop
    /// (otherwise we will not be able to place next iteration at correct timing). This is especially important for sequences 
    /// without loops since we will not be able to automatically calculate duration of such sequences based on their strategy.
    /// Duration is also required when appending one sequence to another (otherwise we will not know at which point to start the second sequence).
    /// - May be initialized at design time or based on RepeatStrategy parameters.
    /// </summary>
    public int Duration
    {
        get;
        set
        {
            field = value; SequenceEnd.InitStrategy(); // recalculate sequence end strategy because it depends on duration
        }
    }

    /// <summary>
    /// Calculcated automatically during generation of sequence timestamps (each sound from SequenceStart until SequenceEnd will increment auto duration).
    /// </summary>
    public int AutoDuration;

    /// <summary>
    /// Name of the current sequence.
    /// Used for logging purposes.
    /// </summary>
    public string Name;

    private SequenceEnd SequenceEnd;

    public static SequenceDesign? FromJson(string json) => Serialization.FromJson<SequenceDesign>(json);

    /// <summary>
    /// Appends new sequence to the end of the current one.
    /// NOTE: duration of the current sequence is increased after adding the new one.
    /// </summary>
    internal SequenceDesign Append(SequenceDesign next)
    {
        Leader.Followers.Add(next.Leader);
        Duration += next.Duration;

        // move SequenceEnd to tail:
        Leader.Followers.Remove(SequenceEnd);
        Leader.WithFollower(SequenceEnd);

        return this;
    }

    public override string ToString() => Name ?? base.ToString() ?? "";
}
