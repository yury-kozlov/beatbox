namespace Beater;

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

    public Sequence Mix(Sequence followers)
    {
        AddRange(followers);
        return SequenceSoundSorter.SortByTimestamp(this);
    }
}
