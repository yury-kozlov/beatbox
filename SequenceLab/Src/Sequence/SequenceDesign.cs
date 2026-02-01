
namespace Beater;

public class SequenceDesign
{
    private int JoinsCounter;

    public SequenceDesign(string name)
    {
        Name = name;
        Leader = SequenceStart = new SequenceStart(name);
    }

    public SequenceStart SequenceStart;

    public Sound Leader { get; set => field = InitLeader(value); }

    private Sound InitLeader(Sound leader)
    {
        if (leader == SequenceStart)
        {
            return leader; // already initialized
        }
        return SequenceStart.WithFollower(leader).WithSequenceIfMissing(this);
    }

    /// <summary>
    /// In milliseconds (represents full loop of a sequence including ending space).
    /// Duration of sequence should be known ahead for each predefined sequence if it's going to be played in loop
    /// (otherwise we will not be able to place next iteration at correct timing).
    /// </summary>
    public int Duration;

    /// <summary>
    /// Name of the current sequence.
    /// Used for logging purposes.
    /// </summary>
    public string Name;

    /// <summary>
    /// Last sequence that was appended to the current one.
    /// </summary>
    public SequenceDesign? LastAppendedSequence;

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
            Leader.Followers = [next.Leader];
            Duration = next.Duration;
            LastAppendedSequence = next;
            return this;
        }

        JoinsCounter++;
        Leader.Followers.Add(new Joint()
        {
            JoinsCounter = JoinsCounter,
            DelayAfterLeader = Duration, // wait for the original sequence to finish, only then start playing the next one
            Followers = [next.Leader],
            PreviousSequence = LastAppendedSequence!,
            NextSequence = next,
        });
        Duration += next.Duration;
        LastAppendedSequence = next;
        return this;
    }

    public override string ToString() => Name ?? base.ToString() ?? "";
}
