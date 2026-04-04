using Beater;

namespace Tests;

public class CombineSequencesTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void Combine_FirstSequenceOnly_BehavesLikeAppend()
    {
        // first sequence is empty, Combine acts like Append: no need for parallel

        // arrange
        var main = new SequenceDesign("main");
        var kicks = new SequenceDesign("kicks") { Leader = new Kick { Followers = [new Kick { DelayAfterLeader = 100 }] } };
        main.Combine(kicks);

        // act
        var actual = SequenceGenerator.Generate(main);

        // assert
        string[] expected = [
            "0000:sequence-start-main",
            "0000:sequence-start-kicks",
            "0000:k",
            "0100:k",
            "0100:sequence-end-kicks",
            "0100:sequence-end-main",
        ];
        actual.GetTimestamps().Should().BeExactSequence(expected);
    }

    [Fact]
    public void Combine_TwoSequences_PlayInParallel()
    {
        // second Combine converts FollowPreviousSoundStrategy → PlayOnceStrategy so both sequences start at the same time

        // arrange
        var main = new SequenceDesign("main");
        var kicks = new SequenceDesign("kicks") { Leader = new Kick { Followers = [new Kick { DelayAfterLeader = 100 }] } };
        var snares = new SequenceDesign("snares") { Leader = new Snare { Followers = [new Snare { DelayAfterLeader = 150 }] } };
        main.Combine(kicks);
        main.Combine(snares);

        // act
        var actual = SequenceGenerator.Generate(main);
        var actualTimestamps = actual.GetTimestamps();

        // assert — both sequence-starts appear at ts=0
        actualTimestamps.Should().BeExactSequence([
            "0000:sequence-start-main",
            "0000:sequence-start-kicks",
            "0000:k",
            "0000:sequence-start-snares",
            "0000:s",
            "0100:k",
            "0100:sequence-end-kicks",
            "0150:s",
            "0150:sequence-end-snares",
            "0150:sequence-end-main",
        ]);
    }

    [Fact]
    public void Combine_TwoSequencesWithRepeat_PlayInParallel()
    {
        // arrange
        var kicks = new SequenceDesign("kicks")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Interval = 500, Count = 4 } },
        };

        var snares = new SequenceDesign("snares")
        {
            Leader = new Metronome
            {
                Strategy = new RepeatStrategy { Interval = 400, Count = 4 },
                Followers = [new Snare { Strategy = new PlayOnceStrategy { PlayEveryX = 2, DelayAfterLeader = 100 } }],
            },
        };

        var main = new SequenceDesign("main") { Strategy = new RepeatStrategy { Count = 4 } };
        main.Combine(kicks);
        main.Combine(snares);

        // act
        var actual = SequenceGenerator.Generate(main);
        var actualTimestamps = actual.GetTimestamps();
        string[] expected = [
            "0000:sequence-start-main",
            "0000:sequence-start-kicks",
            "0000:sequence-start-snares",
            "0000:k",
            "0000:metronome",
            "0400:metronome",
            "0500:k",
            "0500:s",
            "0800:metronome",
            "1000:k",
            "1200:metronome",
            "1300:s",
            "1500:k",
            "1600:end-of-loop",
            "1600:sequence-end-snares",
            "2000:end-of-loop",
            "2000:sequence-end-kicks",
            "2000:sequence-end-main",
            "2000:sequence-start-main",
            "2000:sequence-start-kicks",
            "2000:sequence-start-snares",
            "2000:metronome",
            "2000:k",
            "2400:metronome",
            "2500:k",
            "2500:s",
            "2800:metronome",
            "3000:k",
            "3200:metronome",
            "3300:s",
            "3500:k",
            "3600:end-of-loop",
            "3600:sequence-end-snares",
            "4000:end-of-loop",
            "4000:sequence-end-kicks",
            "4000:sequence-end-main",
            "4000:sequence-start-main",
            "4000:sequence-start-kicks",
            "4000:sequence-start-snares",
            "4000:metronome",
            "4000:k",
            "4400:metronome",
            "4500:k",
            "4500:s",
            "4800:metronome",
            "5000:k",
            "5200:metronome",
            "5300:s",
            "5500:k",
            "5600:end-of-loop",
            "5600:sequence-end-snares",
            "6000:end-of-loop",
            "6000:sequence-end-kicks",
            "6000:sequence-end-main",
            "6000:sequence-start-main",
            "6000:sequence-start-kicks",
            "6000:sequence-start-snares",
            "6000:k",
            "6000:metronome",
            "6400:metronome",
            "6500:k",
            "6500:s",
            "6800:metronome",
            "7000:k",
            "7200:metronome",
            "7300:s",
            "7500:k",
            "7600:end-of-loop",
            "7600:sequence-end-snares",
            "8000:end-of-loop",
            "8000:end-of-sequence-loop",
            "8000:sequence-end-kicks",
            "8000:sequence-end-main",
        ];

        // assert
        main.Duration.Should().Be(8000);
        actualTimestamps.Should().BeExactSequence(expected);
    }

    [Fact]
    public void Combine_TwoSequencesWithRepeat_SecondSequenceIsLonger_PlayInParallel()
    {
        // arrange
        var kicks = new SequenceDesign("kicks")
        {
            Strategy = new RepeatStrategy { Count = 2 },
            Leader = new Kick { Strategy = new RepeatStrategy { Interval = 100, Count = 3 } },
        };

        var snares = new SequenceDesign("snares")
        {
            Strategy = new RepeatStrategy { Count = 2 },
            Leader = new Snare { Strategy = new RepeatStrategy { Interval = 1000, Count = 2} },
        };

        var main = new SequenceDesign("main");
        main.Combine(kicks);
        main.Combine(snares);

        // act
        var actual = SequenceGenerator.Generate(main);
        var actualTimestamps = actual.GetTimestamps();
        string[] expected = [
            "0000:sequence-start-main",
            "0000:sequence-start-kicks",
            "0000:sequence-start-snares",
            "0000:k",
            "0000:s",
            "0100:k",
            "0200:k",
            "0300:end-of-loop",
            "0300:sequence-end-kicks",
            "0300:sequence-start-kicks",
            "0300:k",
            "0400:k",
            "0500:k",
            "0600:end-of-loop",
            "0600:end-of-sequence-loop",
            "0600:sequence-end-kicks",
            "1000:s",
            "2000:end-of-loop",
            "2000:sequence-end-snares",
            "2000:sequence-start-snares",
            "2000:s",
            "3000:s",
            "4000:end-of-loop",
            "4000:end-of-sequence-loop",
            "4000:sequence-end-snares",
            "4000:sequence-end-main",
        ];

        // assert
        main.Duration.Should().Be(4000);
        actualTimestamps.Should().BeExactSequence(expected);
    }

    [Fact]
    public void Combine_ThreeSequences_AllPlayInParallel()
    {
        // arrange
        var kicks = new SequenceDesign("kicks") { Leader = new Kick { Followers = [new Kick() { DelayAfterLeader = 100 }] } };
        var snares = new SequenceDesign("snares") { Leader = new Snare { Followers = [new Snare() { DelayAfterLeader = 150 }] } };
        var hihats = new SequenceDesign("hihats") { Leader = new Sound("h") { Followers = [new Sound("h") { DelayAfterLeader = 200 }] } };

        var main = new SequenceDesign("main");
        main.Combine(kicks);
        main.Combine(snares);
        main.Combine(hihats);

        // act
        var actual = SequenceGenerator.Generate(main);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence([
            "0000:sequence-start-main",
            "0000:sequence-start-kicks",
            "0000:k",
            "0000:sequence-start-snares",
            "0000:s",
            "0000:sequence-start-hihats",
            "0000:h",
            "0100:k",
            "0100:sequence-end-kicks",
            "0150:s",
            "0150:sequence-end-snares",
            "0200:h",
            "0200:sequence-end-hihats",
            "0200:sequence-end-main",
        ]);
    }

    [Fact]
    public void Combine_Duration_IsMaxOfBoth()
    {
        // arrange
        var kicks = new SequenceDesign("kicks") { Duration = 2000, Leader = new Kick() };
        var snares = new SequenceDesign("snares") { Duration = 3000, Leader = new Snare() };

        var main = new SequenceDesign("main");

        // act
        main.Combine(kicks);
        main.Combine(snares);

        // assert
        main.Duration.Should().Be(3000);
    }

    [Fact]
    public void Combine_WithRepeatStrategy_PreservesStrategy()
    {
        // sequences with an explicit strategy (not FollowPreviousSoundStrategy) should not have their strategy replaced

        // arrange
        var kicks = new SequenceDesign("kicks") { Leader = new Kick { Strategy = new RepeatStrategy { Interval = 500, Count = 4 } } };
        var snares = new SequenceDesign("snares") { Leader = new Snare { Strategy = new RepeatStrategy { Interval = 600, Count = 4 } } };

        var main = new SequenceDesign("main");
        main.Combine(kicks);
        main.Combine(snares);

        // act
        var actual = SequenceGenerator.Generate(main);
        var actualTimestamps = actual.GetTimestamps();

        // assert — both sequences play at ts=0 with their own repeat intervals preserved
        actualTimestamps.Should().BeExactSequence([
            "0000:sequence-start-main",
            "0000:sequence-start-kicks",
            "0000:k",
            "0000:sequence-start-snares",
            "0000:s",
            "0500:k",
            "0600:s",
            "1000:k",
            "1200:s",
            "1500:k",
            "1800:s",
            "2000:end-of-loop",
            "2000:sequence-end-kicks",
            "2400:end-of-loop",
            "2400:sequence-end-snares",
            "2400:sequence-end-main",
        ]);
    }
}
