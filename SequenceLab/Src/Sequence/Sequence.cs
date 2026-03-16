namespace Beater;

public class Sequence : List<Sound>
{
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
