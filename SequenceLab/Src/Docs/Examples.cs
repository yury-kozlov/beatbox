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
        var result = new SequenceDesign("example");
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
        var result = new SequenceDesign("example");
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

    public void RepeatSequenceInLoop()
    {
        // sequence may be repeated in a few ways: in a loop or using a strategy
        // this example shows how to repeat/append a sequence in loop:
        var main = new SequenceDesign("main");
        for (int i = 1; i <= 4; i++)
        {
            var kicks = new PrimitiveSequences.Square<Kick>($"kicks.{i}") { Interval = 500 };
            main.Append(kicks);
        }

        // play
        var player = new ConsoleKeyPlayer();
        player.PlayRepeated(main).Wait();
    }

    public void RepeatSequenceWithStrategy()
    {
        // sequence may be repeated in a few ways: in a loop or using a strategy
        // this example shows how to apply repeat strategy for a sequence:
        var kicks = new PrimitiveSequences.Square<Kick>() { Interval = 500 };
        kicks.Strategy = new RepeatStrategy { Count = 2 };

        // play
        var player = new ConsoleKeyPlayer();
        player.PlayRepeated(kicks).Wait();
    }

    public void PlaySequencesAtTheSameTime()
    {
        // in order to play 2 sequences at the same time
        // just add them to the same parent sequence:
        var kicks = new SequenceDesign("kicks")
        {
            Leader = new Kick { Strategy = new RepeatStrategy { Interval = 500, Count = 4 } },
        };

        var snares = new SequenceDesign("snares")
        {
            Leader = new Metronome
            {
                Strategy = new RepeatStrategy { Interval = 400, Count = 4 },
                Followers = [new Snare { Strategy = new PlayOnceStrategy { PlayEveryX = 2, DelayAfterLeader = 100 } }],
            },
        };

        var parent = new SequenceDesign("main")
            .Append(kicks)
            .Append(snares);

        // play
        var player = new ConsoleKeyPlayer();
        player.PlayRepeated(parent).Wait();
    }
}
