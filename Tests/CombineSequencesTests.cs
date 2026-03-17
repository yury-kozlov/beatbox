using Beater;

namespace Tests;

public class CombineSequencesTests
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
        actual.GetTimestamps().Should().BeEquivalentTo(expected);
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
        actualTimestamps.Should().BeEquivalentTo([
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
    public void Combine_ThreeSequences_AllPlayInParallel()
    {
        // arrange
        var kicks = new SequenceDesign("kicks") { Leader = new Kick { Followers = [new Kick() { DelayAfterLeader = 100 }] } };
        var snares = new SequenceDesign("snares") { Leader = new Snare { Followers = [new Snare() { DelayAfterLeader = 150 }] } };
        var hihats = new SequenceDesign("hihats") { Leader = new Sound("h") { Followers = [new Sound("h") { DelayAfterLeader = 200 }] }};

        var main = new SequenceDesign("main");
        main.Combine(kicks);
        main.Combine(snares);
        main.Combine(hihats);

        // act
        var actual = SequenceGenerator.Generate(main);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeEquivalentTo([
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
        actualTimestamps.Should().BeEquivalentTo([
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
