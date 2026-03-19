using Beater;

namespace Tests;

public class SequenceSamplesTests
{
    [Fact]
    public void James_Shinra_Gritty_ReturnExpected()
    {
        // arrange
        var sequence = James_Shinra_Gritty.GetSequence();

        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:metronome",
            "0000:k",
            "0350:k",
            "0465:s",
            "0675:k",
            "1145:k",
            "1385:s",
            "1880:metronome",
            "1880:k",
            "2230:k",
            "2345:s",
            "2555:k",
            "3025:k",
            "3265:s",
            "3760:metronome",
            "3760:k",
            "4110:k",
            "4225:s",
            "4435:k",
            "4905:k",
            "5145:s",
            "5640:metronome",
            "5640:k",
            "5990:k",
            "6105:s",
            "6315:k",
            "6785:k",
            "7025:s",
            "7520:end-of-loop",
            "7520:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.AutoDuration.Should().Be(7520);
    }

    [Fact]
    public void James_Shinra_Poppin_ReturnExpected()
    {
        // arrange
        var sequence = James_Shinra_Poppin.GetSequence();

        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:metronome",
            "0000:k",
            "0450:s",
            "0680:k",
            "1365:s",
            "2020:k",
            "2280:s",
            "2480:k",
            "2940:k",
            "3195:s",
            "3280:k",
            "3380:k",
            "3620:end-of-loop",
            "3620:metronome",
            "3620:k",
            "4070:s",
            "4300:k",
            "4985:s",
            "5640:k",
            "5900:s",
            "6100:k",
            "6560:k",
            "6815:s",
            "6900:k",
            "7000:k",
            "7240:end-of-loop",
            "7240:metronome",
            "7240:k",
            "7690:s",
            "7920:k",
            "8605:s",
            "9260:k",
            "9520:s",
            "9720:k",
            "10180:k",
            "10435:s",
            "10520:k",
            "10620:k",
            "10860:end-of-loop",
            "10860:metronome",
            "10860:k",
            "11310:s",
            "11540:k",
            "12225:s",
            "12880:k",
            "13140:s",
            "13340:k",
            "13800:k",
            "14055:s",
            "14140:k",
            "14240:k",
            "14480:end-of-loop",
            "14480:end-of-loop",
            "14480:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.AutoDuration.Should().Be(14480);
    }

    [Fact]
    public void Scsi9_Nebula_Hotel_ReturnExpected()
    {
        // arrange
        var sequence = Scsi9_Nebula_Hotel.GetSequence();

        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:metronome",
             "0000:k",
             "0260:k",
             "0490:s",
             "1240:k",
             "1460:s",
             "1960:k",
             "2210:k",
             "2460:s",
             "2860:k",
             "3210:k",
             "3460:s",
             "3940:metronome",
             "3940:k",
             "4200:k",
             "4430:s",
             "5180:k",
             "5400:s",
             "5900:k",
             "6150:k",
             "6400:s",
             "6800:k",
             "7150:k",
             "7400:s",
             "7880:metronome",
             "7880:k",
             "8140:k",
             "8370:s",
             "9120:k",
             "9340:s",
             "9840:k",
             "10090:k",
             "10340:s",
             "10740:k",
             "11090:k",
             "11340:s",
             "11820:metronome",
             "11820:k",
             "12080:k",
             "12310:s",
             "13060:k",
             "13280:s",
             "13780:k",
             "14030:k",
             "14280:s",
             "14680:k",
             "15030:k",
             "15280:s",
             "15760:end-of-loop",
             "15760:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.AutoDuration.Should().Be(15760);
    }

    [Fact]
    public void Otik_Clairvoyant_ReturnExpected()
    {
        // arrange
        var sequence = Otik_Clairvoyant.GetSequence();

        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:metronome",
             "0000:k",
             "0360:k",
             "0460:s",
             "0700:k",
             "0905:k",
             "1005:k",
             "1235:k",
             "1335:s",
             "1900:metronome",
             "1900:k",
             "2260:k",
             "2360:s",
             "2600:k",
             "2805:k",
             "2905:k",
             "3135:k",
             "3235:s",
             "3800:metronome",
             "3800:k",
             "4160:k",
             "4260:s",
             "4500:k",
             "4705:k",
             "4805:k",
             "5035:k",
             "5135:s",
             "5700:metronome",
             "5700:k",
             "6060:k",
             "6160:s",
             "6400:k",
             "6605:k",
             "6705:k",
             "6935:k",
             "7035:s",
             "7600:end-of-loop",
             "7600:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.AutoDuration.Should().Be(7600);
    }

    [Fact]
    public void Minimal_TechnoBeat1_ReturnExpected()
    {
        // arrange
        var sequence = Minimal.TechnoBeat1();

        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
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
            "8000:end-of-loop",
            "8000:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.AutoDuration.Should().Be(8000);
    }

