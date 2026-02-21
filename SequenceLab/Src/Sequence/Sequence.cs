namespace Beater;

public class Sequence : List<Sound>
{
    public void SortByTimestamp()
    {
        Sort((a, b) =>
        {
            var order = a.Timestamp.CompareTo(b.Timestamp);
            if (order == 0)
            {
                if (a is LoopEnd)
                {
                    return -1; // put "end-of-loop" sound before any other sound
                }
                if (a is SequenceStart)
                {
                    if (b is SequenceStart)
                    {
                        return 0; // leave as is
                    }
                    if (b is LoopEnd)
                    {
                        // put "sequence-start" sounds after "end-of-loop"
                        return 1;
                    }
                    // put "sequence-start" after reqular sounds only if they belong to another sequence
                    return a.Sequence != b.Sequence ? 1 : -1;
                }
                if (a is Metronome)
                {
                    if (b is SequenceStart or LoopEnd)
                    {
                        return 1; // put "metronome" after "sequence-start"
                    }
                    return -1; // put "metronome" before regular sounds
                }
                if (b is LoopEnd or SequenceStart or Metronome)
                {
                    // put regular sounds after "end-of-loop", "sequence-start", "metronome" only if they belong to the same sequence
                    return a.Sequence == b.Sequence ? 1 : -1;
                }
            }
            return order;
        });
    }

    public Sequence Mix(Sequence followers)
    {
        AddRange(followers);
        SortByTimestamp();

        return this;
    }
}
