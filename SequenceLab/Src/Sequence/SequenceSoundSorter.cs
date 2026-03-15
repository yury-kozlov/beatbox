namespace Beater;

public static class SequenceSoundSorter
{
    public static void SortByTimestamp(List<Sound> sequence)
    {
        sequence.Sort((a, b) =>
        {
            var order = a.Timestamp.CompareTo(b.Timestamp);
            if (order == 0)
            {
                if (a.Iteration != 0 && b.Iteration != 0 && a.Iteration != b.Iteration)
                {
                    // if sequence is repeated in loop and some sounds are overlapping, compare them by iteration number
                    return a.Iteration.CompareTo(b.Iteration);
                }

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
                        // same sequence: "sequence-end" goes after "sequence-start"
                        // different sequences: "sequence-end" goes before "sequence-start" of next sequence
                        return a.Sequence == b.Sequence ? 1 : -1;
                    }
                    // put "sequence-end" after regular sounds of same sequence
                    return a.Sequence == b.Sequence ? 1 : -1;
                }
                if (a is SequenceStart)
                {
                    if (b is SequenceStart)
                    {
                        return 0; // leave as is
                    }
                    if (b is LoopEnd)
                    {
                        return 1; // put "sequence-start" after "end-of-loop"
                    }
                    if (b is SequenceEnd)
                    {
                        // same sequence: "sequence-start" goes before "sequence-end"
                        // different sequences: "sequence-start" goes after "sequence-end" of previous sequence
                        return a.Sequence == b.Sequence ? -1 : 1;
                    }
                    // put "sequence-start" after regular sounds only if they belong to another sequence
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
                if (b is SequenceEnd)
                {
                    // put regular sounds before "sequence-end" of same sequence
                    return a.Sequence == b.Sequence ? -1 : 1;
                }
            }
            return order;
        });
    }
}
