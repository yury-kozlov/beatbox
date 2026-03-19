namespace Beater;

public static class SequenceSoundSorter
{
    /// <summary>
    /// Stable sort preserving generation order for sounds that compare as equal.
    /// Required when mixing simultaneous sequences so that sounds from earlier-generated sub-sequences
    /// stay before sounds from later-generated sub-sequences at the same timestamp.
    /// </summary>
    public static Sequence SortByTimestamp(List<Sound> sequence)
    {
        var sorted = new Sequence(sequence
            // store aside each sound with the original "index" (indicating the initial order in which sounds were added to their sequences)
            .Select((sound, index) => (sound, index))
            .OrderBy(x => x.sound.Timestamp)
            .ThenBy(x => x, Comparer<(Sound sound, int index)>.Create((x, y) =>
            {
                var order = Compare(x.sound, y.sound);
                return order != 0 ? order : x.index.CompareTo(y.index); // stable: preserve original index on tie
            }))
            .Select(x => x.sound));
        return sorted;
    }

    /// <summary>
    /// Compares two hierarchical iteration paths segment by segment as integers.
    /// A shorter prefix sorts before a longer one with the same prefix (e.g. "2" before "2.1"),
    /// so a SequenceStart ("2") always precedes its own followers ("2.1", "2.2").
    /// </summary>
    private static int CompareIterations(string a, string b)
    {
        var aParts = a.Split('.');
        var bParts = b.Split('.');
        for (int i = 0; i < Math.Min(aParts.Length, bParts.Length); i++)
        {
            var cmp = int.Parse(aParts[i]).CompareTo(int.Parse(bParts[i]));
            if (cmp != 0)
            {
                return cmp;
            }
        }
        return aParts.Length.CompareTo(bParts.Length);
    }

    private static int Compare(Sound a, Sound b)
    {
        var order = a.Timestamp.CompareTo(b.Timestamp);
        if (order == 0)
        {
            if (a is LoopEnd aLoopEnd && b is LoopEnd bLoopEnd && aLoopEnd.IsSequenceLoop != bLoopEnd.IsSequenceLoop)
            {
                return aLoopEnd.IsSequenceLoop ? 1 : -1; // sequence loop end goes after regular loop end
            }

            if (a.Iteration.HasValue() && b.Iteration.HasValue() && a.Iteration != b.Iteration)
            {
                // if sequence is repeated in loop and some sounds are overlapping, compare them by iteration path.
                // hierarchical path (e.g. "2.1") sorts after its parent level ("2"), placing followers after their SequenceStart:
                return CompareIterations(a.Iteration, b.Iteration);
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
                if (b is SequenceEnd)
                {
                    return 0; // let stable sort decide by insertion order (outer sequence end is generated last, so it goes last)
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
                // put "sequence-start" before regular sounds only if they belong to the same sequence
                return a.Sequence == b.Sequence ? -1 : 0;
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
    }
}
