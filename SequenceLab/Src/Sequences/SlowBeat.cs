namespace Beater;

public class SlowBeat
{
    internal static Sequence WithMovingSoundBetweenGroups()
    {
        var sequence = new Sequence();
        for (int i = 4; i > 0; i--)
        {
            var slowBeat = Minimal.SlowBeat1WithoutRepeats();
            slowBeat.Leader.FindByTag($"group-{i}")?.Followers.Add(
                new Sound("ts1", "ts9") // play ts1 + ts9 at the same time when group-X begins to close
            );
            sequence.Append(slowBeat);
        }

        return sequence;
    }
}
