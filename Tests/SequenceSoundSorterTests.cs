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

        // "sequence-end" should go before "sequence-start" when sequences are different:
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

        // "sequence-end" should go after "sequence-start" in same sequence:
        yield return new object[] {
            // input
            new Sequence()
            {
                new SequenceStart("test") { Timestamp = 0 },
                new SequenceEnd() { Timestamp = 0 },
                new Kick() { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:sequence-start-test",
                "0000:k",
                "0000:sequence-end",
            },
        };

        // "sequence-end" should go after regular sounds of same sequence:
        yield return new object[] {
            // input
            new Sequence()
            {
                new SequenceEnd(new SequenceDesign("test")) { Timestamp = 0, Sequence = SharedSeq },
                new Kick() { Timestamp = 0, Sequence = SharedSeq },
            },
            // expected
            new string[] {
                "0000:k",
                "0000:sequence-end-test",
            },
        };

        // "sequence-end" should go before regular sounds of a different sequence:
        yield return new object[] {
            // input
            new Sequence()
            {
                new Kick() { Timestamp = 0, Sequence = SharedSeq },
                new SequenceEnd(new SequenceDesign("other")) { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:sequence-end-other",
                "0000:k",
            },
        };

        // sounds with different iterations at the same timestamp should be ordered by iteration:
        yield return new object[] {
            // input
            new Sequence()
            {
                new Kick() { Timestamp = 0, Iteration = "2" },
                new SequenceEnd(new SequenceDesign("test")) { Timestamp = 0, Iteration = "1" },
                new SequenceStart("test") { Timestamp = 0, Iteration = "2" },
            },
            // expected
            new string[] {
                "0000:sequence-end-test",  // iteration 1
                "0000:sequence-start-test",// iteration 2
                "0000:k",                  // iteration 2 (after sequence-start of same iteration)
            },
        };

        // inner sound (iteration "2.1") should follow its SequenceStart (iteration "2"), not precede it:
        yield return new object[] {
            // input
            new Sequence()
            {
                new Kick() { Timestamp = 0, Iteration = "2.1" },
                new SequenceStart("loop") { Timestamp = 0, Iteration = "2" },
            },
            // expected
            new string[] {
                "0000:sequence-start-loop", // "2" < "2.1" — outer start precedes its own followers
                "0000:k",                   // "2.1"
            },
        };

        // full outer-iteration: inner loop end → sequence end → next sequence start → first inner sound:
        yield return new object[] {
            // input (deliberately shuffled)
            new Sequence()
            {
                new Kick() { Timestamp = 0, Iteration = "2.1" },
                new SequenceStart("loop") { Timestamp = 0, Iteration = "2" },
                new SequenceEnd(new SequenceDesign("loop")) { Timestamp = 0, Iteration = "1" },
                new LoopEnd() { Timestamp = 0, Iteration = "1" },
            },
            // expected
            new string[] {
                "0000:end-of-loop",          // "1" — inner loop closes
                "0000:sequence-end-loop",    // "1" — outer iteration 1 ends (after end-of-loop)
                "0000:sequence-start-loop",  // "2" — outer iteration 2 begins
                "0000:k",                    // "2.1" — first sound of outer iteration 2
            },
        };

        // last sound of previous outer iteration vs first sound of next outer iteration:
        yield return new object[] {
            // input
            new Sequence()
            {
                new Kick() { Timestamp = 0, Iteration = "2.1" },
                new Sound("ts") { Timestamp = 0, Iteration = "1.3" },
                new SequenceStart("loop") { Timestamp = 0, Iteration = "2" },
            },
            // expected
            new string[] {
                "0000:ts",                   // "1.3" — tail of outer iteration 1 (1 < 2)
                "0000:sequence-start-loop",  // "2"   — outer iteration 2 starts
                "0000:k",                    // "2.1" — first sound of outer iteration 2
            },
        };

        // three-level nesting: outer start → middle repeat → innermost sound, all at same timestamp:
        yield return new object[] {
            // input
            new Sequence()
            {
                new Kick() { Timestamp = 0, Iteration = "2.1.1" },
                new Sound("metro") { Timestamp = 0, Iteration = "2.1" },
                new SequenceStart("loop") { Timestamp = 0, Iteration = "2" },
            },
            // expected
            new string[] {
                "0000:sequence-start-loop", // "2"     — outermost
                "0000:metro",               // "2.1"   — one level in
                "0000:k",                   // "2.1.1" — innermost
            },
        };

        // "end-of-loop" should go before "end-of-sequence-loop":
        yield return new object[] {
            // input (sequence loop placed first)
            new Sequence()
            {
                new LoopEnd(new SequenceStart("seq")) { Timestamp = 0 },
                new LoopEnd() { Timestamp = 0 },
            },
            // expected
            new string[] {
                "0000:end-of-loop",
                "0000:end-of-sequence-loop",
            },
        };

        // "end-of-loop" should go before "end-of-sequence-loop" even when iteration path of sequence loop sorts earlier:
        yield return new object[] {
            // input — sequence loop has iteration "1" (parent path), regular loop has "1.1" (child path);
            // without special handling the iteration-based comparison would incorrectly put sequence loop first
            new Sequence()
            {
                new LoopEnd(new SequenceStart("seq")) { Timestamp = 0, Iteration = "1" },
                new LoopEnd() { Timestamp = 0, Iteration = "1.1" },
            },
            // expected — type wins over iteration: regular loop end before sequence loop end
            new string[] {
                "0000:end-of-loop",           // "1.1" — inner regular loop
                "0000:end-of-sequence-loop",  // "1"   — outer sequence loop
            },
        };
    }

    private static SequenceDesign SharedSeq = new SequenceDesign("shared-sequence");

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void SortByTimestamp_ReturnExpected(Sequence sequence, string[] expected)
    {
        // act
        var sorted = SequenceSoundSorter.SortByTimestamp(sequence);
        var actualTimestamps = sorted.GetTimestamps();

        // assert
        actualTimestamps.Should().ContainInOrder(expected);
    }
}
