namespace Beater;

public class GeneratedSequence : Sequence
{
    public GeneratedSequence()
    { }

    public GeneratedSequence(IEnumerable<Sound> source) : base(source)
    { }
}

public class Sequence : List<Sound>
{
    /// <summary>
    /// Number of sounds in the sequence after it was initialized (assigned as followers to a leader).
    /// </summary>
    public int InitialLength;

    public Sequence()
    { }

    public Sequence(IEnumerable<Sound> source) : base(source)
    { }

    public void SetLeader(Sound leader)
    {
        foreach (var sound in this)
        {
            sound.Leader = leader;
        }
    }

    public GeneratedSequence Mix(GeneratedSequence followers)
    {
        AddRange(followers);
        return SequenceSoundSorter.SortByTimestamp(this);
    }

    /// <summary>
    /// Example returned value: "k, 1200 k, 1200 k, 600 k, 600 k".
    /// </summary>
    public string DebuggerDisplay => SequenceDebuggerDisplay.Get(this);
}
