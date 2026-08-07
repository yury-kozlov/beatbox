using Beater;

namespace Tests;

public class SequenceDebuggerDisplayTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void Get_EmptySequence_ReturnsEmptyString()
    {
        // arrange
        var sequence = new FollowersDesign();

        // act
        var result = SequenceDebuggerDisplay.Get(sequence);

        // assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Get_SoundWithoutDelay_ReturnsNameOnly()
    {
        // arrange
        var sequence = new FollowersDesign { new Kick() };

        // act
        var result = SequenceDebuggerDisplay.Get(sequence);

        // assert
        result.Should().Be("k");
    }

    [Fact]
    public void Get_SoundWithDelay_ReturnsDelayAndName()
    {
        // arrange
        var sequence = new FollowersDesign { new Kick { DelayAfterLeader = 100 } };

        // act
        var result = SequenceDebuggerDisplay.Get(sequence);

        // assert
        result.Should().Be("100 k");
    }

    [Fact]
    public void Get_MultipleSounds_JoinsWithCommaAndSpace()
    {
        // arrange
        var sequence = new FollowersDesign { new Kick(), new Snare { DelayAfterLeader = 50 } };

        // act
        var result = SequenceDebuggerDisplay.Get(sequence);

        // assert
        result.Should().Be("k, 50 s");
    }

    [Fact]
    public void Get_SequenceContainsNoSound_ExcludesItFromResult()
    {
        // arrange
        var sequence = new FollowersDesign { new Kick(), new NoSound() };

        // act
        var result = SequenceDebuggerDisplay.Get(sequence);

        // assert
        result.Should().Be("k");
    }

    [Fact]
    public void Get_SoundHasFollowers_IncludesFollowersRecursively()
    {
        // arrange
        var leader = new Kick();
        leader.WithFollower(new Snare { DelayAfterLeader = 50 });
        var sequence = new FollowersDesign { leader };

        // act
        var result = SequenceDebuggerDisplay.Get(sequence);

        // assert
        result.Should().Be("k, 50 s");
    }

    [Fact]
    public void Get_FollowersNestedBeyondMaxRecursion_StopsDescending()
    {
        // arrange
        var leader = new Sound("sound-0");
        var current = leader;
        for (var i = 1; i <= 11; i++)
        {
            var next = new Sound($"sound-{i}");
            current.WithFollower(next);
            current = next;
        }
        var sequence = new FollowersDesign { leader };

        // act
        var result = SequenceDebuggerDisplay.Get(sequence);

        // assert
        result.Should().Be("sound-0, sound-1, sound-2, sound-3, sound-4, sound-5, sound-6, sound-7, sound-8, sound-9");
    }
}
