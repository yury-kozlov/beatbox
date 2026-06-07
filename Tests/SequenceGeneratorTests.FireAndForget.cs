using Beater;

namespace Tests;

public class SequenceGeneratorFireAndForgetTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void FireAndForget_SkippedByFollowPreviousSoundStrategy()
    {
        // arrange
        // Kick has FireAndForget=true, so Snare (using FollowPreviousSoundStrategy) should skip it
        // and follow Sound("a") instead
        var sequence = new SequenceDesign("test")
        {
            Duration = 0,
            Leader = new NoSound
            {
                Strategy = new RepeatStrategy() { Count = 2, Interval = 1000 },
                Followers = [
                    new Sound("a") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 200 } },
                    new Kick { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 100, FireAndForget = true } },
                    new Snare { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 50 } },
                ]
            },
        };

        string[] expected = [
            "0000:sequence-start-test",
            "0000:no-sound",
            "0200:a",
            "0250:s",   // follows a (200+50), skipping k which has FireAndForget=true
            "0300:k",   // FireAndForget: follows a (200+100), plays in parallel to rest
            "1000:no-sound",
            "1200:a",
            "1250:s",
            "1300:k",
            "2000:end-of-loop",
            "2000:sequence-end-test: 2000 ms",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.AutoDuration.Should().Be(2000);
    }

    [Fact]
    public void FireAndForget_PropagatedToDirectFollowers()
    {
        // arrange
        // Kick has FireAndForget=true; its direct follower Sound("a") inherits this flag via propagation.
        // Snare (using FollowPreviousSoundStrategy) should skip both k and a (both FAF) and find no previous sound.
        var sequence = new SequenceDesign("test")
        {
            Duration = 0,
            Leader = new Sound("")
            {
                Strategy = new RepeatStrategy() { Count = 2, Interval = 1000 },
                Followers = [
                    new Kick
                    {
                        Strategy = new FollowLeaderStrategy() { DelayAfterLeader = 0, FireAndForget = true },
                        Followers = [
                            new Sound("a") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 100 } },
                        ]
                    },
                    new Snare { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 50 } },
                ]
            },
        };

        string[] expected = [
            "0000:sequence-start-test",
            "0000:no-sound",
            "0000:k",   // FireAndForget
            "0050:s",   // skips k (FAF) and a (inherited FAF), no previous found — uses delay only
            "0100:a",   // inherits FireAndForget from k via propagation
            "1000:no-sound",
            "1000:k",
            "1050:s",
            "1100:a",
            "2000:end-of-loop",
            "2000:sequence-end-test: 2000 ms",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.AutoDuration.Should().Be(2000);
    }

    [Fact]
    public void SequenceDesignLevelFireAndForget_InternalSoundsChainNormally()
    {
        // arrange
        // FireAndForget is set at the sequence-design level (via sequence.Strategy).
        // Internal sounds should still chain normally via FollowPreviousSoundStrategy,
        // while the whole sequence is treated as fire-and-forget by external sequences (via SequenceEnd).
        var sequence = new SequenceDesign("test")
        {
            Strategy = new FollowLeaderStrategy() { FireAndForget = true },
            Leader = new Snare()
            {
                Followers = new()
                {
                    new Snare() { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 100}},
                    new Snare() { Strategy = new FollowPreviousSoundStrategy() {DelayAfterLeader = 200}},
                },
            },
        };

        string[] expected = [
            "0000:sequence-start-test",
            "0000:s",
            "0100:s",
            "0300:s",
            "0300:sequence-end-test: 300 ms",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.AutoDuration.Should().Be(300);
    }

}