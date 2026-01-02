using Beater;

namespace Tests;

public class SequenceSamplesTests
{
    [Fact]
    public void James_Shinra_Gritty_ReturnExpected()
    {
        // arrange
        var sequence = James_Shinra_Gritty.GetSequence();

        string[] expected = ["0000:no-sound", "0000:b1", "0350:b1", "0465:b2", "0675:b1", "1145:b1", "1385:b2", "1880:no-sound", "1880:b1", "2230:b1", "2345:b2", "2555:b1", "3025:b1", "3265:b2", "3760:no-sound", "3760:b1", "4110:b1", "4225:b2", "4435:b1", "4905:b1", "5145:b2", "5640:no-sound", "5640:b1", "5990:b1", "6105:b2", "6315:b1", "6785:b1", "7025:b2", "7520:no-sound"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void James_Shinra_Poppin_ReturnExpected()
    {
        // arrange
        var sequence = James_Shinra_Poppin.GetSequence();

        string[] expected = ["0000:no-sound", "0000:b1", "0450:b2", "0680:b1", "1365:b2", "2020:b1", "2280:b2", "2480:b1", "2940:b1", "3195:b2", "3280:b1", "3380:b1", "3620:no-sound", "3620:no-sound", "3620:b1", "4070:b2", "4300:b1", "4985:b2", "5640:b1", "5900:b2", "6100:b1", "6560:b1", "6815:b2", "6900:b1", "7000:b1", "7240:no-sound", "7240:no-sound", "7240:b1", "7690:b2", "7920:b1", "8605:b2", "9260:b1", "9520:b2", "9720:b1", "10180:b1", "10435:b2", "10520:b1", "10620:b1", "10860:no-sound", "10860:no-sound", "10860:b1", "11310:b2", "11540:b1", "12225:b2", "12880:b1", "13140:b2", "13340:b1", "13800:b1", "14055:b2", "14140:b1", "14240:b1", "14480:no-sound", "14480:no-sound"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Scsi9_Nebula_Hotel_ReturnExpected()
    {
        // arrange
        var sequence = Scsi9_Nebula_Hotel.GetSequence();

        string[] expected = ["0000:no-sound", "0000:b1", "0260:b1", "0490:b2", "1240:b1", "1460:b2", "1960:b1", "2210:b1", "2460:b2", "2860:b1", "3210:b1", "3460:b2", "3940:no-sound", "3940:b1", "4200:b1", "4430:b2", "5180:b1", "5400:b2", "5900:b1", "6150:b1", "6400:b2", "6800:b1", "7150:b1", "7400:b2", "7880:no-sound", "7880:b1", "8140:b1", "8370:b2", "9120:b1", "9340:b2", "9840:b1", "10090:b1", "10340:b2", "10740:b1", "11090:b1", "11340:b2", "11820:no-sound", "11820:b1", "12080:b1", "12310:b2", "13060:b1", "13280:b2", "13780:b1", "14030:b1", "14280:b2", "14680:b1", "15030:b1", "15280:b2", "15760:no-sound"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Otik_Clairvoyant_ReturnExpected()
    {
        // arrange
        var sequence = Otik_Clairvoyant.GetSequence();

        string[] expected = ["0000:no-sound", "0000:b1", "0360:b1", "0460:b2", "0700:b1", "0905:b1", "1005:b1", "1235:b1", "1335:b2", "1900:no-sound", "1900:b1", "2260:b1", "2360:b2", "2600:b1", "2805:b1", "2905:b1", "3135:b1", "3235:b2", "3800:no-sound", "3800:b1", "4160:b1", "4260:b2", "4500:b1", "4705:b1", "4805:b1", "5035:b1", "5135:b2", "5700:no-sound", "5700:b1", "6060:b1", "6160:b2", "6400:b1", "6605:b1", "6705:b1", "6935:b1", "7035:b2", "7600:no-sound"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Minimal_TechnoBeat1_ReturnExpected()
    {
        // arrange
        var sequence = Minimal.TechnoBeat1();

        string[] expected = ["0000:b1", "0150:ts1", "0230:ts1", "0310:no-sound", "0500:b1", "0650:ts1", "0730:ts1", "0810:no-sound", "1000:b1", "1150:ts1", "1230:ts1", "1310:no-sound", "1500:b1", "1500:ts2", "1650:ts1", "1730:ts1", "1750:b2", "1810:no-sound", "2000:b1", "2150:ts1", "2230:ts1", "2310:no-sound", "2500:b1", "2650:ts1", "2730:ts1", "2810:no-sound", "3000:b1", "3150:ts1", "3230:ts1", "3310:no-sound", "3500:b1", "3500:ts2", "3580:ts3", "3650:ts1", "3660:ts3", "3730:ts1", "3730:ts3", "3750:b2", "3790:ts3", "3810:no-sound", "3900:no-sound", "4000:b1", "4150:ts1", "4230:ts1", "4310:no-sound", "4500:b1", "4650:ts1", "4730:ts1", "4810:no-sound", "5000:b1", "5150:ts1", "5230:ts1", "5310:no-sound", "5500:b1", "5500:ts2", "5650:ts1", "5730:ts1", "5750:b2", "5810:no-sound", "6000:b1", "6150:ts1", "6230:ts1", "6310:no-sound", "6500:b1", "6650:ts1", "6730:ts1", "6810:no-sound", "7000:b1", "7150:ts1", "7230:ts1", "7310:no-sound", "7500:b1", "7500:ts2", "7580:ts3", "7650:ts1", "7660:ts3", "7730:ts1", "7730:ts3", "7750:b2", "7790:ts3", "7810:no-sound", "7900:no-sound", "8000:no-sound"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Minimal_TechnoBeat2_ReturnExpected()
    {
        // arrange
        var sequence = Minimal.TechnoBeat2();
        string[] expected = ["0000:b1", "0100:ts1", "0100:b1", "0180:ts1", "0200:b1", "0260:no-sound", "0300:no-sound", "0500:b1", "0550:ts3", "0600:ts2", "0630:ts3", "0700:ts2", "0700:ts3", "0760:ts3", "0800:no-sound", "0870:no-sound", "1000:b1", "1000:b2", "1500:b1", "1600:ts2", "2000:b1", "2100:ts1", "2100:b1", "2180:ts1", "2200:b1", "2260:no-sound", "2300:no-sound", "2500:b1", "2550:ts3", "2600:ts2", "2630:ts3", "2700:ts2", "2700:ts3", "2760:ts3", "2800:no-sound", "2870:no-sound", "3000:b1", "3000:b2", "3500:b1", "3600:ts2", "4000:b1", "4100:ts1", "4100:b1", "4180:ts1", "4200:b1", "4260:no-sound", "4300:no-sound", "4500:b1", "4550:ts3", "4600:ts2", "4630:ts3", "4700:ts2", "4700:ts3", "4760:ts3", "4800:no-sound", "4870:no-sound", "5000:b1", "5000:b2", "5500:b1", "5600:ts2", "6000:b1", "6100:ts1", "6100:b1", "6180:ts1", "6200:b1", "6260:no-sound", "6300:no-sound", "6500:b1", "6550:ts3", "6600:ts2", "6630:ts3", "6700:ts2", "6700:ts3", "6760:ts3", "6800:no-sound", "6870:no-sound", "7000:b1", "7000:b2", "7500:b1", "7600:ts2", "8000:no-sound"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Minimal_BrokenBeat1_ReturnExpected()
    {
        // arrange
        var sequence = Minimal.BrokenBeat1();
        string[] expected = ["0000:no-sound", "0000:b1", "0085:ts1", "0250:b1", "0500:b2", "0500:no-sound", "0585:ts1", "0710:ts2", "1085:b1", "1085:ts1", "1210:b1", "1335:no-sound", "1460:b2", "1585:ts1", "2000:no-sound", "2000:no-sound", "2000:b1", "2085:ts1", "2250:b1", "2500:b2", "2500:no-sound", "2585:ts1", "2710:ts2", "3085:b1", "3085:ts1", "3210:b1", "3335:no-sound", "3460:b2", "3585:ts1", "4000:no-sound", "4000:no-sound", "4000:b1", "4085:ts1", "4250:b1", "4500:b2", "4500:no-sound", "4585:ts1", "4710:ts2", "5085:b1", "5085:ts1", "5210:b1", "5335:no-sound", "5460:b2", "5585:ts1", "6000:no-sound", "6000:no-sound", "6000:b1", "6085:ts1", "6250:b1", "6500:b2", "6500:no-sound", "6585:ts1", "6710:ts2", "7085:b1", "7085:ts1", "7210:b1", "7335:no-sound", "7460:b2", "7585:ts1", "8000:no-sound", "8000:no-sound"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Minimal_SlowBeat1_ReturnExpected()
    {
        // arrange
        var sequence = Minimal.SlowBeat1();
        string[] expected = ["0000:no-sound", "0000:b1", "0330:b1", "0660:b2", "0660:no-sound", "1610:b1", "1940:b2", "2550:no-sound", "2550:b1", "2880:b1", "3210:b2", "3210:no-sound", "3710:b1", "4160:b1", "4490:b2", "4610:no-sound", "5100:no-sound", "5100:b1", "5430:b1", "5760:b2", "5760:no-sound", "6710:b1", "7040:b2", "7650:no-sound", "7650:b1", "7980:b1", "8310:b2", "8310:no-sound", "8810:b1", "9260:b1", "9590:b2", "9710:no-sound", "10200:no-sound"];

        // act
        var actual = sequence.Generate();
        var actualTimestamps = actual.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}");

        // assert
        actualTimestamps.Should().BeEquivalentTo(expected);
    }
}
