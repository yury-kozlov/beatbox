
namespace Beater;

public class SequenceDesign
{
    private SequenceDesignInitializer _initializer;

    public SequenceDesign(string name)
    {
        Name = name;
        SequenceEnd = new SequenceEnd(this);
        _initializer = new SequenceDesignInitializer(this);
        Leader = new SequenceStart(name) { Sequence = this };
    }

    /// <summary>
    /// Leader of a sequence design is always an instance of <see cref="SequenceStart"/> type.
    /// Here we make sure that when setting leader of a sequence, the first sound will always remain sequence-start.
    /// Actual leader will be the first follower of <see cref="SequenceStart"/>.
    /// </summary>
    public Sound Leader
    {
        get;
        set => field = _initializer.SetLeader(value);
    }

    /// <summary>
    /// First sound simplifies access to the first actual sound of the sequence, because Leader of a sequence is always a SequenceStart sound.
    /// </summary>
    public Sound? FirstSound => Leader.Followers.FirstOrDefault();

    public AbstractStrategy Strategy
    {
        get => Leader.Strategy;
        set => _initializer.SetStrategy(value);
    }

    /// <summary>
    /// Assigns delay to the underlying strategy shifting the whole sequence.
    /// NOTE: if sound strategy property is initialized inside the same block after delay is set, the current delay value will be ignored.
    /// </summary>
    public int DelayAfterLeader { set => _initializer.SetDelayAfterLeader(value); }

    /// <summary>
    /// Full loop of a sequence (in milliseconds) including ending space and all iterations.
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
            _initializer.OnDurationSet();
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

    /// <summary>
    /// WARNING: do not use this during sequence generation — each sound is cloned with an updated timestamp,
    /// so in repeated sequences this instance will not correspond to the current iteration's SequenceEnd.
    /// Use <see cref="SequenceStart.GetSequenceEnd"/> instead to locate the correct instance dynamically.
    /// </summary>
    public SequenceEnd SequenceEnd { get; private set; }

    public bool IsEmpty => Sequences.Count == 0;

    private List<SequenceDesign> Sequences = [];

    public static SequenceDesign? FromJson(string json) => Serialization.FromJson<SequenceDesign>(json);

    /// <summary>
    /// Appends new sequence to the end of the current one (to the followers of SequenceEnd of the last added sequence).
    /// NOTE:
    ///  - Duration of the current sequence is increased after adding the new one.
    ///    Appending doesn't mean the new sequence will be limited to the duration of the base,
    ///    but on the opposite: base duration will be increased to accomodate the new sequence.
    ///  - If base sequence is repeated and we append to it - it means we will append to each iteration of this sequence
    ///    also increasing total duration of the sequence
    /// </summary>
    internal SequenceDesign Append(SequenceDesign next)
    {
        _initializer.UpdateDurationOnAppend(Strategy, next.Duration);

        Sequences.Add(next);

        if (Sequences.Count == 1 || next.Leader.Strategy is FollowPreviousSoundStrategy)
        {
            // if the added sequence is the first one or it has default strategy
            // just add it as the next follower (it will be played after all previous sequences)
            Leader.WithFollower(next.Leader);
            Leader.MoveFollowerToTheEnd(SequenceEnd);
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
        Leader.MoveFollowerToTheEnd(SequenceEnd);
        return this;
    }

    /// <summary>
    /// Combines current sequence with another to allow playing them in parallel (instead of playing one after another).
    /// NOTE: duration of the current sequence is calculated as maximum between the two.
    /// </summary>
    internal SequenceDesign Combine(SequenceDesign next)
    {
        if (IsEmpty)
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
        Leader.MoveFollowerToTheEnd(SequenceEnd);
        return this;
    }

    public override string ToString() => Name ?? base.ToString() ?? "";
}
