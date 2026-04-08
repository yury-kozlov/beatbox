using Beater;

namespace Tests;

public class SequenceDesignInitializerTests(ITestOutputHelper output) : TestBase(output)
{
    // -------------------------------------------------------------------------
    // SetLeader — EnforceSequenceStartLeader
    // -------------------------------------------------------------------------

    [Fact]
    public void SetLeader_AlwaysWrapsActualLeaderInSequenceStart()
    {
        // The Leader property of SequenceDesign is always a SequenceStart,
        // even when a plain sound is assigned.
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick()
        };

        sequence.Leader.Should().BeOfType<SequenceStart>();
    }

    [Fact]
    public void SetLeader_ActualSoundBecomesFirstFollowerOfSequenceStart()
    {
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick()
        };

        sequence.FirstSound.Should().BeOfType<Kick>();
    }

    [Fact]
    public void SetLeader_SequenceEndIsLastFollowerOfSequenceStart()
    {
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick()
        };

        sequence.Leader.Followers.Last().Should().BeOfType<SequenceEnd>();
    }

    [Fact]
    public void SetLeader_SequenceEndIsLastFollower_AfterMultipleFollowers()
    {
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick()
            {
                Followers =
                [
                    new Snare { Strategy = new PlayOnceStrategy { DelayAfterLeader = 100 } },
                    new Snare { Strategy = new PlayOnceStrategy { DelayAfterLeader = 200 } },
                ]
            }
        };

        sequence.Leader.Followers.Last().Should().BeOfType<SequenceEnd>();
    }

    [Fact]
    public void SetLeader_AssignsSequenceToLeaderAndFollowers()
    {
        // WithSequenceIfMissing should propagate sequence reference down to followers.
        var kick = new Kick
        {
            Followers = [new Snare()]
        };

        var sequence = new SequenceDesign("test")
        {
            Leader = kick
        };

        // The actual leader sound (first follower of SequenceStart) should have Sequence assigned.
        sequence.FirstSound!.Sequence.Should().Be(sequence);

        // And so should its follower.
        sequence.FirstSound!.Followers[0].Sequence.Should().Be(sequence);
    }

    [Fact]
    public void SetLeader_GeneratesCorrectTimestamps_LeaderWithFollowers()
    {
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick
            {
                Followers =
                [
                    new Snare { Strategy = new PlayOnceStrategy { DelayAfterLeader = 200 } }
                ]
            }
        };

        string[] expected =
        [
            "0000:sequence-start-test",
            "0000:k",
            "0200:s",
            "0200:sequence-end-test",
        ];

        var actualTimestamps = SequenceGenerator.Generate(sequence).GetTimestamps();

        actualTimestamps.Should().BeExactSequence(expected);
    }

    // -------------------------------------------------------------------------
    // SetStrategy — UpdateLoopInterval / UpdateLoopDuration
    // -------------------------------------------------------------------------

    [Fact]
    public void SetStrategy_RepeatStrategy_SetsStrategyOnLeader()
    {
        var strategy = new RepeatStrategy { Count = 3, Interval = 100 };
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Count = 3, Interval = 100 } },
            Strategy = strategy,
        };

        sequence.Strategy.Should().Be(strategy);
    }

    [Fact]
    public void SetStrategy_RepeatStrategy_UpdatesLoopInterval_WhenDurationIsKnown()
    {
        // Duration is 600 (3 * 100 * 2). After assigning outer RepeatStrategy with Count=2,
        // UpdateLoopInterval should set Interval = Duration / Count = 600 / 2 = 300.
        var outerStrategy = new RepeatStrategy { Count = 2 };
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Count = 3, Interval = 100 } },
            Strategy = outerStrategy,
        };

        // Duration = 100 * 3 * 2 = 600; Interval = 600 / 2 = 300
        outerStrategy.Interval.Should().Be(300);
        sequence.Duration.Should().Be(600);
    }

    [Fact]
    public void SetStrategy_RepeatStrategy_WithNoLeaderRepeatStrategy_DoesNotChangeDuration()
    {
        var sequence = new SequenceDesign("test")
        {
            Strategy = new RepeatStrategy { Count = 2 },
        };

        // No inner loop: cannot auto-calculate interval, Duration stays 0
        sequence.Duration.Should().Be(0);
    }

    // -------------------------------------------------------------------------
    // OnDurationSet — SequenceEnd strategy recalculation and UpdateLoopInterval
    // -------------------------------------------------------------------------

    [Fact]
    public void OnDurationSet_SequenceEnd_UsesPlayOnceStrategy_WhenDurationIsPositive()
    {
        var sequence = new SequenceDesign("test")
        {
            Duration = 500,
            Leader = new Kick()
        };

        // SequenceEnd.InitStrategy should have produced a PlayOnceStrategy with DelayAfterLeader = 500
        sequence.SequenceEnd.Strategy.Should().BeOfType<PlayOnceStrategy>();
        sequence.SequenceEnd.Strategy.DelayAfterLeader.Should().Be(500);
    }

    [Fact]
    public void OnDurationSet_SequenceEnd_UsesFollowPreviousSoundStrategy_WhenDurationIsZero()
    {
        var sequence = new SequenceDesign("test")
        {
            Duration = 0,
            Leader = new Kick()
        };

        sequence.SequenceEnd.Strategy.Should().BeOfType<FollowPreviousSoundStrategy>();
    }

    [Fact]
    public void OnDurationSet_UpdatesLoopInterval_WhenOuterRepeatStrategyExists()
    {
        var outerStrategy = new RepeatStrategy { Count = 2 };
        var sequence = new SequenceDesign("test")
        {
            Strategy = outerStrategy,
        };

        // Duration is 0 at this point; interval should remain 0
        outerStrategy.Interval.Should().Be(0);

        // Now set duration explicitly, triggering OnDurationSet → UpdateLoopInterval
        sequence.Duration = 800;

        // Interval = 800 / 2 = 400
        outerStrategy.Interval.Should().Be(400);
    }

    [Fact]
    public void OnDurationSet_SequenceEnd_PlacedAtDuration_WhenGeneratingTimestamps()
    {
        var sequence = new SequenceDesign("test")
        {
            Duration = 500,
            Leader = new Kick()
        };

        string[] expected =
        [
            "0000:sequence-start-test",
            "0000:k",
            "0500:sequence-end-test",
        ];

        var actualTimestamps = SequenceGenerator.Generate(sequence).GetTimestamps();

        actualTimestamps.Should().BeExactSequence(expected);
    }

    // -------------------------------------------------------------------------
    // UpdateLoopInterval
    // -------------------------------------------------------------------------

    [Fact]
    public void UpdateLoopInterval_SetsIntervalToDurationDividedByCount_WhenRepeatStrategyAndDurationSet()
    {
        var outerStrategy = new RepeatStrategy { Count = 4 };
        var sequence = new SequenceDesign("test")
        {
            Strategy = outerStrategy,
            Duration = 2000,
        };

        // Interval = 2000 / 4 = 500
        outerStrategy.Interval.Should().Be(500);
    }

    [Fact]
    public void UpdateLoopInterval_SetsIntervalToDuration_WhenRepeatStrategyCountIsZero()
    {
        // Count = 0: division guard uses Duration directly
        var outerStrategy = new RepeatStrategy { Count = 0 };
        var sequence = new SequenceDesign("test")
        {
            Strategy = outerStrategy,
            Duration = 1000,
        };

        outerStrategy.Interval.Should().Be(1000);
    }

    [Fact]
    public void UpdateLoopInterval_DoesNotSetInterval_WhenStrategyIsNotRepeat()
    {
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick(),
            Duration = 1000,
        };

        // PlayOnceStrategy has no Interval — just confirm no exception and Duration stays set
        sequence.Duration.Should().Be(1000);
    }

    // -------------------------------------------------------------------------
    // UpdateLoopDuration
    // -------------------------------------------------------------------------

    [Fact]
    public void UpdateLoopDuration_SetsDuration_WhenLeaderHasInitializedRepeatStrategy()
    {
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Count = 4, Interval = 250 } },
        };

        // Duration = 250 * 4 * 1 (no outer loop) = 1000
        sequence.Duration.Should().Be(1000);
    }

    [Fact]
    public void UpdateLoopDuration_MultipliesByOuterLoopCount_WhenBothStrategiesAreRepeat()
    {
        var sequence = new SequenceDesign("test")
        {
            Strategy = new RepeatStrategy { Count = 3 },
            Leader = new Kick { Strategy = new RepeatStrategy { Count = 2, Interval = 200 } },
        };

        // Duration = 200 * 2 * 3 = 1200
        sequence.Duration.Should().Be(1200);
    }

    [Fact]
    public void UpdateLoopDuration_DoesNotChangeDuration_WhenLeaderHasNoRepeatStrategy()
    {
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick(),
        };

        sequence.Duration.Should().Be(0);
    }

    [Fact]
    public void UpdateLoopDuration_DoesNotChangeDuration_WhenRepeatStrategyIsNotInitialized()
    {
        // RepeatStrategy with Count=0 or Interval=0 is not initialized
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Count = 0, Interval = 0 } },
        };

        sequence.Duration.Should().Be(0);
    }

    // -------------------------------------------------------------------------
    // UpdateDurationOnAppend
    // -------------------------------------------------------------------------

    [Fact]
    public void UpdateDurationOnAppend_AccumulatesDuration_WhenBaseSequenceIsNotEmpty()
    {
        // Base sequence starts empty; first append goes through the expansion path.
        // Second append accumulates.
        var seq1 = new SequenceDesign("seq1") { Duration = 300 };
        var seq2 = new SequenceDesign("seq2") { Duration = 400 };

        var main = new SequenceDesign("main");
        main.Append(seq1);
        main.Append(seq2);

        // Total duration = 300 + 400 = 700
        main.Duration.Should().Be(700);
    }

    [Fact]
    public void UpdateDurationOnAppend_ExpandsRepeatInterval_WhenBaseIsEmpty_AndAppendedIsLarger()
    {
        // Base sequence is empty but has a RepeatStrategy with Count=2 and Interval=100.
        // Appended sequence has Duration=300 which is larger than Interval=100.
        // Interval should be expanded to 300; Duration = 300 * 2 = 600.
        var strategy = new RepeatStrategy { Count = 2, Interval = 100 };
        var main = new SequenceDesign("main")
        {
            Strategy = strategy,
        };

        var appended = new SequenceDesign("appended") { Duration = 300 };
        main.Append(appended);

        strategy.Interval.Should().Be(300);
        main.Duration.Should().Be(600);
    }

    [Fact]
    public void UpdateDurationOnAppend_KeepsRepeatInterval_WhenBaseIsEmpty_AndAppendedIsSmallerThanInterval()
    {
        // When only an outer RepeatStrategy is set (no inner leader loop), Interval stays as-is.
        // Duration = Interval * Count = 100 * 2 = 200.
        var strategy = new RepeatStrategy { Count = 2, Interval = 100 };
        var main = new SequenceDesign("main")
        {
            Strategy = strategy,
        };

        // After setting strategy: Interval=100, Duration=200
        strategy.Interval.Should().Be(100);

        var appended = new SequenceDesign("appended") { Duration = 50 };
        main.Append(appended);

        // Appended duration (50) < current interval (100): Math.Max(100,50)=100, Duration stays 200
        strategy.Interval.Should().Be(100);
        main.Duration.Should().Be(200);
    }

    [Fact]
    public void UpdateDurationOnAppend_AccumulateDuration_ThenUpdateLoopDuration_WhenStrategyIsRepeat()
    {
        // After accumulation UpdateLoopDuration is called:
        // if sequence strategy is NOT RepeatStrategy, we just accumulate. Here it's PlayOnce.
        var seq1 = new SequenceDesign("seq1") { Duration = 200 };
        var seq2 = new SequenceDesign("seq2") { Duration = 300 };
        var seq3 = new SequenceDesign("seq3") { Duration = 100 };

        var main = new SequenceDesign("main");
        main.Append(seq1);
        main.Append(seq2);
        main.Append(seq3);

        // 200 + 300 + 100 = 600
        main.Duration.Should().Be(600);
    }

    // -------------------------------------------------------------------------
    // SetDelayAfterLeader
    // -------------------------------------------------------------------------

    [Fact]
    public void SetDelayAfterLeader_SetsDelayOnLeaderStrategy()
    {
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick(),
            DelayAfterLeader = 250,
        };

        sequence.Strategy.DelayAfterLeader.Should().Be(250);
    }

    [Fact]
    public void SetDelayAfterLeader_ShiftsEntireSequenceInTimestamps()
    {
        var sequence = new SequenceDesign("test")
        {
            Leader = new Kick(),
            DelayAfterLeader = 300,
        };

        // The SequenceStart itself follows a previous sound so it won't appear shifted;
        // but the first actual sound (Kick) will be delayed by 300ms relative to SequenceStart.
        // In the generated output the Kick appears at timestamp 300.
        var actualTimestamps = SequenceGenerator.Generate(sequence).GetTimestamps();

        actualTimestamps.Should().Contain("0300:k");
    }

    // -------------------------------------------------------------------------
    // Integration — NestedLoops via SetStrategy + SetLeader interactions
    // -------------------------------------------------------------------------

    [Fact]
    public void NestedLoop_Strategy_And_Leader_Produce_CorrectTimestamps()
    {
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
            "0300:sequence-end-test",
            "0300:sequence-start-test",
            "0300:k",
            "0400:k",
            "0500:k",
            "0600:end-of-loop",
            "0600:end-of-sequence-loop-test",
            "0600:sequence-end-test",
        ];

        var actualTimestamps = SequenceGenerator.Generate(sequence).GetTimestamps();

        actualTimestamps.Should().BeExactSequence(expected);
        sequence.Duration.Should().Be(600);
    }

    [Fact]
    public void SetLeader_InitializationOrder_LeaderFirst_ThenStrategy_SameDuration()
    {
        // Verify that setting leader before strategy yields the same duration
        // as strategy before leader (regression guard for order-dependence).
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

        leaderFirst.Duration.Should().Be(strategyFirst.Duration).And.Be(600);
    }
}
