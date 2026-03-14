namespace Beater;

public class Sequence : List<Sound>
{
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
        SequenceSoundSorter.SortByTimestamp(this);

        return this;
    }
}
