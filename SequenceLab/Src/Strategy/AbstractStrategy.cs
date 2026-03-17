namespace Beater;

public abstract class AbstractStrategy
{
    /// <summary>
    /// Time to wait after the leader sound before starting to play the first follower or the whole loop (in case of repeated sounds).
    /// </summary>
    public int DelayAfterLeader;

    /// <summary>
    /// Total number of times this strategy was checked and called (including skipped calls) starting from 1.
    /// NOTE: this counter is related to the strategy, not to the sound itself
    /// (e.g. for repeat strategy it will count number of loops, not total number of sounds in all loops)
    /// </summary>
    public int CheckedTimes;

    /// <summary>
    /// Number of times when this strategy was actually played (not skipped).
    /// </summary>
    public int CalledTimes;

    /// <summary>
    /// The sound will be played every X-th time:
    ///   1 - every time without skipping
    ///   2 - at even times (2,4,6...), while all odd times (1,3,5...) will be skipped.
    /// NOTE: this counter is related to the strategy, not to the sound itself. 
    ///       Meaning if sound has "repeat strategy" - the whole loop will be played every X times (not every X sound inside the loop).
    /// </summary>
    public int PlayEveryX = 1; // play every time by default

    /// <summary>
    /// The sound will be played every X-th time within repeated range, for example:
    ///  3/4 - each 3rd time will be played out of every 4.
    /// NOTE: this counter is related to the strategy, not to the sound itself.
    /// </summary>
    public string? PlayEveryXOutOf;

    /// <summary>
    /// Will replace current sound with an empty sound preserving the same followers, for example:
    ///  3/4 - each 3rd time out of every 4 will be silenced.
    /// NOTE: this counter is related to the strategy, not to the sound itself.
    /// </summary>
    public string? SilenceEveryXOutOf;

    public abstract Sequence ApplyStrategy(Sound leader);
}

public static class StrategyExtensions
{
    public static TTarget CopyBasePropertiesFrom<TSource, TTarget>(this TTarget target, TSource source)
        where TSource : AbstractStrategy where TTarget : AbstractStrategy
    {
        target.DelayAfterLeader = source.DelayAfterLeader;
        target.PlayEveryX = source.PlayEveryX;
        target.PlayEveryXOutOf = source.PlayEveryXOutOf;
        target.SilenceEveryXOutOf = source.SilenceEveryXOutOf;

        return target;
    }
}