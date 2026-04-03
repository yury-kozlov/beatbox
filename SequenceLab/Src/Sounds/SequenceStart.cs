namespace Beater;

public record SequenceStart : NoSound
{
    public SequenceStart(string sequenceName)
    {
        FriendlyName = $"sequence-start-{sequenceName}";

        /// follow last sound of the previous sequence (usually <see cref="SequenceEnd"/>):
        Strategy = new FollowPreviousSoundStrategy();
    }

    /// <summary>
    /// NOTE: We don't store SequenceEnd as part of SequenceDesign 
    /// (but rather search for it dynamically by sequence-start) because when generating actual sequence
    /// each sound is cloned with updated timestamp. In repeated sequences it will be impossible to always work 
    /// on the same instance of a SequenceEnd sound shared between different iterations.
    /// </summary>
    public SequenceEnd GetSequenceEnd() => (SequenceEnd)Followers.Last(s => s is SequenceEnd);

    public override string? ToString() => Format(FriendlyName);
}