using Beater;

namespace Tests;

public class SequenceDesignTests(ITestOutputHelper output) : TestBase(output)
{
    // -------------------------------------------------------------------------
    // SetLeader — EnforceSequenceStartLeader
    // -------------------------------------------------------------------------

    [Fact]
    public void SetLeader_AlwaysWrapsActualLeaderInSequenceStart()
    {
        // arrange / act
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick()
        };

        // assert
        sequence.Leader.Should().BeOfType<SequenceStart>(); // Leader of SequenceDesign is always a SequenceStart, even when a plain sound is assigned
        sequence.FirstSound.Should().BeOfType<Kick>(); // actual sound becomes first follower of SequenceStart
        sequence.Leader.Followers.Last().Should().BeOfType<SequenceEnd>(); // Sequence end is last follower of SequenceStart
    }

    [Fact]
    public void SetLeader_SequenceEndIsLastFollower_AfterMultipleFollowers()
    {
        // arrange / act
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick()
            {
                Followers =
                [
                    new Snare { Strategy = new FollowLeaderStrategy { DelayAfterLeader = 100 } },
                    new Snare { Strategy = new FollowLeaderStrategy { DelayAfterLeader = 200 } },
                ]
            }
        };

        // assert
        sequence.Leader.Followers.Last().Should().BeOfType<SequenceEnd>();
    }

    [Fact]
    public void SetLeader_AssignsSequenceToLeaderAndFollowers()
    {
        // arrange
        var kick = new Kick
        {
            Followers = [new Snare()]
        };

        // act — WithSequenceIfMissing should propagate sequence reference down to followers
        var sequence = new SequenceDesign("test")
        {
            Leader = kick
        };

        // assert
        sequence.FirstSound!.Sequence.Should().Be(sequence);
        sequence.FirstSound!.Followers[0].Sequence.Should().Be(sequence);
    }

    [Fact]
    public void SetLeader_GeneratesCorrectTimestamps_LeaderWithFollowers()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick
            {
                Followers =
                [
                    new Snare { Strategy = new FollowLeaderStrategy { DelayAfterLeader = 200 } }
                ]
            }
        };

        string[] expected =
        [
            "0000:sequence-start-test",
            "0000:k",
            "0200:s",
            "0200:sequence-end-test: 200 ms",
        ];

        // act
        var actualTimestamps = SequenceGenerator.Generate(sequence).GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
    }

    // -------------------------------------------------------------------------
    // SetStrategy — UpdateLoopInterval / UpdateLoopDuration
    // -------------------------------------------------------------------------

    [Fact]
    public void SetStrategy_RepeatStrategy_SetsStrategyOnLeader()
    {
        // arrange
        var strategy = new RepeatStrategy { Count = 3, Interval = 100 };

        // act
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Count = 3, Interval = 100 } },
            Strategy = strategy,
        };

        // assert
        sequence.Strategy.Should().Be(strategy);
    }

    [Fact]
    public void SetStrategy_RepeatStrategy_UpdatesLoopInterval_WhenDurationIsKnown()
    {
        // arrange
        var repeatStrategy = new RepeatStrategy { Count = 2 };

        // act — Duration is 600 (3 * 100 * 2). After assigning sequence RepeatStrategy with Count=2,
        // UpdateLoopInterval sets Interval = Duration / Count = 600 / 2 = 300.
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Count = 3, Interval = 100 } },
            Strategy = repeatStrategy,
        };

        // assert
        repeatStrategy.Interval.Should().Be(300);
        sequence.Duration.Should().Be(600);
    }

    [Fact]
    public void SetStrategy_RepeatStrategy_WithNoLeaderRepeatStrategy_DoesNotChangeDuration()
    {
        // arrange / act
        var sequence = new SequenceDesign("test")
        {
            Strategy = new RepeatStrategy { Count = 2 },
        };

        // assert — no inner loop: cannot auto-calculate interval, Duration stays 0
        sequence.Duration.Should().Be(0);
    }

    // -------------------------------------------------------------------------
    // OnDurationSet — SequenceEnd strategy recalculation and UpdateLoopInterval
    // -------------------------------------------------------------------------

    [Fact]
    public void OnDurationSet_SequenceEnd_UsesFollowLeaderStrategy_WhenDurationIsPositive()
    {
        // arrange / act
        var sequence = new SequenceDesign("test")
        {
            Duration = 500,
            Leader = new Kick()
        };

        // assert — SequenceEnd.InitStrategy should have produced a FollowLeaderStrategy with DelayAfterLeader = 500
        sequence.SequenceEnd.Strategy.Should().BeOfType<FollowLeaderStrategy>();
        sequence.SequenceEnd.Strategy.DelayAfterLeader.Should().Be(500);
    }

    [Fact]
    public void OnDurationSet_SequenceEnd_UsesFollowPreviousSoundStrategy_WhenDurationIsZero()
    {
        // arrange / act
        var sequence = new SequenceDesign("test")
        {
            Duration = 0,
            Leader = new Kick()
        };

        // assert
        sequence.SequenceEnd.Strategy.Should().BeOfType<FollowPreviousSoundStrategy>();
    }

    [Fact]
    public void OnDurationSet_UpdatesLoopInterval_WhenSequenceRepeatStrategyExists()
    {
        // arrange
        var repeatStrategy = new RepeatStrategy { Count = 2 };
        var sequence = new SequenceDesign("test")
        {
            Strategy = repeatStrategy,
        };
        sequence.Duration.Should().Be(0); // Duration is 0 at this point
        repeatStrategy.Interval.Should().Be(0); // Interval should remain 0

        // act — set duration explicitly, triggering duration and interval update
        sequence.Duration = 800;

        // assert
        repeatStrategy.Interval.Should().Be(400); // Interval should be derived from Duration = 800 / 2 = 400
    }

    [Fact]
    public void OnDurationSet_SequenceEnd_PlacedAtDuration_WhenGeneratingTimestamps()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 500,
            Leader = new Kick()
        };

        string[] expected =
        [
            "0000:sequence-start-test",
            "0000:k",
            "0500:sequence-end-test: 500 ms",
        ];

        // act
        var actualTimestamps = SequenceGenerator.Generate(sequence).GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
    }

    // -------------------------------------------------------------------------
    // UpdateLoopInterval
    // -------------------------------------------------------------------------

    [Fact]
    public void UpdateLoopInterval_SetsIntervalToDurationDividedByCount_WhenRepeatStrategyAndDurationSet()
    {
        // arrange
        var repeatStrategy = new RepeatStrategy { Count = 4 };

        // act
        var sequence = new SequenceDesign("test")
        {
            Strategy = repeatStrategy,
            Duration = 2000,
        };

        // assert
        repeatStrategy.Interval.Should().Be(500); // Interval should be derived from Duration = 2000 / 4 = 500
    }

    [Fact]
    public void UpdateLoopInterval_SetsIntervalToDuration_WhenRepeatStrategyCountIsZero()
    {
        // arrange — Count = 0: division guard uses Duration directly
        var repeatStrategy = new RepeatStrategy { Count = 0 };

        // act
        var sequence = new SequenceDesign("test")
        {
            Strategy = repeatStrategy,
            Duration = 1000,
        };

        // assert
        repeatStrategy.Interval.Should().Be(1000);
    }

    [Fact]
    public void UpdateLoopInterval_DoesNotSetInterval_WhenStrategyIsNotRepeat()
    {
        // arrange / act
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick(),
            Duration = 1000,
        };

        // assert — FollowLeaderStrategy has no Interval; confirm no exception and Duration stays set
        sequence.Duration.Should().Be(1000);
    }

    // -------------------------------------------------------------------------
    // UpdateLoopDuration
    // -------------------------------------------------------------------------

    [Fact]
    public void UpdateLoopDuration_SetsDuration_WhenLeaderHasInitializedRepeatStrategy()
    {
        // arrange / act
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Count = 4, Interval = 250 } },
        };

        // assert
        sequence.Duration.Should().Be(1000); // derive total sequence duration from leader's repeat strategy: 250 * 4 = 1000
    }

    [Fact]
    public void UpdateLoopDuration_MultipliesBySequenceLoopCount_WhenBothStrategiesAreRepeat()
    {
        // arrange / act
        var sequence = new SequenceDesign("test")
        {
            Strategy = new RepeatStrategy { Count = 3 },
            Leader = new Kick { Strategy = new RepeatStrategy { Count = 2, Interval = 200 } },
        };

        // assert
        sequence.Duration.Should().Be(1200); // derive total sequence duration from leader's repeat strategy and sequence loop: 200 * 2 * 3 = 1200
    }

    [Fact]
    public void UpdateLoopDuration_DoesNotChangeDuration_WhenLeaderHasNoRepeatStrategy()
    {
        // arrange / act
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick(),
        };

        // assert
        sequence.Duration.Should().Be(0);
    }

    [Fact]
    public void UpdateLoopDuration_DoesNotChangeDuration_WhenRepeatStrategyIsNotInitialized()
    {
        // arrange / act — RepeatStrategy with Count=0 or Interval=0 is not initialized
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Count = 0, Interval = 0 } },
        };

        // assert
        sequence.Duration.Should().Be(0);
    }

    // -------------------------------------------------------------------------
    // UpdateDurationOnAppend
    // -------------------------------------------------------------------------

    [Fact]
    public void UpdateDurationOnAppend_AccumulatesDuration_WhenBaseSequenceIsNotEmpty()
    {
        // arrange — base sequence starts empty; first append goes through the expansion path, second accumulates
        var seq1 = new SequenceDesign("seq1") { Duration = 300 };
        var seq2 = new SequenceDesign("seq2") { Duration = 400 };
        var main = new SequenceDesign("main");

        // act
        main.Append(seq1);
        main.Append(seq2);

        // assert
        main.Duration.Should().Be(700); // total duration = 300 + 400 = 700
    }

    [Fact]
    public void UpdateDurationOnAppend_ExpandsRepeatInterval_WhenBaseIsEmpty_AndAppendedIsLarger()
    {
        // arrange — base sequence is empty but has a RepeatStrategy with Count=2 and Interval=100;
        // appended sequence has Duration=300 which is larger than Interval=100
        var strategy = new RepeatStrategy { Count = 2, Interval = 100 };
        var main = new SequenceDesign("main") { Strategy = strategy };
        var appended = new SequenceDesign("appended") { Duration = 300 };

        // act
        main.Append(appended);

        // assert
        strategy.Interval.Should().Be(300); // Interval should be expanded from 100 to 300 to accomodate the appended sequence
        main.Duration.Should().Be(600); // total sequence duration should be expanded to 300 * 2 = 600
    }

    [Fact]
    public void UpdateDurationOnAppend_KeepsRepeatInterval_WhenBaseIsEmpty_AndAppendedIsSmallerThanInterval()
    {
        // arrange
        var strategy = new RepeatStrategy { Count = 2, Interval = 100 };
        var main = new SequenceDesign("main") { Strategy = strategy };
        strategy.Interval.Should().Be(100); // Interval=100, Duration=200 after setting strategy
        main.Duration.Should().Be(200);

        var appended = new SequenceDesign("appended") { Duration = 50 };

        // act
        main.Append(appended);

        // assert
        strategy.Interval.Should().Be(100); // Interval should remain 100 (no change, since appended duration 50 is smaller than current interval 100)
        main.Duration.Should().Be(200); // Duration stays 200 because Interval is unchanged
    }

    [Fact]
    public void UpdateDurationOnAppend_AccumulateDuration_ThenUpdateLoopDuration_WhenStrategyIsRepeat()
    {
        // arrange — sequence strategy is PlayOnce (default), so durations simply accumulate
        var seq1 = new SequenceDesign("seq1") { Duration = 200 };
        var seq2 = new SequenceDesign("seq2") { Duration = 300 };
        var seq3 = new SequenceDesign("seq3") { Duration = 100 };
        var main = new SequenceDesign("main");

        // act
        main.Append(seq1);
        main.Append(seq2);
        main.Append(seq3);

        // assert
        main.Duration.Should().Be(600); // 200 + 300 + 100 = 600
    }

    // -------------------------------------------------------------------------
    // SetDelayAfterLeader
    // -------------------------------------------------------------------------

    [Fact]
    public void SetDelayAfterLeader_SetsDelayOnLeaderStrategy()
    {
        // arrange / act
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick(),
            DelayAfterLeader = 250,
        };

        // assert
        sequence.Strategy.DelayAfterLeader.Should().Be(250);
    }

    [Fact]
    public void SetDelayAfterLeader_ShiftsEntireSequenceInTimestamps()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick(),
            DelayAfterLeader = 300,
        };

        // act
        var actualTimestamps = SequenceGenerator.Generate(sequence).GetTimestamps();

        // assert
        actualTimestamps.Should().Contain("0300:k"); // Kick delayed by 300ms relative to SequenceStart
    }

    // -------------------------------------------------------------------------
    // Integration — NestedLoops via SetStrategy + SetLeader interactions
    // -------------------------------------------------------------------------

    [Fact]
    public void NestedLoop_Strategy_And_Leader_Produce_CorrectTimestamps()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Strategy = new RepeatStrategy { Count = 2 },
            Leader = new Kick { Strategy = new RepeatStrategy { Interval = 100, Count = 3 } },
        };

        string[] expected =
        [
            "0000:sequence-start-test",
            "0000:k",
            "0100:k",
            "0200:k",
            "0300:end-of-loop",
            "0300:sequence-end-test: 300 ms",
            "0300:sequence-start-test",
            "0300:k",
            "0400:k",
            "0500:k",
            "0600:end-of-loop",
            "0600:end-of-sequence-loop-test",
            "0600:sequence-end-test: 300 ms",
        ];

        // act
        var actualTimestamps = SequenceGenerator.Generate(sequence).GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.Duration.Should().Be(600); // 300 + 300
    }

    [Fact]
    public void SetLeader_InitializationOrder_LeaderFirst_ThenStrategy_SameDuration()
    {
        // arrange / act — regression guard for order-dependence: leader before strategy vs strategy before leader
        var leaderFirst = new SequenceDesign("a")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Interval = 100, Count = 3 } },
            Strategy = new RepeatStrategy { Count = 2 },
        };

        var strategyFirst = new SequenceDesign("b")
        {
            Strategy = new RepeatStrategy { Count = 2 },
            Leader = new Kick { Strategy = new RepeatStrategy { Interval = 100, Count = 3 } },
        };

        // assert
        leaderFirst.Duration.Should().Be(strategyFirst.Duration).And.Be(600);
    }
}
