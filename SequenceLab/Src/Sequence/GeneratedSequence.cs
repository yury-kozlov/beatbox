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

    /// <summary>
    /// Example returned value: "k, 1200 k, 1200 k, 600 k, 600 k".
    /// </summary>
    public string DebuggerDisplay => SequenceDebuggerDisplay.Get([.. this.Select(s => s.SoundDesign)]);
}