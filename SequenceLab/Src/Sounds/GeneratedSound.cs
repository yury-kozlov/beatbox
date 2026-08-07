namespace Beater;

/// <summary>
/// Result of generating a single sound instance: only data computed during sequence generation,
/// ready for further sending via transport.
/// </summary>
public record GeneratedSound
{
    public GeneratedSound(SoundDesign soundDesign)
    {
        SoundDesign = soundDesign;
        Name = soundDesign.Name;
    }

    public string? Name;

    /// <summary>
    /// In the final sequence, represents absolute position of the sound from the beginning of the whole sequence.
    /// If the sound is an X iteration inside a loop, position will still be calculated from the very beginning (including all previous iterations).
    /// NOTE: during sequence generation, this value is calculated relatively to the current leader and then shifted according to leader's position (becomes absolute).
    /// </summary>
    public int Timestamp;

    public bool IsSilenced;

    /// <summary>
    /// Hierarchical iteration path (starting from 1) of the current sound in nested loops, e.g. "1", "2.3", "1.2.1", etc.
    /// Specified in the following format: "{OuterLoopIteration}.{...}.{InnerLoopIteration}".
    /// NOTE: if current sound is not part of any loop, Iteration will be null.
    /// This field is used for sorting sequence sounds.
    /// </summary>
    public string? Iteration;

    /// <summary>
    /// Generation-time comment (e.g. iteration marker), used for logging.
    /// </summary>
    public string? Comment;

    /// <summary>
    /// The SoundDesign this instance was generated from (its data and structure — Name, Tags, Followers, Strategy, etc.).
    /// </summary>
    public SoundDesign SoundDesign;
}
