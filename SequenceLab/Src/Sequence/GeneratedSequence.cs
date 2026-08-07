namespace Beater;

/// <summary>
/// Final output of sequence generation: one <see cref="GeneratedSound"/> per sound actually produced, ready for transport.
/// </summary>
public class GeneratedSequence : List<GeneratedSound>
{
    public GeneratedSequence()
    { }

    public GeneratedSequence(params IEnumerable<GeneratedSound> source) : base(source)
    { }

    public GeneratedSequence(params IEnumerable<SoundDesign> source) : base(source.Select(s => s.Generated))
    { }

    public GeneratedSequence Mix(GeneratedSequence followers)
    {
        AddRange(followers);
        return SequenceSoundSorter.SortByTimestamp(this);
    }
}