    [Fact]
    public void Minimal_TechnoBeat2_ReturnExpected()
    {
        // arrange
        var sequence = Minimal.TechnoBeat2();
        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:k",
            "0100:ts1",
            "0100:k",
            "0180:ts1",
            "0200:k",
            "0260:end-of-loop",
            "0300:end-of-loop",
            "0500:k",
            "0550:ts3",
            "0600:ts2",
            "0630:ts3",
            "0700:ts2",
            "0700:ts3",
            "0760:ts3",
            "0800:end-of-loop",
            "0870:end-of-loop",
            "1000:k",
            "1000:s",
            "1500:k",
            "1600:ts2",
            "2000:k",
            "2100:ts1",
            "2100:k",
            "2180:ts1",
            "2200:k",
            "2260:end-of-loop",
            "2300:end-of-loop",
            "2500:k",
            "2550:ts3",
            "2600:ts2",
            "2630:ts3",
            "2700:ts2",
            "2700:ts3",
            "2760:ts3",
            "2800:end-of-loop",
            "2870:end-of-loop",
            "3000:k",
            "3000:s",
            "3500:k",
            "3600:ts2",
            "4000:k",
            "4100:ts1",
            "4100:k",
            "4180:ts1",
            "4200:k",
            "4260:end-of-loop",
            "4300:end-of-loop",
            "4500:k",
            "4550:ts3",
            "4600:ts2",
            "4630:ts3",
            "4700:ts2",
            "4700:ts3",
            "4760:ts3",
            "4800:end-of-loop",
            "4870:end-of-loop",
            "5000:k",
            "5000:s",
            "5500:k",
            "5600:ts2",
            "6000:k",
            "6100:ts1",
            "6100:k",
            "6180:ts1",
            "6200:k",
            "6260:end-of-loop",
            "6300:end-of-loop",
            "6500:k",
            "6550:ts3",
            "6600:ts2",
            "6630:ts3",
            "6700:ts2",
            "6700:ts3",
            "6760:ts3",
            "6800:end-of-loop",
            "6870:end-of-loop",
            "7000:k",
            "7000:s",
            "7500:k",
            "7600:ts2",
            "8000:end-of-loop",
            "8000:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.AutoDuration.Should().Be(8000);
    }

    [Fact]
    public void Minimal_TechnoBeat3_ReturnExpected()
    {
        // arrange
        var sequence = Minimal.TechnoBeat3();
        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:k",
            "0500:k",
            "0500:s",
            "1000:k",
            "1300:s",
            "1500:k",
            "2000:end-of-loop",
            "2000:sequence-end-TechnoBeat3",
            "2000:sequence-start-TechnoBeat3",
            "2000:k",
            "2500:k",
            "2500:s",
            "3000:k",
            "3300:s",
            "3500:k",
            "4000:end-of-loop",
            "4000:sequence-end-TechnoBeat3",
            "4000:sequence-start-TechnoBeat3",
            "4000:k",
            "4500:k",
            "4500:s",
            "5000:k",
            "5300:s",
            "5500:k",
            "6000:end-of-loop",
            "6000:sequence-end-TechnoBeat3",
            "6000:sequence-start-TechnoBeat3",
            "6000:k",
            "6500:k",
            "6500:s",
            "7000:k",
            "7300:s",
            "7500:k",
            "8000:end-of-loop",
            "8000:end-of-sequence-loop",
            "8000:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.Duration.Should().Be(8000);
    }

    [Fact]
    public void Minimal_BrokenBeat1_ReturnExpected()
    {
        // arrange
        var sequence = Minimal.BrokenBeat1();
        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:metronome",
            "0000:k",
            "0085:ts1",
            "0250:k",
            "0500:end-of-loop",
            "0500:s",
            "0585:ts1",
            "0710:ts2",
            "1085:k",
            "1085:ts1",
            "1210:k",
            "1335:end-of-loop",
            "1460:s",
            "1585:ts1",
            "2000:end-of-loop",
            "2000:metronome",
            "2000:k",
            "2085:ts1",
            "2250:k",
            "2500:end-of-loop",
            "2500:s",
            "2585:ts1",
            "2710:ts2",
            "3085:k",
            "3085:ts1",
            "3210:k",
            "3335:end-of-loop",
            "3460:s",
            "3585:ts1",
            "4000:end-of-loop",
            "4000:metronome",
            "4000:k",
            "4085:ts1",
            "4250:k",
            "4500:end-of-loop",
            "4500:s",
            "4585:ts1",
            "4710:ts2",
            "5085:k",
            "5085:ts1",
            "5210:k",
            "5335:end-of-loop",
            "5460:s",
            "5585:ts1",
            "6000:end-of-loop",
            "6000:metronome",
            "6000:k",
            "6085:ts1",
            "6250:k",
            "6500:end-of-loop",
            "6500:s",
            "6585:ts1",
            "6710:ts2",
            "7085:k",
            "7085:ts1",
            "7210:k",
            "7335:end-of-loop",
            "7460:s",
            "7585:ts1",
            "8000:end-of-loop",
            "8000:end-of-loop",
            "8000:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.AutoDuration.Should().Be(8000);
    }

    [Fact]
    public void Minimal_SlowBeat1_ReturnExpected()
    {
        // arrange
        var sequence = Minimal.SlowBeat1WithRepeats();
        string[] expected = [
            "0000:sequence-start-" + sequence.Name,
            "0000:metronome",
            "0000:k",
            "0330:k",
            "0660:end-of-loop",
            "0660:s",
            "1610:k",
            "1940:s",
            "2550:metronome",
            "2550:k",
            "2880:k",
            "3210:end-of-loop",
            "3210:s",
            "3710:k",
            "4160:k",
            "4490:s",
            "4610:end-of-loop",
            "5100:metronome",
            "5100:k",
            "5430:k",
            "5760:end-of-loop",
            "5760:s",
            "6710:k",
            "7040:s",
            "7650:metronome",
            "7650:k",
            "7980:k",
            "8310:end-of-loop",
            "8310:s",
            "8810:k",
            "9260:k",
            "9590:s",
            "9710:end-of-loop",
            "10200:end-of-loop",
            "10200:sequence-end-" + sequence.Name,
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        sequence.AutoDuration.Should().Be(10200);
    }
}
