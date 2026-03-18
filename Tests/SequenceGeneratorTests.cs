using Beater;

namespace Tests;

public class SequenceGeneratorTests
{
    [Fact]
    public void PlayOnceStrategy_ReturnExpected()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Metronome()
            {
                Followers = [
                    new Kick(),
                    new Kick { Strategy = new PlayOnceStrategy() { DelayAfterLeader = 300 }},
                    new Snare { Strategy = new PlayOnceStrategy() {DelayAfterLeader = 600 }},
                    new Snare { Strategy = new PlayOnceStrategy() {DelayAfterLeader = 1500 }},
                    new Kick { Strategy = new PlayOnceStrategy() {DelayAfterLeader = 1800 }},
                ]
            },
        };

        string[] expected = [
            "0000:sequence-start-test",
            "0000:metronome",
            "0000:k",
            "0300:k",
            "0600:s",
            "1500:s",
            "1800:k",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
        sequence.AutoDuration.Should().Be(1800);
    }

    [Fact]
    public void Metronom_RepeatStrategy_ReturnExpected()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Metronome() { Strategy = new RepeatStrategy() { Count = 2, Interval = 1000 } },
        };

        string[] expected = [
            "0000:sequence-start-test",
            "0000:metronome",
            "1000:metronome",
            "2000:end-of-loop",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
        sequence.AutoDuration.Should().Be(2000);
    }

    [Fact]
    public void RepeatStrategy_SoundsBeyondTheLoopShouldBeIgnored()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Kick()
            {
                Strategy = new RepeatStrategy() { Count = 1, Interval = 1000 },
                Followers = [
                    new Snare { Strategy = new RepeatStrategy() {
                        Interval = 600, Count = 3, // third iteration at 1200ms will exceed leader's loop of 1000ms
                        TrimIfExceedsParentLoop = true}},
                ]
            },
        };

        string[] expected = [
            "0000:sequence-start-test",
            "0000:k",
            "0000:s",
            "0600:s",
            "1000:end-of-loop",
            "1000:end-of-loop",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
        sequence.AutoDuration.Should().Be(1000);
    }

    [Fact]
    public void PlayOnceStrategy_Repeated_ReturnExpected()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy() { Count = 2, Interval = 1000 },
                Followers = [
                    new Kick(),
                    new Snare { Strategy = new PlayOnceStrategy() {DelayAfterLeader = 500 }},
                ]
            },
        };

        string[] expected = [
            "0000:sequence-start-test",
            "0000:metronome",
            "0000:k",
            "0500:s",
            "1000:metronome",
            "1000:k",
            "1500:s",
            "2000:end-of-loop",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
        sequence.AutoDuration.Should().Be(2000);
    }

    [Fact]
    public void GenerateSquareLoopSequence_ReturnExpected()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Kick
            {
                Strategy = new RepeatStrategy { Count = 16, Interval = 500 },
                Followers = [
                  new Sound("ts1") { Strategy = new RepeatStrategy { DelayAfterLeader = 150, Count = 2, Interval = 80 } },
                  new Sound("ts2") { Strategy = new PlayOnceStrategy { PlayEveryX = 4 } },
                  new Snare { Strategy = new PlayOnceStrategy { DelayAfterLeader = 250, PlayEveryX = 4 } },
                  new Sound("ts3") { Strategy = new RepeatStrategy { DelayAfterLeader = 80, Count = 4, Interval = 80, LinearIncrement = -10, PlayEveryX = 8 } },
               ]
            },
        };
        string[] expected = [
            "0000:sequence-start-test",
            "0000:k",
            "0150:ts1",
            "0230:ts1",
            "0310:end-of-loop",
            "0500:k",
            "0650:ts1",
            "0730:ts1",
            "0810:end-of-loop",
            "1000:k",
            "1150:ts1",
            "1230:ts1",
            "1310:end-of-loop",
            "1500:k",
            "1500:ts2",
            "1650:ts1",
            "1730:ts1",
            "1750:s",
            "1810:end-of-loop",
            "2000:k",
            "2150:ts1",
            "2230:ts1",
            "2310:end-of-loop",
            "2500:k",
            "2650:ts1",
            "2730:ts1",
            "2810:end-of-loop",
            "3000:k",
            "3150:ts1",
            "3230:ts1",
            "3310:end-of-loop",
            "3500:k",
            "3500:ts2",
            "3580:ts3",
            "3650:ts1",
            "3660:ts3",
            "3730:ts1",
            "3730:ts3",
            "3750:s",
            "3790:ts3",
            "3810:end-of-loop",
            "3900:end-of-loop",
            "4000:k",
            "4150:ts1",
            "4230:ts1",
            "4310:end-of-loop",
            "4500:k",
            "4650:ts1",
            "4730:ts1",
            "4810:end-of-loop",
            "5000:k",
            "5150:ts1",
            "5230:ts1",
            "5310:end-of-loop",
            "5500:k",
            "5500:ts2",
            "5650:ts1",
            "5730:ts1",
            "5750:s",
            "5810:end-of-loop",
            "6000:k",
            "6150:ts1",
            "6230:ts1",
            "6310:end-of-loop",
            "6500:k",
            "6650:ts1",
            "6730:ts1",
            "6810:end-of-loop",
            "7000:k",
            "7150:ts1",
            "7230:ts1",
            "7310:end-of-loop",
            "7500:k",
            "7500:ts2",
            "7580:ts3",
            "7650:ts1",
            "7660:ts3",
            "7730:ts1",
            "7730:ts3",
            "7750:s",
            "7790:ts3",
            "7810:end-of-loop",
            "7900:end-of-loop",
            "8000:end-of-loop"
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
        sequence.AutoDuration.Should().Be(8000);
    }

    [Fact]
    public void FollowPreviousSoundStrategy_ReturnExpected()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Sound("")
            {
                Strategy = new RepeatStrategy() { Count = 2, Interval = 1000 },
                Followers = [
                    new Kick { Strategy = new FollowPreviousSoundStrategy() },
                    new Snare { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 100 } },
                    new Sound("b3") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 200 } },
                    new Sound("b4") { Strategy = new FollowPreviousSoundStrategy() { DelayAfterLeader = 300 } },
                ]
            }
        };

        string[] expected = [
            "0000:sequence-start-test",
            "0000:no-sound",
            "0000:k",
            "0100:s",
            "0300:b3",
            "0600:b4",
            "1000:no-sound",
            "1000:k",
            "1100:s",
            "1300:b3",
            "1600:b4",
            "2000:end-of-loop",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
        sequence.AutoDuration.Should().Be(2000);
    }

    [Fact]
    public void GenerateFollowers_PlayEvery2_ReturnExpected()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Kick
            {
                Strategy = new RepeatStrategy { Count = 4, Interval = 1000 },
                Followers = [new Sound("every-2nd") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 100, PlayEveryX = 2 } }],
            },
        };
        string[] expected = [
            "0000:sequence-start-test",
            "0000:k",
            "1000:k",
            "1100:every-2nd",
            "2000:k",
            "3000:k",
            "3100:every-2nd",
            "4000:end-of-loop",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
        sequence.AutoDuration.Should().Be(4000);
    }

    [Fact]
    public void GenerateFollowers_PlayEvery3_ReturnExpected()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Metronome()
            {
                Strategy = new RepeatStrategy { Count = 1, Interval = 1000 },
                Followers = [
                    new Kick
                    {
                        Strategy = new RepeatStrategy { Count = 6, Interval = 100 },
                        Followers = [
                            new Sound("every-3rd") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 50, PlayEveryX = 3 } },
                       ]
                    },
                ]
            },
        };
        string[] expected = [
            "0000:sequence-start-test",
            "0000:metronome",
            "0000:k",
            "0100:k",
            "0200:k",
            "0250:every-3rd",
            "0300:k",
            "0400:k",
            "0500:k",
            "0550:every-3rd",
            "0600:end-of-loop",
            "1000:end-of-loop",
            "1000:sequence-end-test",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
        sequence.AutoDuration.Should().Be(1000);
    }

    [Fact]
    public void GenerateFollowerEvery3rdOutOf4_ReturnExpected()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Kick
            {
                Strategy = new RepeatStrategy { Count = 8, Interval = 500 },
                Followers = [
                  new Snare { Strategy = new PlayOnceStrategy { PlayEveryXOutOf = "3/4" } },
                ]
            },
        };
        string[] expected = [
            "0000:sequence-start-test",
            "0000:k",
            "0500:k",
            "1000:k",
            "1000:s",
            "1500:k",
            "2000:k",
            "2500:k",
            "3000:k",
            "3000:s",
            "3500:k",
            "4000:end-of-loop",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
        sequence.AutoDuration.Should().Be(4000);
    }

    [Fact]
    public void GenerateFollowerEvery1stOutOf2_ReturnExpected()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Kick
            {
                Strategy = new RepeatStrategy { Count = 4, Interval = 500 },
                Followers = [
                  new Sound("ts1") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 100, PlayEveryXOutOf = "1/2" } },
                ]
            },
        };
        string[] expected = [
            "0000:sequence-start-test",
            "0000:k",
            "0100:ts1",
            "0500:k",
            "1000:k",
            "1100:ts1",
            "1500:k",
            "2000:end-of-loop",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
        sequence.AutoDuration.Should().Be(2000);
    }

    [Fact]
    public void SilenceEveryXOutOf4_ReturnExpected()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Kick
            {
                Strategy = new RepeatStrategy { Count = 8, Interval = 500, SilenceEveryXSoundOutOf = "3/4" },
                Followers = [
                  new Sound("ts1") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 100, SilenceEveryXOutOf = "2/4" } },
                ]
            },
        };
        string[] expected = [
            "0000:sequence-start-test",
            "0000:k",
            "0100:ts1",
            "0500:k",
            "0600:ts1-silenced",
            "1000:k-silenced",
            "1100:ts1",
            "1500:k",
            "1600:ts1",
            "2000:k",
            "2100:ts1",
            "2500:k",
            "2600:ts1-silenced",
            "3000:k-silenced",
            "3100:ts1",
            "3500:k",
            "3600:ts1",
            "4000:end-of-loop",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        foreach (var sound in actual)
        {
            // adjust sound name for easier assertion:
            sound.Name += (sound.IsSilenced is true ? "-silenced" : "");
        }
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
        sequence.AutoDuration.Should().Be(4000);
    }


    [Fact]
    public void NestedLoop_WithRepeatStrategy_ReturnExpected()
    {
        // arrange
        var sequence = new SequenceDesign("loop")
        {
            Strategy = new RepeatStrategy { Count = 2 },
            Leader = new Kick { Strategy = new RepeatStrategy { Interval = 100, Count = 3 } },
        };

        string[] expected = [
            "0000:sequence-start-loop",
            "0000:k",
            "0100:k",
            "0200:k",
            "0300:end-of-loop",
            "0300:sequence-end-loop",
            "0300:sequence-start-loop",
            "0300:k",
            "0400:k",
            "0500:k",
            "0600:end-of-loop",
            "0600:end-of-loop",
            "0600:sequence-end-loop",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
    }
}