
namespace Beater;

public class SequenceGenerator
{
    public static Sequence Generate(SequenceDesign seq)
    {
        seq.Leader.WithSequenceIfMissing(seq);
        return GenerateSequence(seq.Leader);
    }

    /// <summary>
    /// An entry point to generate sequence of sounds of leader/follower.
    /// </summary>
    /// <param name="previousSounds">Sequence of previous sounds (if any), from which the generated sequence will continue.</param>
    private static Sequence GenerateSequence(Sound leader)
    {
        leader = leader with { /* clone */ };
        leader.Followers.SetLeader(leader);

        leader.Strategy.CheckedTimes++;
        if (IsSkipped(leader))
        {
            // skip
            return new();
        }

        if (IsSilenced(leader))
        {
            leader = leader with { IsSilenced = true };
        }

        leader.Strategy.CalledTimes++;

        // in most cases "leaders" will contain single sound (current leader), except for repeat strategy which will clone leader in loop
        Sequence leaders = leader.Strategy.ApplyStrategy(leader);

        // note: followers are generated separately from the leader - meaning leader will not be able to take decisions based on its followers
        var followers = new Sequence();
        foreach (var currentLeader in leaders)
        {
            if (currentLeader.Followers.HasItems())
            {
                followers.AddRange(GenerateFollowersSequence(currentLeader));
            }
        }

        // mix leaders with all followers:
        return leaders.Mix(followers);
    }

    private static Sequence GenerateFollowersSequence(Sound leader)
    {
        var allFollowers = new Sequence();
        foreach (var follower in leader.Followers)
        {
            follower.PreviousSounds = allFollowers;
            if (leader.Strategy.FireAndForget)
            {
                // propagate this flag to direct followeres
                follower.Strategy.FireAndForget = leader.Strategy.FireAndForget;
            }

            // NOTE: separate followers are played independently to allow overlapping sequences (mixed together)
            var nestedFollowers = GenerateSequence(follower);
            allFollowers.AddRange(nestedFollowers);
        }

        foreach (var follower in allFollowers)
        {
            // shift timestamp relatively to the leader (so that each sound will have an absolute position from the beginning of the whole sequence):
            follower.Timestamp += leader.Timestamp;
            SetIterationPath(leader, follower);
        }

        if (leader.Strategy is RepeatStrategy)
        {
            // we can't allow sounds to fall out of the loop
            RemoveSoundsExceedingLoop(leader, allFollowers);
        }

        return allFollowers;
    }

    private static void SetIterationPath(Sound leader, Sound follower)
    {
        if (leader.Iteration.IsNullOrEmpty())
        {
            // nothing to prepend to the follower
            return;
        }

        if (follower.Iteration.IsNullOrEmpty())
        {
            follower.Iteration = leader.Iteration;
            return;
        }

        follower.Iteration = $"{leader.Iteration}.{follower.Iteration}";
    }

    /// <summary>
    /// Any sounds exceeding total duration of repeated sequence are removed here.
    /// </summary>
    private static void RemoveSoundsExceedingLoop(Sound leader, Sequence seq)
    {
        var repeatStrategy = (RepeatStrategy)leader.Strategy;
        var maxAllowedTimestamp = leader.Timestamp + repeatStrategy.Interval;
        var outliers = seq.Where(s => s.Timestamp > maxAllowedTimestamp).ToList();
        foreach (var sound in outliers)
        {
            Console.WriteLine($"Trimming sound '{sound.Name}' at {sound.Timestamp}ms as it exceeds parent loop interval of {repeatStrategy.Interval}ms started at {leader.Timestamp}");
                    seq.Remove(sound);

                    if (sound is SequenceEnd sequenceEnd)
                    {
                        // add indication that sequence was trimmed (instead of previously deleted SequenceEnd)
                        seq.Add(new SequenceEndTrimmed(sequenceEnd) { Timestamp = maxAllowedTimestamp });
                    }
            else if (sound is LoopEnd loopEnd)
            {
                // add indication that the loop was trimmed (instead of previously deleted LoopEnd)
                seq.Add(new LoopEndTrimmed(loopEnd) { Timestamp = maxAllowedTimestamp });
                }
            }
        }

    private static bool IsSilenced(Sound sound)
    {
        return Numbers.IsXOutOf(sound.Strategy.SilenceEveryXOutOf, sound.Strategy.CheckedTimes);
    }

    private static bool IsSkipped(Sound sound)
    {
        if (sound.Strategy.PlayEveryXOutOf.HasValue())
        {
            return !Numbers.IsXOutOf(sound.Strategy.PlayEveryXOutOf, sound.Strategy.CheckedTimes);
        }

        // count every call
        return sound.Strategy.CheckedTimes % sound.Strategy.PlayEveryX > 0;
    }
}
