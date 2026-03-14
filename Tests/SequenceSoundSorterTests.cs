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
