using Beater;

namespace Tests;

public class SlowBeatTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void SlowBeat_GetSequence1_ReturnExpected()
    {
        // arrange
        var sequence = SlowBeat.GetSequence1();
        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:k",
            "0300:s",
            "0500:k",
            "1000:k",
            "1800:k",
            "2300:s",
            "2500:k",
            "3000:k",
            "4000:sequence-end-" + sequence.Name,
            "4000:sequence-start-" + sequence.Name,
            "4000:k",
            "4300:s",
            "4500:k",
            "5000:k",
            "5800:k",
            "6300:s",
            "6500:k",
            "7000:k",
            "8000:sequence-end-" + sequence.Name,
            "8000:sequence-start-" + sequence.Name,
            "8000:k",
            "8300:s",
            "8500:k",
            "9000:k",
            "9800:k",
            "10300:s",
            "10500:k",
            "11000:k",
            "12000:sequence-end-" + sequence.Name,
            "12000:sequence-start-" + sequence.Name,
            "12000:k",
            "12300:s",
            "12500:k",
            "13000:k",
            "13800:k",
            "14300:s",
            "14500:k",
            "15000:k",
            "16000:end-of-sequence-loop-" + sequence.Name,
            "16000:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.Duration.Should().Be(16000);
    }

    [Fact]
    public void SlowBeat_GetSequence2_ReturnExpected()
    {
        // arrange
        var sequence = SlowBeat.GetSequence2();
        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:k",
            "0300:s",
            "0500:k",
            "0800:s",
            "1000:k",
            "1500:s",
            "2000:k",
            "3400:sequence-end-" + sequence.Name,
            "3400:sequence-start-" + sequence.Name,
            "3400:k",
            "3700:s",
            "3900:k",
            "4200:s",
            "4400:k",
            "4900:s",
            "5400:k",
            "6800:sequence-end-" + sequence.Name,
            "6800:sequence-start-" + sequence.Name,
            "6800:k",
            "7100:s",
            "7300:k",
            "7600:s",
            "7800:k",
            "8300:s",
            "8800:k",
            "10200:sequence-end-" + sequence.Name,
            "10200:sequence-start-" + sequence.Name,
            "10200:k",
            "10500:s",
            "10700:k",
            "11000:s",
            "11200:k",
            "11700:s",
            "12200:k",
            "13600:end-of-sequence-loop-" + sequence.Name,
            "13600:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.Duration.Should().Be(13600);
    }

    [Fact]
    public void SlowBeat_GetSequence3_ReturnExpected()
    {
        // arrange
        var sequence = SlowBeat.GetSequence3();
        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:k",
            "0300:k",
            "0500:k",
            "1000:s",
            "1500:k",
            "1800:k",
            "2000:k",
            "2300:k",
            "2500:k",
            "3000:s",
            "3500:k",
            "4800:sequence-end-" + sequence.Name,
            "4800:sequence-start-" + sequence.Name,
            "4800:k",
            "5100:k",
            "5300:k",
            "5800:s",
            "6300:k",
            "6600:k",
            "6800:k",
            "7100:k",
            "7300:k",
            "7800:s",
            "8300:k",
            "9600:sequence-end-" + sequence.Name,
            "9600:sequence-start-" + sequence.Name,
            "9600:k",
            "9900:k",
            "10100:k",
            "10600:s",
            "11100:k",
            "11400:k",
            "11600:k",
            "11900:k",
            "12100:k",
            "12600:s",
            "13100:k",
            "14400:sequence-end-" + sequence.Name,
            "14400:sequence-start-" + sequence.Name,
            "14400:k",
            "14700:k",
            "14900:k",
            "15400:s",
            "15900:k",
            "16200:k",
            "16400:k",
            "16700:k",
            "16900:k",
            "17400:s",
            "17900:k",
            "19200:end-of-sequence-loop-" + sequence.Name,
            "19200:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.Duration.Should().Be(19200);
    }

    [Fact]
    public void SlowBeat_GetSequence4_ReturnExpected()
    {
        // arrange
        var sequence = SlowBeat.GetSequence4();
        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:k",
            "0200:k",
            "0400:k",
            "0800:s",
            "1200:k",
            "1400:k",
            "1600:k",
            "2400:s",
            "3200:k",
            "3400:k",
            "3600:k",
            "4000:s",
            "4400:k",
            "4600:k",
            "4800:k",
            "5220:k",
            "5620:s",
            "6400:sequence-end-" + sequence.Name,
            "6400:sequence-start-" + sequence.Name,
            "6400:k",
            "6600:k",
            "6800:k",
            "7200:s",
            "7600:k",
            "7800:k",
            "8000:k",
            "8800:s",
            "9600:k",
            "9800:k",
            "10000:k",
            "10400:s",
            "10800:k",
            "11000:k",
            "11200:k",
            "11620:k",
            "12020:s",
            "12800:sequence-end-" + sequence.Name,
            "12800:sequence-start-" + sequence.Name,
            "12800:k",
            "13000:k",
            "13200:k",
            "13600:s",
            "14000:k",
            "14200:k",
            "14400:k",
            "15200:s",
            "16000:k",
            "16200:k",
            "16400:k",
            "16800:s",
            "17200:k",
            "17400:k",
            "17600:k",
            "18020:k",
            "18420:s",
            "19200:sequence-end-" + sequence.Name,
            "19200:sequence-start-" + sequence.Name,
            "19200:k",
            "19400:k",
            "19600:k",
            "20000:s",
            "20400:k",
            "20600:k",
            "20800:k",
            "21600:s",
            "22400:k",
            "22600:k",
            "22800:k",
            "23200:s",
            "23600:k",
            "23800:k",
            "24000:k",
            "24420:k",
            "24820:s",
            "25600:end-of-sequence-loop-" + sequence.Name,
            "25600:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.Duration.Should().Be(25600);
    }

    [Fact]
    public void SlowBeat_GetSequence5_ReturnExpected()
    {
        // arrange
        var sequence = SlowBeat.GetSequence5();
        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:k",
            "0200:k",
            "0400:k",
            "0800:s",
            "1200:k",
            "1400:k",
            "1800:k",
            "2200:k",
            "2400:s",
            "3200:sequence-end-" + sequence.Name,
            "3200:sequence-start-" + sequence.Name,
            "3200:k",
            "3400:k",
            "3600:k",
            "4000:s",
            "4400:k",
            "4600:k",
            "5000:k",
            "5400:k",
            "5600:s",
            "6400:sequence-end-" + sequence.Name,
            "6400:sequence-start-" + sequence.Name,
            "6400:k",
            "6600:k",
            "6800:k",
            "7200:s",
            "7600:k",
            "7800:k",
            "8200:k",
            "8600:k",
            "8800:s",
            "9600:sequence-end-" + sequence.Name,
            "9600:sequence-start-" + sequence.Name,
            "9600:k",
            "9800:k",
            "10000:k",
            "10400:s",
            "10800:k",
            "11000:k",
            "11400:k",
            "11800:k",
            "12000:s",
            "12800:end-of-sequence-loop-" + sequence.Name,
            "12800:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.Duration.Should().Be(12800);
    }
}