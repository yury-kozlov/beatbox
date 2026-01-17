namespace Beater;

public class PlayOnceStrategy : AbstractStrategy
{
    protected override List<Sound> GenerateSequenceFor(Sound sound, List<Sound>? previousMessages = null)
    {
        sound.Timestamp = DelayAfterLeader;

        var sequence = new List<Sound>() { sound };

        AddFollowers(sound, sequence);

        return sequence;
    }
}