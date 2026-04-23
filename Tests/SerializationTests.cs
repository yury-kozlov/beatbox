using Beater;

namespace Tests;

public class SerializationTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void SerializeAndDeserialize_ReturnExpected()
    {
        // arrange
        var sourceSequence = new SequenceDesign("test")
        {
            Duration = 0, // duration will be calculated automatically
            Leader = new Metronome()
            {
                Followers = [
                    new Kick(),
                    new Kick { Strategy = new PlayOnceStrategy() { DelayAfterLeader = 300 }},
                    new Snare { Strategy = new PlayOnceStrategy() {DelayAfterLeader = 600 }},
                ]
            },
        };

        // act
        var json = sourceSequence.ToJson();
        var actual = SequenceDesign.FromJson(json);

        // assert
        actual.Should().NotBeNull();
        actual!.Name.Should().Be(sourceSequence.Name);

        var leader = actual.Leader;
        leader.Should().BeOfType<SequenceStart>();
        leader.Strategy.Should().BeOfType<FollowPreviousSoundStrategy>();
        leader.Followers.Should().HaveCount(1);

        var metronome = leader.Followers[0].Should().BeOfType<Metronome>().Subject;
        metronome.Strategy.Should().BeOfType<PlayOnceStrategy>();
        metronome.Followers.Should().HaveCount(3);
        metronome.Followers[0].Should().BeOfType<Kick>().Which.Strategy.Should().BeOfType<PlayOnceStrategy>()
            .Which.DelayAfterLeader.Should().Be(0);
        metronome.Followers[1].Should().BeOfType<Kick>().Which.Strategy.Should().BeOfType<PlayOnceStrategy>()
            .Which.DelayAfterLeader.Should().Be(300);
        metronome.Followers[2].Should().BeOfType<Snare>().Which.Strategy.Should().BeOfType<PlayOnceStrategy>()
            .Which.DelayAfterLeader.Should().Be(600);
    }

    [Fact]
    public void FromJsonTest()
    {
        // arrange
        var filePath = "json/test-sequence.json";

        // act
        var json = File.ReadAllText(filePath);
        var sequence = SequenceDesign.FromJson(json);

        // assert
        sequence.Should().NotBeNull();
        sequence!.Name.Should().Be("testJson1");

        var leader = sequence.Leader;
        leader.Should().BeOfType<SequenceStart>();
        leader.Strategy.Should().BeOfType<FollowPreviousSoundStrategy>();
        leader.Followers.Should().HaveCount(2);

        var metronome = leader.Followers.First().Should().BeOfType<Metronome>().Subject;
        var metronomeStrategy = metronome.Strategy.Should().BeOfType<RepeatStrategy>().Subject;
        metronomeStrategy.Count.Should().Be(4);
        metronomeStrategy.Interval.Should().Be(2100);
        metronomeStrategy.TrimIfExceedsParentLoop.Should().BeTrue();

        metronome.Followers.Should().HaveCount(3);
        metronome.Followers[0].Name.Should().Be("k");
        metronome.Followers[0].Strategy.Should().BeOfType<FollowPreviousSoundStrategy>()
            .Which.DelayAfterLeader.Should().Be(0);
        metronome.Followers[1].Name.Should().Be("k");
        metronome.Followers[1].Strategy.Should().BeOfType<FollowPreviousSoundStrategy>()
            .Which.DelayAfterLeader.Should().Be(700);
        metronome.Followers[2].Name.Should().Be("s");
        metronome.Followers[2].Strategy.Should().BeOfType<FollowPreviousSoundStrategy>()
            .Which.DelayAfterLeader.Should().Be(400);

        var sequenceEnd = leader.Followers[1];
        sequenceEnd.Should().BeOfType<SequenceEnd>();
        sequenceEnd.Strategy.Should().BeOfType<FollowPreviousSoundStrategy>();
    }
}
