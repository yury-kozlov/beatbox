using Beater;

namespace Tests;

/// <summary>
/// Covers <see cref="SequenceGenerator"/> scenarios where a sound is injected into a leader's followers
/// after the followers were already initialized (see <see cref="InjectionMap"/>).
/// </summary>
public class SequenceGeneratorInjectionMapTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void InjectedSoundWithLowerDelayThanFollower_IsGeneratedFirst_AndShiftsFollowPreviousDelay()
    {
        // arrange
        var kick = new Kick { Followers = [new Sound("follower") { Strategy = new FollowPreviousSoundStrategy { DelayAfterLeader = 100 } }] };
        var sequence = new SequenceDesign("test")
        {
            Leader = kick,
        };

        // simulate injecting a new sound into the followers after they were already initialized
        var injectedSound = new Sound("injected") { Strategy = new FollowLeaderStrategy { DelayAfterLeader = 50 } };
        kick.Followers.Add(injectedSound);

        string[] expected = [
            "0000:sequence-start-test",
            "0000:k",
            "0050:injected",
            "0150:follower", // 50 (injected's timestamp) + 100 (own delay) -- InjectionMap moved "injected" before "follower" so it becomes its previous sound
            "0150:sequence-end-test: 150 ms",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
    }

    [Fact]
    public void InjectedSoundWithDelayNotLowerThanFollower_IsNotReordered_FollowPreviousDelayIsUnaffected()
    {
        // arrange
        var followingSound = new Sound("follower") { Strategy = new FollowPreviousSoundStrategy { DelayAfterLeader = 100 } };
        var kick = new Kick { Followers = [followingSound] };
        var sequence = new SequenceDesign("test")
        {
            Leader = kick,
        };

        // injected sound's delay (200) is not lower than the follower's delay (100), so InjectionMap should not detect it as an injection
        var injectedSound = new Sound("injected") { Strategy = new FollowLeaderStrategy { DelayAfterLeader = 200 } };
        kick.Followers.Add(injectedSound);

        string[] expected = [
            "0000:sequence-start-test",
            "0000:k",
            "0100:follower", // stays relative to the leader -- "injected" was generated after it, so it's not its previous sound
            "0200:injected",
            "0200:sequence-end-test: 200 ms",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
    }

    [Fact]
    public void InjectedSoundPrecedesTwoFollowers_IsGeneratedOnce_AndAffectsFollowPreviousChain()
    {
        // arrange
        var firstFollower = new Sound("first") { Strategy = new FollowPreviousSoundStrategy { DelayAfterLeader = 100 } };
        var secondFollower = new Sound("second") { Strategy = new FollowLeaderStrategy { DelayAfterLeader = 250 } };
        var kick = new Kick { Followers = [firstFollower, secondFollower] };
        var sequence = new SequenceDesign("test")
        {
            Leader = kick,
        };

        // this single injected sound qualifies as "injected before" both followers (its delay is lower than both)
        var injectedSound = new Sound("injected") { Strategy = new FollowLeaderStrategy { DelayAfterLeader = 20 } };
        kick.Followers.Add(injectedSound);

        string[] expected = [
            "0000:sequence-start-test",
            "0000:k",
            "0020:injected", // generated only once, even though it precedes two followers
            "0120:first", // 20 (injected's timestamp) + 100 (own delay)
            "0250:second", // FollowLeaderStrategy is relative to the leader, so it's unaffected by generation order
            "0250:sequence-end-test: 250 ms",
        ];

        // act
        var actual = SequenceGenerator.Generate(sequence);
        var actualTimestamps = actual.GetTimestamps();

        // assert
        actualTimestamps.Should().BeExactSequence(expected);
    }
}
