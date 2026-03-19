using Beater;

namespace Tests;

public class SequenceDesignDurationTests
{
    [Fact]
    public void Duration_StrategyFirst_ThenLeader_BothRepeat_IsCalculatedCorrectly()
    {
        // arrange: outer strategy is initialized before inner leader strategy
        var sequence = new SequenceDesign("test")
        {
            Strategy = new RepeatStrategy { Count = 2 },
            Leader = new Kick { Strategy = new RepeatStrategy { Interval = 100, Count = 3 } },
        };

        // act + assert: Duration = inner_interval * inner_count * outer_count = 100 * 3 * 2 = 600
        sequence.Duration.Should().Be(600);
    }

    [Fact]
    public void Duration_LeaderFirst_ThenStrategy_BothRepeat_IsCalculatedCorrectly()
    {
        // arrange: inner leader strategy is initialized before outer strategy
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Interval = 100, Count = 3 } },
            Strategy = new RepeatStrategy { Count = 2 },
        };

        // act + assert: same result regardless of initialization order
        sequence.Duration.Should().Be(600);
    }

    [Fact]
    public void Duration_LeaderOnlyWithRepeatStrategy_IsInnerLoopDurationWithNoOuterMultiplier()
    {
        // arrange: only inner leader strategy, no outer sequence loop
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Interval = 100, Count = 3 } },
        };

        // act + assert: Duration = inner_interval * inner_count * 1 (no outer loop) = 100 * 3 = 300
        sequence.Duration.Should().Be(300);
    }

    [Fact]
    public void Duration_StrategyOnlyWithRepeatStrategy_IsZero_BecauseNoIntervalIsKnown()
    {
        // arrange: outer strategy set without any inner loop providing the interval
        var sequence = new SequenceDesign("test")
        {
            Strategy = new RepeatStrategy { Count = 2 },
        };

        // act + assert: cannot calculate duration without knowing inner loop interval
        sequence.Duration.Should().Be(0);
    }

    [Fact]
    public void Duration_LeaderWithNoRepeatStrategy_IsZero()
    {
        // arrange: leader has no repeat strategy (played once), duration must be set explicitly
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick(),
        };

        // act + assert: cannot auto-calculate duration for non-repeating leaders
        sequence.Duration.Should().Be(0);
    }
}
