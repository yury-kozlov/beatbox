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
                if (a is Joint)
                {
                    return b is LoopEnd ? 1 : -1; // put "joint" sound before other sounds, but after "end-of-loop"
                }
                if (a is SequenceStart)
                {
                    if (b is SequenceStart)
                    {
                        return 0; // leave as is
                    }
                    if (b is LoopEnd or Joint)
                    {
                        // put "sequence-start" sounds after "end-of-loop" and "joint"
                        return 1;
                    }
                    // put "sequence-start" after reqular sounds only if they belong to another sequence
                    return a.Sequence != b.Sequence ? 1 : -1;
                }
                if (a is Metronome)
                {
                    if (b is SequenceStart or Joint or LoopEnd)
                    {
                        return 1; // put "metronome" after "sequence-start" and "joint"
                    }
                    return -1; // put "metronome" before regular sounds
                }
                if (b is LoopEnd or SequenceStart or Joint or Metronome)
                {
                    // put regular sounds after "end-of-loop", "sequence-start", "joint", "metronome" only if they belong to the same sequence
                    return a.Sequence == b.Sequence ? 1 : -1;
                }
            }
            return order;
        });
    }
}
