namespace Beater;

public static class SequenceSoundSorter
{
    public static void SortByTimestamp(Sequence sequence)
    {
        sequence.Sort((a, b) =>
        {
            var order = a.Timestamp.CompareTo(b.Timestamp);
            if (order == 0)
            {
                if (a is LoopEnd)
                {
                    return -1; // put "end-of-loop" sound before any other sound
                }
                if (a is SequenceEnd)
                {
                    if (b is LoopEnd or Metronome)
                    {
                        return 1; // put "sequence-end" after "end-of-loop" and "metronome"
                    }
                    if (b is SequenceStart)
                    {
                        return -1; // put "sequence-end" before "sequence-start"
                    }
                }
                if (a is SequenceStart)
                {
                    if (b is SequenceStart)
                    {
                        return 0; // leave as is
                    }
                    if (b is LoopEnd or SequenceEnd)
                    {
                        // put "sequence-start" sounds after "end-of-loop" and "sequence-end"
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
                if (b is LoopEnd or SequenceEnd or SequenceStart or Metronome)
                {
                    // put regular sounds after "end-of-loop", "sequence-end", "sequence-start", "metronome" only if they belong to the same sequence
                    return a.Sequence == b.Sequence ? 1 : -1;
                }
            }
            return order;
        });
    }
}
