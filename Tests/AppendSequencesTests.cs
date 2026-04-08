using Beater;

namespace Tests;

public class AppendSequencesTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void AppendSequences_3Sequences_WithoutExplicitDuration_ReturnJoined()
    {
        // arrange
        var kicks = new SequenceDesign("kicks")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Kick() { Followers = [new Kick() { DelayAfterLeader = 100 }] }
        };
        var snares = new SequenceDesign("snares")
        {
            Duration = 0,
            Leader = new Snare() { Followers = [new Snare() { DelayAfterLeader = 150 }] }
        };
        var hihats = new SequenceDesign("hihats")
        {
            Duration = 0,
            Leader = new Sound("h") { Followers = [new Sound("h") { DelayAfterLeader = 200 }] }
        };

        // act
        var sequence = new SequenceDesign("main");
        sequence.Append(kicks);
        sequence.Append(snares);
        sequence.Append(hihats);
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        string[] expected = [
            "0000:sequence-start-main",
            "0000:sequence-start-kicks",
            "0000:k",
            "0100:k",
            "0100:sequence-end-kicks",
            "0100:sequence-start-snares",
            "0100:s",
            "0250:s",
            "0250:sequence-end-snares",
            "0250:sequence-start-hihats",
            "0250:h",
            "0450:h",
            "0450:sequence-end-hihats",
            "0450:sequence-end-main",
        ];

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        kicks.AutoDuration.Should().Be(100);
        snares.AutoDuration.Should().Be(150);
        hihats.AutoDuration.Should().Be(200);
    }

    [Fact]
    public void AppendSequences_3Sounds_WithoutExplicitDuration_ReturnJoined()
    {
        // arrange
        var getSequence = (string name) => new SequenceDesign(name)
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Kick()
            {
                Followers = [new Snare() { DelayAfterLeader = 100 }, new Snare() { DelayAfterLeader = 150 }]
            }
        };
        var seq1 = getSequence("test1");
        var seq2 = getSequence("test2");

        // act
        var sequence = new SequenceDesign("main");
        sequence.Append(seq1);
        sequence.Append(seq2);
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        string[] expected = [
            "0000:sequence-start-main",
            "0000:sequence-start-test1",
            "0000:k",
            "0100:s",
            "0150:s",
            "0150:sequence-end-test1",
            "0150:sequence-start-test2",
            "0150:k",
            "0250:s",
            "0300:s",
            "0300:sequence-end-test2",
            "0300:sequence-end-main",
        ];

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
        seq1.AutoDuration.Should().Be(150);
        seq2.AutoDuration.Should().Be(150);
    }

    [Fact]
    public void AppendSequences_WithExplicitDuration_ReturnJoined()
    {
        // arrange
        var getSequence = (string name) => new SequenceDesign(name)
        {
            Duration = 500,
            Leader = new Kick()
            {
                Followers = [new Snare() { DelayAfterLeader = 100 }, new Snare() { DelayAfterLeader = 150 }]
            }
        };
        var seq1 = getSequence("test1");
        var seq2 = getSequence("test2");

        // act
        var sequence = new SequenceDesign("main");
        sequence.Append(seq1);
        sequence.Append(seq2);
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        string[] expected = [
            "0000:sequence-start-main",
            "0000:sequence-start-test1",
            "0000:k",
            "0100:s",
            "0150:s",
            "0500:sequence-end-test1",
            "0500:sequence-start-test2",
            "0500:k",
            "0600:s",
            "0650:s",
            "1000:sequence-end-test2",
            "1000:sequence-end-main",
        ];

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
    }

    [Fact]
    public void AppendSequences_SecondSequenceExceedsExplicitDurationOfFirst_Trim()
    {
        // this test assumes that if some sequence has explicit duration,
        // then any new sound added to it either as an individual sound or as part of another sequence
        // should not exceed total duration of any of it parent's repeat loops.

        // arrange
        var seq1 = new SequenceDesign("kicks")
        {
            Duration = 200,
            Leader = new Kick()
            {
                Followers = [new Kick() { DelayAfterLeader = 100 }, new Kick() { DelayAfterLeader = 150 }]
            }
        };
        var seq2 = new SequenceDesign("snares")
        {
            Duration = 300,
            DelayAfterLeader = 0, // must be 0 to be appended right after the first sequence, otherwise it will exceed first sequence duration and be trimmed
            Leader = new Snare()
            {
                Followers = [new Snare() { DelayAfterLeader = 160 }]
            }
        };
        seq1.Leader.Followers.Add(seq2.Leader);

        // act
        var sequence = new SequenceDesign("main")
        {
            Strategy = new RepeatStrategy
            {
                Count = 1,
                Interval = 100, // interval will be increased to 200 after appending, so the loop will be long enough to fit both sequences without trimming
            }
        };
        sequence.Append(seq1);
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        string[] expected = [
            "0000:sequence-start-main",
            "0000:sequence-start-kicks",
            "0000:k",
            "0100:k",
            "0150:k",
            "0200:sequence-end-kicks",
            "0200:sequence-start-snares",
            "0200:s",
            // "0360:s", // this one exceeds first sequence duration and is trimmed
            // "0500:sequence-end-snares",    // this one exceeds first sequence duration and is trimmed
            "0200:sequence-trimmed-snares",   // replaced sequence-end
            "0200:end-of-sequence-loop-main", // outer loop closing "main" sequence
            "0200:sequence-end-main",
        ];

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
    }

    [Fact]
    public void AppendTwoSequences_PlayOneAfterEachOther()
    {
        /// this test checks that if a sequence without <see cref="FollowPreviousSoundStrategy"/> is appended to another sequence
        /// they will be played after each other and not in parallel

        var trapezoid = new PrimitiveSequences.Trapezoid<Kick>()
        {
            XInterval = 500,
            YInterval = 700,
        };

        var square = new PrimitiveSequences.Square<Snare>()
        {
            Interval = 600,
            Strategy = new RepeatStrategy { Count = 1 },
        };

        var main = new SequenceDesign("main");
        main.Append(trapezoid);
        main.Append(square);
        var mainSequence = SequenceGenerator.Generate(main);

        // act
        var actualTimestamps = mainSequence.GetTimestamps();
        var expected = new string[] {
            "0000:sequence-start-main",
            "0000:sequence-start-trapezoid",
            "0000:k",
            "0500:k",
            "1200:k",
            "1900:k",
            "2400:sequence-end-trapezoid",

            "2400:sequence-start-square",
            "2400:s",
            "3000:s",
            "3600:s",
            "4200:s",
            "4800:end-of-loop",
            "4800:end-of-sequence-loop-square",
            "4800:sequence-end-square",
            "4800:sequence-end-main",
        };

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
    }
}