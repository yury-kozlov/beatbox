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
        // Both LoopEnds with different types: type takes priority over iteration paths
        if (a is LoopEnd aLoopEnd && b is LoopEnd bLoopEnd && aLoopEnd.IsSequenceLoop != bLoopEnd.IsSequenceLoop)
        {
            return aLoopEnd.IsSequenceLoop ? 1 : -1; // sequence loop end goes after regular loop end
        }

        // If sequence is repeated in loop and some sounds are overlapping, compare them by iteration path.
        // Hierarchical path (e.g. "2.1") sorts after its parent level ("2"), placing followers after their SequenceStart:
        if (a.Iteration.HasValue() && b.Iteration.HasValue() && a.Iteration != b.Iteration)
        {
            return CompareIterations(a.Iteration, b.Iteration);
        }

        // Trimmed marker goes before sequence-end
        if (a is SequenceEndTrimmed && b is SequenceEnd)
        {
            return -1;
        }

        if (a is LoopEnd aSeqLoop)
        {
            if (aSeqLoop.IsSequenceLoop)
            {
                if (b is LoopEnd) { return 0; } // let stable sort decide between loop ends
                if (b is SequenceEnd bSeqEnd)
                {
                    if (aSeqLoop.Sequence == bSeqEnd.Sequence) { return -1; }                          // same-sequence loop goes before its own seq-end
                    if (aSeqLoop.Sequence?.Sequences.Contains(bSeqEnd.Sequence) == true) { return 1; } // outer loop goes after inner seq-end
                    return 0; // unrelated: stable sort
                }
                return 1; // sequence loop end goes after seq-start, regular sounds, trimmed, metronome
            }
            return -1; // regular loop-end goes before any other sound
        }

        if (a is SequenceEnd aSeqEnd)
        {
            if (b is LoopEnd bLoopEndSE)
            {
                if (!bLoopEndSE.IsSequenceLoop) { return 1; }                                                  // seq-end after regular loop-end
                if (aSeqEnd.Sequence == bLoopEndSE.Sequence) { return 1; }                                     // same-sequence seq-end after its own loop
                if (bLoopEndSE.Sequence?.Sequences.Contains(aSeqEnd.Sequence) == true) { return -1; }          // inner seq-end before outer loop
                return 0; // unrelated: stable sort
            }
            if (b is Metronome) { return 1; }
            if (b is SequenceEnd) { return 0; } // let stable sort decide by insertion order (outer sequence end is generated last, so it goes last)
            if (b is SequenceEndTrimmed) { return 1; } // sequence-end goes after trimmed marker
            // put "sequence-end" after "sequence-start" and regular sounds of same sequence
            // different sequences: rely on insertion order — outer container end is generated last
            return a.Sequence == b.Sequence ? 1 : 0;
        }

        if (a is SequenceStart)
        {
            if (b is SequenceStart) { return 0; } // leave as is
            if (b is LoopEnd bLoopEndSS)
            {
                // seq-start goes before outer sequence loop end; seq-start goes after regular loop-end
                return bLoopEndSS.IsSequenceLoop ? -1 : 1;
            }
            // same sequence: seq-start goes before seq-end and regular sounds
            // different sequences: rely on insertion order
            return a.Sequence == b.Sequence ? -1 : 0;
        }

        if (a is Metronome)
        {
            if (b is LoopEnd bLoopEndM)
            {
                // metronome before outer sequence loop end; metronome after regular loop-end
                return bLoopEndM.IsSequenceLoop ? -1 : 1;
            }
            if (b is SequenceStart) { return 1; }
            return -1; // put "metronome" before regular sounds
        }

        // a is a regular sound:
        if (b is LoopEnd bLoopEndRegular)
        {
            if (bLoopEndRegular.IsSequenceLoop) { return -1; } // regular sounds before outer sequence loop end
            return a.Sequence == b.Sequence ? 1 : -1; // put regular sounds after regular loop-end only if they belong to the same sequence
        }
        if (b is SequenceStart or Metronome)
        {
            return a.Sequence == b.Sequence ? 1 : -1; // put regular sounds after "sequence-start", "metronome" only if they belong to the same sequence
        }
        if (b is SequenceEnd)
        {
            // put regular sounds before "sequence-end" of same sequence
            // different sequences: rely on insertion order
            return a.Sequence == b.Sequence ? -1 : 0;
        }
        return 0;
    }
}
