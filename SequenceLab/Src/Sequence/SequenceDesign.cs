
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
    /// Leader of a sequence design is always an instance of <see cref="SequenceStart"/> type.
    /// Here we make sure that when setting leader of a sequence, the first sound will always remain sequence-start.
    /// Actual leader will be the first follower of <see cref="SequenceStart"/>.
    /// </summary>
    public Sound Leader
    {
        get;
        set
        {
            if (value is SequenceStart)
            {
                // if it's initial call from ctor the value is already SequenceStart, no need to enforce it:
                field = value;
                return;
            }
            field = EnforceSequenceStartLeader(value);
            UpdateLoopDuration(value.Strategy); // if leader is a loop, we can use it's interval to calculate total sequence duration
        }
    }

    /// <summary>
    /// First sound simplifies access to the first actual sound of the sequence, because Leader of a sequence is always a SequenceStart sound.
    /// </summary>
    public Sound? FirstSound => Leader.Followers.FirstOrDefault();

    public AbstractStrategy Strategy
    {
        get => Leader.Strategy;
        set
        {
            Leader.Strategy = value;
            UpdateLoopInterval();
            UpdateLoopDuration(value);
        }
    }

    /// <summary>
    /// Initializes interval of sequence loop when sequence strategy was changed or duration of the sequence was changed.
    /// NOTE: this is a shortcut path to omit specifying explicit Interval in repeated sequences (e.g. <see cref="PrimitiveSequences.Trapezoid{TSound}"/>)
    /// when duration of the underlying sequence is known and we just need to calculate total repetition time:
    /// in this case sequence loop interval will be automatically set as duration of the original sequence
    /// </summary>
    private void UpdateLoopInterval()
    {
        if (Strategy is RepeatStrategy sequenceLoop)
        {
            if (Duration > 0)
            {
                // NOTE: sequence Duration includes all iterations of a loop
                sequenceLoop.Interval = sequenceLoop.Count > 0 ? Duration / sequenceLoop.Count : Duration;
            }
        }
    }

    /// <summary>
    /// Updates duration (as a result of strategy change, or leader initialization).
    /// NOTE: change in sequence duration also triggers re-calculation of loop interval.
    /// </summary>
    private void UpdateLoopDuration(AbstractStrategy leaderStrategy)
    {
        if (leaderStrategy is RepeatStrategy repeatStrategy)
        {
            if (repeatStrategy.Count > 0 && repeatStrategy.Interval > 0)
            {
                // if sequence is repeated, it's duration should be multipled by number of iterations:
                var sequenceLoops = (Strategy as RepeatStrategy)?.Count ?? 1;
                Duration = repeatStrategy.Interval * repeatStrategy.Count * sequenceLoops;
            }
        }
    }

    /// <summary>
    /// Force actual leader to be a follower of "SequenceStart" sound.
    /// </summary>
    private Sound EnforceSequenceStartLeader(Sound leader)
    {
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
    /// In milliseconds (represents full loop of a sequence including ending space and all iterations).
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
            field = value;
            SequenceEnd.InitStrategy(); // recalculate sequence end strategy because it depends on duration
            UpdateLoopInterval();
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

    private List<SequenceDesign> Sequences = [];

    public static SequenceDesign? FromJson(string json) => Serialization.FromJson<SequenceDesign>(json);

    /// <summary>
    /// Appends new sequence to the end of the current one.
    /// NOTE: duration of the current sequence is increased after adding the new one.
    /// </summary>
    internal SequenceDesign Append(SequenceDesign next)
    {
        Duration += next.Duration;
        UpdateLoopDuration(Strategy);

        Sequences.Add(next);

        if (Sequences.Count == 1 || next.Leader.Strategy is FollowPreviousSoundStrategy)
        {
            // if the added sequence is the first one or it has default strategy
            // just add it as the next follower (it will be played after all previous sequences)
            Leader.WithFollower(next.Leader);
            UpdateSequenceEnd();
            return this;
        }

        // append to the end of the previous sequence (to make sure the added sequence is played after it and not in parallel)
        if (Sequences[^2].SequenceEnd is SequenceEnd previousEnd)
        {
            previousEnd.WithFollower(next.Leader);
            return this;
        }

        // this is not supposed to happen:
        Console.WriteLine($"Appended sequence {next.Name} will be played in parallel instead if sequentially because sequence end of the previous sequence was not found");
        Leader.WithFollower(next.Leader);
        UpdateSequenceEnd();
        return this;
    }

    /// <summary>
    /// Combines current sequence with another to allow playing them in parallel (instead of playing one after another).
    /// NOTE: duration of the current sequence is calculated as maximum between the two.
    /// </summary>
    internal SequenceDesign Combine(SequenceDesign next)
    {
        if (Sequences.Count == 0)
        {
            Append(next);
            return this;
        }

        Duration = Math.Max(Duration, next.Duration);
        Sequences.Add(next);

        if (next.Leader.Strategy is FollowPreviousSoundStrategy followStrategy)
        {
            // if the added sequence has default strategy, change it to allow play in parallel
            next.Leader.Strategy = followStrategy.ToPlayOnceStrategy();
        }

        Leader.WithFollower(next.Leader);
        UpdateSequenceEnd();
        return this;
    }

    private void UpdateSequenceEnd()
    {
        // make sure SequenceEnd is the last follower:
        Leader.Followers.Remove(SequenceEnd);
        Leader.WithFollower(SequenceEnd);
    }

    public override string ToString() => Name ?? base.ToString() ?? "";
}
