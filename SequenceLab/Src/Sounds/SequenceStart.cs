namespace Beater;

public record SequenceStart : NoSound
{
    public SequenceStart(string sequenceName)
    {
        FriendlyName = $"sequence-start-{sequenceName}";

        /// follow last sound of the previous sequence (usually <see cref="SequenceEnd"/>):
        Strategy = new FollowPreviousSoundStrategy();
    }

    public SequenceEnd GetSequenceEnd() => (SequenceEnd)Followers.Last(s => s is SequenceEnd);

    public override string? ToString() => Format(FriendlyName);
}