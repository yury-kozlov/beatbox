namespace Beater;

/// <summary>
/// Joins two sequences.
/// The same as NoSound, used for better debugging experience.
/// </summary>
public record Joint : NoSound
{
    /// <summary>
    /// Indicates number of joined sequences.
    /// Initialized when a sequence is appended to another sequence.
    /// </summary>
    public required int JoinsCounter;

    /// <summary>
    /// The original sequence to which a new one will be appended.
    /// </summary>
    public required SequenceDesign PreviousSequence;

    /// <summary>
    /// Next sequence appended to the original one.
    /// </summary>
    public required SequenceDesign NextSequence;

    public string FriendlyName => $"joint #{JoinsCounter} {PreviousSequence.Name}__{NextSequence.Name}";

    public override string? ToString() => FriendlyName;
}
