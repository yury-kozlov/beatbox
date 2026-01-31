namespace Beater;

class Examples
{
    public void Play()
    {
        // how to play sequence repeatedly
        var player = new ConsoleKeyPlayer();
        player.PlayRepeated(James_Shinra_Poppin.GetSequence()).Wait();
    }

    public void AppendSequence()
    {
        // how to append one sequence to another
        var result = new MiniSequence();
        var sequence1 = Minimal.SlowBeat1WithoutRepeats();
        var sequence2 = Minimal.SlowBeat1WithoutRepeats();
        var sequence3 = Minimal.SlowBeat1WithoutRepeats();
        // note: duration property of each sequence should be predefined
        result.Append(sequence1).Append(sequence2).Append(sequence3);

        // play
        var player = new ConsoleKeyPlayer();
        player.PlayRepeated(result).Wait();
    }

    public void InjectSound()
    {
        // how to inject sounds into each repetition of a sequence
        var result = new MiniSequence();
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

    public void PlaySequencesAtTheSameTime()
    {
        // in order to play 2 sequences at the same time
        // need to add them as followers of an arbitrary sound
    }
}
