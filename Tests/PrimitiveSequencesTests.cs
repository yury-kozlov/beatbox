using Beater;

namespace Tests;

public class PrimitiveSequencesTests
{
    [Fact]
    public void Trapezoid_SingleIteration_ReturnExpected()
    {
        // arrange
        var sequence = new PrimitiveSequences.Trapezoid<Kick>()
        {
            XInterval = 500,
            YInterval = 700,
        };

        string[] expected = [
            "0000:sequence-start-trapezoid",
            "0000:k",
            "0500:k",
            "1200:k",
            "1900:k",
            "2400:sequence-end-trapezoid",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
        sequence.AutoDuration.Should().Be(2400);
    }

    [Fact]
    public void Trapezoid_RepeatedX4_ReturnExpected()
    {
        // arrange
        var sequence = new PrimitiveSequences.Trapezoid<Kick>()
        {
            XInterval = 500,
            YInterval = 700,
            Strategy = new RepeatStrategy { Count = 4 },
        };

        string[] expected = [
            "0000:sequence-start-trapezoid",
            "0000:k",
            "0500:k",
            "1200:k",
            "1900:k",
            "2400:sequence-end-trapezoid",
            "2400:sequence-start-trapezoid",
            "2400:k",
            "2900:k",
            "3600:k",
            "4300:k",
            "4800:sequence-end-trapezoid",
            "4800:sequence-start-trapezoid",
            "4800:k",
            "5300:k",
            "6000:k",
            "6700:k",
            "7200:sequence-end-trapezoid",
            "7200:sequence-start-trapezoid",
            "7200:k",
            "7700:k",
            "8400:k",
            "9100:k",
            "9600:end-of-loop",
            "9600:sequence-end-trapezoid",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
        sequence.Duration.Should().Be(9600);

        // verify overlapping sounds at iteration transitions are in correct order:
        actualTimestamps[5].Should().Be("2400:sequence-end-trapezoid");
        actualTimestamps[6].Should().Be("2400:sequence-start-trapezoid");
        actualTimestamps[7].Should().Be("2400:k");
    }
}
