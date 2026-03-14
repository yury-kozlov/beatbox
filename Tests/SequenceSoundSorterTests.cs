using Beater;

namespace Tests;

public class SequenceSoundSorterTests
{
    public static IEnumerable<object[]> GetTestData()
    {
        // already sorted:
        yield return new object[] {
            // input
            new Sequence()
            {
                new LoopEnd() { Timestamp = 0 },
                new SequenceStart("test") { Timestamp = 0 },
                new Kick() { Timestamp = 0 },
                new Snare() { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:end-of-loop",
                "0000:sequence-start-test",
                "0000:k",
                "0000:s",
            },
        };

        // "end-of-loop" should go before "sequence-start":
        yield return new object[] {
            // input
            new Sequence()
            {
                new SequenceStart("test") { Timestamp = 0 },
                new LoopEnd() { Timestamp = 0 },
                new Kick() { Timestamp = 0 },
                new Snare() { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:end-of-loop",
                "0000:sequence-start-test",
                "0000:k",
                "0000:s",
            },
        };

        // "end-of-loop" and "sequence-start" should go before regular sounds:
        yield return new object[] {
            // input
            new Sequence()
            {
                new Kick() { Timestamp = 0 },
                new Snare() { Timestamp = 0 },
                new SequenceStart("test") { Timestamp = 0 },
                new LoopEnd() { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:end-of-loop",
                "0000:sequence-start-test",
                "0000:k",
                "0000:s",
            },
        };

        // relative order of regular sounds should be preserved:
        yield return new object[] {
            // input
            new Sequence()
            {
                new Snare() { Timestamp = 0 },
                new SequenceStart("test") { Timestamp = 0 },
                new LoopEnd() { Timestamp = 0 },
                new Kick() { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:end-of-loop",
                "0000:sequence-start-test",
                "0000:s",
                "0000:k",
            },
        };

        // should sort by timestamp when all timestamps are different:
        yield return new object[] {
            // input
            new Sequence()
            {
                new Snare() { Timestamp = 3 },
                new SequenceStart("test") { Timestamp = 1 },
                new LoopEnd() { Timestamp = 4 },
                new Kick() { Timestamp = 2 },
            },
            // expected
            new string[] {
                "0001:sequence-start-test",
                "0002:k",
                "0003:s",
                "0004:end-of-loop",
            },
        };

        // "sequence-start" should go after "end-of-loop":
        yield return new object[] {
            // input
            new Sequence()
            {
                new Kick() { Timestamp = 0 },
                new Snare() { Timestamp = 0 },
                new LoopEnd() { Timestamp = 0 },
                new SequenceStart("test") { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:end-of-loop",
                "0000:sequence-start-test",
                "0000:k",
                "0000:s",
            },
        };

        // "end-of-loop" should go first even when placed last:
        yield return new object[] {
            // input
            new Sequence()
            {
                new Kick() { Timestamp = 0 },
                new Snare() { Timestamp = 0 },
                new SequenceStart("test") { Timestamp = 0 },
                new LoopEnd() { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:end-of-loop",
                "0000:sequence-start-test",
                "0000:k",
                "0000:s",
            },
        };

        // "sequence-end" should go after "end-of-loop":
        yield return new object[] {
            // input
            new Sequence()
            {
                new SequenceEnd(new SequenceDesign("test")) { Timestamp = 0 },
                new LoopEnd() { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:end-of-loop",
                "0000:sequence-end-test",
            },
        };

        // "sequence-end" should go after "metronome":
        yield return new object[] {
            // input
            new Sequence()
            {
                new SequenceEnd(new SequenceDesign("test")) { Timestamp = 0 },
                new Metronome() { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:metronome",
                "0000:sequence-end-test",
            },
        };

        // "sequence-end" should go before "sequence-start":
        yield return new object[] {
            // input
            new Sequence()
            {
                new SequenceStart("next") { Timestamp = 0 },
                new SequenceEnd(new SequenceDesign("prev")) { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:sequence-end-prev",
                "0000:sequence-start-next",
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void SortByTimestamp_ReturnExpected(Sequence sequence, string[] expected)
    {
        // act
        SequenceSoundSorter.SortByTimestamp(sequence);
        var actualTimestamps = sequence.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
    }
}
