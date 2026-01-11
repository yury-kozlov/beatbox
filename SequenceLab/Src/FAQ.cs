namespace Beater;

class FAQ
{
    public void ExamplePlay()
    {
        // how to play sequence repeatedly
        var player = new ConsoleKeyPlayer();
        player.PlayRepeated(James_Shinra_Poppin.GetSequence()).Wait();
    }

    public void ExampleAppendSequence()
    {
        // how to append one sequence to another
        var result = new Sequence();
        var slowBeat1 = Minimal.SlowBeat1WithoutRepeats();
        var slowBeat2 = Minimal.SlowBeat1WithoutRepeats();
        var slowBeat3 = Minimal.SlowBeat1WithoutRepeats();
        // note: duration property of each sequence should be predefined
        result.Append(slowBeat1).Append(slowBeat2).Append(slowBeat3);

        // play
        var player = new ConsoleKeyPlayer();
        player.PlayRepeated(result).Wait();
    }

    public void ExampleInjectSound()
    {
        // how to inject sounds into each repetition of a sequence
        var result = new Sequence();
        for (int i = 0; i < 4; i++)
        {
            var sequence = Minimal.SlowBeat1WithoutRepeats();
            sequence.Leader.FindByTag($"group-4")?.Followers.Add(
                new Sound("ts1", "ts9") // play ts1 + ts9 at the same time when group-4 begins to close
            );
            result.Append(sequence);
        }

        // play
        var player = new ConsoleKeyPlayer();
        player.PlayRepeated(result).Wait();
    }
}
