using Beater;

namespace Tests;

public class InjectionMapTests(ITestOutputHelper output) : TestBase(output)
{
    private static Sound CreateSound(string name, int delay, AbstractStrategy? strategy = null)
    {
        var sound = new Sound(name);
        if (strategy is not null)
        {
            sound.Strategy = strategy;
        }
        sound.DelayAfterLeader = delay;
        return sound;
    }

    [Fact]
    public void Constructor_NoSoundsAddedSinceInitialization_OrderedIsNull()
    {
        // arrange
        var soundA = CreateSound("a", 10);
        var soundB = CreateSound("b", 20);
        var source = new Sequence { soundA, soundB };
        source.InitialLength = source.Count;

        // act
        var map = new InjectionMap(source);

        // assert
        map.Ordered.Should().BeNull();
    }

    [Fact]
    public void Constructor_InjectedSoundHasLowerDelayThanYieldingSounds_OrdersInjectedSoundFirst()
    {
        // arrange
        var soundA = CreateSound("a", 100);
        var soundB = CreateSound("b", 200);
        var soundC = CreateSound("c", 50);
        var source = new Sequence { soundA, soundB, soundC };
        source.InitialLength = 2; // soundA, soundB were part of the initial sequence; soundC was injected later

        // act
        var map = new InjectionMap(source);

        // assert
        map.Ordered.Should().Equal(soundC, soundA, soundB);
    }

    [Fact]
    public void Constructor_InjectedSoundDelayNotLessThanAnyYieldingSound_OrderedIsNull()
    {
        // arrange
        var soundA = CreateSound("a", 10);
        var soundB = CreateSound("b", 20);
        var soundC = CreateSound("c", 100);
        var source = new Sequence { soundA, soundB, soundC };
        source.InitialLength = 2;

        // act
        var map = new InjectionMap(source);

        // assert
        map.Ordered.Should().BeNull();
    }

    [Fact]
    public void Constructor_YieldingSoundAfterFirstUsesFollowPreviousStrategy_SkipsInjectionCheckForIt()
    {
        // arrange
        var soundA = CreateSound("a", 200);
        var soundB = CreateSound("b", 300, new FollowPreviousSoundStrategy());
        var soundC = CreateSound("c", 150);
        var source = new Sequence { soundA, soundB, soundC };
        source.InitialLength = 2;

        // act
        var map = new InjectionMap(source);
        map.SetInjectionSequence(soundC, new Sequence());

        // assert
        map.GetInjectedSequence(soundA).Should().NotBeNull();
        map.GetInjectedSequence(soundB).Should().BeNull(); // soundB was never checked, so it's not aware of the injection
    }

    [Fact]
    public void GetInjectedSequence_BeforeSetInjectionSequenceIsCalled_ReturnsNull()
    {
        // arrange
        var soundA = CreateSound("a", 100);
        var soundB = CreateSound("b", 200);
        var soundC = CreateSound("c", 50);
        var source = new Sequence { soundA, soundB, soundC };
        source.InitialLength = 2;
        var map = new InjectionMap(source);

        // act
        var injectedSequence = map.GetInjectedSequence(soundA);

        // assert
        injectedSequence.Should().BeNull();
    }

    [Fact]
    public void SetInjectionSequence_InjectedSoundPrecedesMultipleYieldingSounds_MakesSequenceAvailableToAllOfThem()
    {
        // arrange
        var soundA = CreateSound("a", 100);
        var soundB = CreateSound("b", 200);
        var soundC = CreateSound("c", 50);
        var source = new Sequence { soundA, soundB, soundC };
        source.InitialLength = 2;
        var map = new InjectionMap(source);
        var generatedSequence = new Sequence { CreateSound("generated", 0) };

        // act
        map.SetInjectionSequence(soundC, generatedSequence);

        // assert
        map.GetInjectedSequence(soundA).Should().BeSameAs(generatedSequence);
        map.GetInjectedSequence(soundB).Should().BeSameAs(generatedSequence);
    }

    [Fact]
    public void GetInjectedSequence_SoundIsNotAYieldingSound_ReturnsNull()
    {
        // arrange
        var soundA = CreateSound("a", 100);
        var soundB = CreateSound("b", 200);
        var soundC = CreateSound("c", 50);
        var source = new Sequence { soundA, soundB, soundC };
        source.InitialLength = 2;
        var map = new InjectionMap(source);
        map.SetInjectionSequence(soundC, new Sequence());

        // act
        var injectedSequence = map.GetInjectedSequence(soundC);

        // assert
        injectedSequence.Should().BeNull();
    }
}
