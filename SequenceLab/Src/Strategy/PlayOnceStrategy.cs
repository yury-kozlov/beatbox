namespace Beater;

public class PlayOnceStrategy : AbstractStrategy
{
    protected override Sequence GenerateSequenceFor(Sound sound, Sequence? previousSounds = null)
    {
        sound.Timestamp = DelayAfterLeader;

        var sequence = new Sequence() { sound };

        AddFollowers(sound, sequence);

        return sequence;
    }
}