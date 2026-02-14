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
                    // put "sequence-start" sounds before regular sounds, but after "end-of-loop"
                    return b is LoopEnd ? 1 : -1;
                }
                if (b is LoopEnd or SequenceStart)
                {
                    return 1; // put regular sounds after "end-of-loop" and "sequence-start"
                }
            }
            return order;
        });
    }
}
