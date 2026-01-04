using Beater;

namespace Tests;

public class SequenceTests
{
    [Fact]
    public void GenerateSquareLoopSequence_ReturnExpected()
    {
        // arrange
        var sequence = new Sequence
        {
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
            "0000:k",
            "0150:ts1",
            "0230:ts1",
            "0310:no-sound",
            "0500:k",
            "0650:ts1",
            "0730:ts1",
            "0810:no-sound",
            "1000:k",
            "1150:ts1",
            "1230:ts1",
            "1310:no-sound",
            "1500:k",
            "1500:ts2",
            "1650:ts1",
            "1730:ts1",
            "1750:s",
            "1810:no-sound",
            "2000:k",
            "2150:ts1",
            "2230:ts1",
            "2310:no-sound",
            "2500:k",
            "2650:ts1",
            "2730:ts1",
            "2810:no-sound",
            "3000:k",
            "3150:ts1",
            "3230:ts1",
            "3310:no-sound",
            "3500:k",
            "3500:ts2",
            "3580:ts3",
            "3650:ts1",
            "3660:ts3",
            "3730:ts1",
            "3730:ts3",
            "3750:s",
            "3790:ts3",
            "3810:no-sound",
            "3900:no-sound",
            "4000:k",
            "4150:ts1",
            "4230:ts1",
            "4310:no-sound",
            "4500:k",
            "4650:ts1",
            "4730:ts1",
            "4810:no-sound",
            "5000:k",
            "5150:ts1",
            "5230:ts1",
            "5310:no-sound",
            "5500:k",
            "5500:ts2",
            "5650:ts1",
            "5730:ts1",
            "5750:s",
            "5810:no-sound",
            "6000:k",
            "6150:ts1",
            "6230:ts1",
            "6310:no-sound",
            "6500:k",
            "6650:ts1",
            "6730:ts1",
            "6810:no-sound",
            "7000:k",
            "7150:ts1",
            "7230:ts1",
            "7310:no-sound",
            "7500:k",
            "7500:ts2",
            "7580:ts3",
            "7650:ts1",
            "7660:ts3",
            "7730:ts1",
            "7730:ts3",
            "7750:s",
            "7790:ts3",
            "7810:no-sound",
            "7900:no-sound",
            "8000:no-sound",
        ];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void FollowPreviousSoundStrategy_ReturnExpected()
    {
        // arrange
        var sequence = new Sequence
        {
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

        string[] expected = ["0000:no-sound", "0000:k", "0100:s", "0300:b3", "0600:b4", "1000:no-sound", "1000:k", "1100:s", "1300:b3", "1600:b4", "2000:no-sound"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GenerateFollowers_PlayEvery2_ReturnExpected()
    {
        // arrange
        var sequence = new Sequence
        {
            Leader = new Kick
            {
                Strategy = new RepeatStrategy { Count = 4, Interval = 1000 },
                Followers = [new Sound("every-2nd") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 100, PlayEveryX = 2 } }],
            },
        };
        string[] expected = [
            "0000:k",
            "1000:k",
            "1100:every-2nd",
            "2000:k",
            "3000:k",
            "3100:every-2nd",
            "4000:no-sound",
        ];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GenerateFollowers_PlayEvery3_ReturnExpected()
    {
        // arrange
        var sequence = new Sequence
        {
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
            "0000:no-sound",
            "0000:k",
            "0100:k",
            "0200:k",
            "0250:every-3rd",
            "0300:k",
            "0400:k",
            "0500:k",
            "0550:every-3rd",
            "0600:no-sound",
            "1000:no-sound",
        ];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GenerateFollowerEvery3rdOutOf4_ReturnExpected()
    {
        // arrange
        var sequence = new Sequence
        {
            Leader = new Kick
            {
                Strategy = new RepeatStrategy { Count = 8, Interval = 500 },
                Followers = [
                  new Snare { Strategy = new PlayOnceStrategy { PlayEveryXOutOf = "3/4" } },
                ]
            },
        };
        string[] expected = ["0000:k", "0500:k", "1000:k", "1000:s", "1500:k", "2000:k", "2500:k", "3000:k", "3000:s", "3500:k", "4000:no-sound"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GenerateFollowerEvery1stOutOf2_ReturnExpected()
    {
        // arrange
        var sequence = new Sequence
        {
            Leader = new Kick
            {
                Strategy = new RepeatStrategy { Count = 4, Interval = 500 },
                Followers = [
                  new Sound("ts1") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 100, PlayEveryXOutOf = "1/2" } },
                ]
            },
        };
        string[] expected = ["0000:k", "0100:ts1", "0500:k", "1000:k", "1100:ts1", "1500:k", "2000:no-sound"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SilenceEveryXOutOf4_ReturnExpected()
    {
        // arrange
        var sequence = new Sequence
        {
            Leader = new Kick
            {
                Strategy = new RepeatStrategy { Count = 8, Interval = 500, SilenceEveryXSoundOutOf = "3/4" },
                Followers = [
                  new Sound("ts1") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 100, SilenceEveryXOutOf = "2/4" } },
                ]
            },
        };
        string[] expected = [
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
            "4000:no-sound",
        ];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}{(msg.Sound?.IsSilenced is true ? "-silenced" : "")}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void AppendSequences_ReturnJoined()
    {
        // arrange
        var getSequence = () => new Sequence
        {
            Duration = 500,
            Leader = new Kick()
            {
                Followers = [new Snare() { DelayAfterLeader = 100 }, new Snare() { DelayAfterLeader = 150 }]
            }
        };
        var seq1 = getSequence();
        var seq2 = getSequence();

        // act
        var sequence = new Sequence();
        sequence.Append(seq1);
        sequence.Append(seq2);
        var actual = sequence.Generate();
        var actualTimestamps = actual.GetTimestamps();

        string[] expected = [
            "0000:no-sound", // sequence start
            "0000:k",
            "0100:s",
            "0150:s",
            "0500:no-sound", // joint
            "0500:k",
            "0600:s",
            "0650:s",
        ];

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }
}