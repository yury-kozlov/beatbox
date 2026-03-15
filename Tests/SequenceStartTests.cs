using Beater;

namespace Tests;

public class SequenceStartTests
{
    [Fact]
    public void GetSequenceEnd_WithExplicitDuration_ReturnsSequenceEndWithCorrectDelay()
    {
        // arrange
        var sequence = new SequenceDesign("test")
        {
            Duration = 1000,
            Leader = new Kick(),
        };
        var sequenceStart = (SequenceStart)sequence.Leader;

        // act
        var sequenceEnd = sequenceStart.GetSequenceEnd();

        // assert
        sequenceEnd.Should().NotBeNull();
        sequenceEnd.Strategy.DelayAfterLeader.Should().Be(sequence.Duration);
    }

    [Fact]
    public void GetSequenceEnd_AfterAppend_ReturnsLastSequenceEnd()
    {
        // arrange
        var firstDuration = 500;
        var secondDuration = 100;
        var first = new SequenceDesign("first")
        {
            Duration = firstDuration,
            Leader = new Kick(),
        };
        
        var second = new SequenceDesign("second")
        {
            Duration = secondDuration,
            Leader = new Snare(),
        };

        first.Append(second);

        var sequenceStart = (SequenceStart)first.Leader;

        // act
        var sequenceEnd = sequenceStart.GetSequenceEnd();
        var secondSequenceEnd = ((SequenceStart)second.Leader).GetSequenceEnd();

        // assert - should be the last SequenceEnd (from "first"), not "second"
        sequenceEnd.FriendlyName.Should().Be("sequence-end-first");
        secondSequenceEnd.FriendlyName.Should().Be("sequence-end-second");

        // when appending to a sequence, it's duration is increased by duration of the added sequence
        // for this reason, the end of the original sequence should also be increased:
        sequenceEnd.Strategy.DelayAfterLeader.Should().Be(firstDuration + secondDuration);
    }
}
