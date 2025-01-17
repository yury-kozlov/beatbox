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
            Leader = new Sound("b1")
            {
                Strategy = new RepeatStrategy { Count = 16, Interval = 500 },
                Followers = new() {
                  new Sound("ts1") { Strategy = new RepeatStrategy { DelayAfterLeader = 150, Count = 2, Interval = 80 } },
                  new Sound("ts2") { Strategy = new PlayOnceStrategy { PlayEveryX = 4 } },
                  new Sound("b2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 250, PlayEveryX = 4 } },
                  new Sound("ts3") { Strategy = new RepeatStrategy { DelayAfterLeader = 80, Count = 4, Interval = 80, LinearIncrement = -10, PlayEveryX = 8 } },
               },
            },
        };
        string[] expected = [
            "0000:b1",
            "0150:ts1",
            "0230:ts1",
            "0310:",
            "0500:b1",
            "0650:ts1",
            "0730:ts1",
            "0810:",
            "1000:b1",
            "1150:ts1",
            "1230:ts1",
            "1310:",
            "1500:b1",
            "1500:ts2",
            "1650:ts1",
            "1730:ts1",
            "1750:b2",
            "1810:",
            "2000:b1",
            "2150:ts1",
            "2230:ts1",
            "2310:",
            "2500:b1",
            "2650:ts1",
            "2730:ts1",
            "2810:",
            "3000:b1",
            "3150:ts1",
            "3230:ts1",
            "3310:",
            "3500:b1",
            "3500:ts2",
            "3580:ts3",
            "3650:ts1",
            "3660:ts3",
            "3730:ts1",
            "3730:ts3",
            "3750:b2",
            "3790:ts3",
            "3810:",
            "3900:",
            "4000:b1",
            "4150:ts1",
            "4230:ts1",
            "4310:",
            "4500:b1",
            "4650:ts1",
            "4730:ts1",
            "4810:",
            "5000:b1",
            "5150:ts1",
            "5230:ts1",
            "5310:",
            "5500:b1",
            "5500:ts2",
            "5650:ts1",
            "5730:ts1",
            "5750:b2",
            "5810:",
            "6000:b1",
            "6150:ts1",
            "6230:ts1",
            "6310:",
            "6500:b1",
            "6650:ts1",
            "6730:ts1",
            "6810:",
            "7000:b1",
            "7150:ts1",
            "7230:ts1",
            "7310:",
            "7500:b1",
            "7500:ts2",
            "7580:ts3",
            "7650:ts1",
            "7660:ts3",
            "7730:ts1",
            "7730:ts3",
            "7750:b2",
            "7790:ts3",
            "7810:",
            "7900:",
            "8000:",
        ];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GenerateFollowerEvery3rdOutOf4_ReturnExpected()
    {
        // arrange
        var sequence = new Sequence
        {
            Leader = new Sound("b1")
            {
                Strategy = new RepeatStrategy { Count = 8, Interval = 500 },
                Followers = new() {
                  new Sound("b2") { Strategy = new PlayOnceStrategy { PlayEveryXOutOf = "3/4" } },
               },
            },
        };
        string[] expected = ["0000:b1", "0500:b1", "1000:b1", "1000:b2", "1500:b1", "2000:b1", "2500:b1", "3000:b1", "3000:b2", "3500:b1", "4000:"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GenerateFollowerEvery1stOutOf2_ReturnExpected()
    {
        // arrange
        var sequence = new Sequence
        {
            Leader = new Sound("b1")
            {
                Strategy = new RepeatStrategy { Count = 4, Interval = 500 },
                Followers = new() {
                  new Sound("ts1") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 100, PlayEveryXOutOf = "1/2" } },
               },
            },
        };
        string[] expected = ["0000:b1", "0100:ts1", "0500:b1", "1000:b1", "1100:ts1", "1500:b1", "2000:"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }
}