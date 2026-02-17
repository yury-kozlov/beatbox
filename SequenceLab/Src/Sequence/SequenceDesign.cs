
namespace Beater;

public class SequenceDesign
{
    public SequenceDesign(string name)
    {
        Name = name;
        Leader = SequenceStart = new SequenceStart(name);
    }

    /// <summary>
    /// SequenceStart is a "meta" sound that allows to abstract current sequence to one sound.
    /// </summary>
    public SequenceStart SequenceStart;

    /// <summary>
    /// Leader of a sequence design is always an instance of <see cref="nameof(SequenceStart)"/> type.
    /// Actual leader will be the first follower of <see cref="SequenceStart"/>.
    /// </summary>
    public Sound Leader { get; set => field = InitLeader(value); }

    public AbstractStrategy Strategy
    {
        get => SequenceStart.Strategy;
        set { SequenceStart.Strategy = value; TrySetDuration(value); }
    }

    private void TrySetDuration(AbstractStrategy strategy)
    {
        if (strategy is RepeatStrategy repeat)
        {
            var sequenceDuration = Duration;
            if (repeat.Interval == 0 && sequenceDuration > 0)
            {
                // automatically set sequence loop interval as the original duration of the sequence
                repeat.Interval = sequenceDuration;

            }
            if (repeat.Count > 0)
            {
                // automatically update sequence duration as full duration of the repeat:
                Duration = repeat.Interval * repeat.Count;
            }
        }
    }

    /// <summary>
    /// Wrap actual leader with "SequenceStart" sound.
    /// </summary>
    private Sound InitLeader(Sound leader)
    {
        if (leader == SequenceStart)
        {
            // this is an init call (from ctor)
            return SequenceStart;
        }
        // "real" leader is assigned
        return SequenceStart.WithFollower(leader).WithSequenceIfMissing(this);
    }

    /// <summary>
    /// In milliseconds (represents full loop of a sequence including ending space).
    /// Duration of sequence should be known ahead for each predefined sequence if it's going to be played in loop
    /// (otherwise we will not be able to place next iteration at correct timing).
    /// </summary>
    public int Duration; /// TODO: should we add <see cref="LoopEnd"/> sound at the end of each sequence?

    /// <summary>
    /// Name of the current sequence.
    /// Used for logging purposes.
    /// </summary>
    public string Name;

    public static SequenceDesign? FromJson(string json) => Serialization.FromJson<SequenceDesign>(json);

    /// <summary>
    /// Appends new sequence to the end of the current one.
    /// NOTE: duration of the current sequence is increased after adding the new one.
    /// </summary>
    internal SequenceDesign Append(SequenceDesign next)
    {
        if (Leader.Followers.Count == 0)
        {
            // this is the first sequence
            Leader.Followers = [next.SequenceStart];
            Duration = next.Duration;
            return this;
        }

        Leader.Followers.Add(next.SequenceStart with
        {
            DelayAfterLeader = Duration, // wait for the original sequence to finish, only then start playing the next one
        });
        Duration += next.Duration;
        return this;
    }

    public override string ToString() => Name ?? base.ToString() ?? "";
}
