
namespace Beater;

public class SequenceGenerator
{
    public static GeneratedSequence Generate(SequenceDesign seq)
    {
        seq.Leader.WithSequenceIfMissing(seq);
        return GenerateSequence(seq.Leader);
    }

    /// <summary>
    /// An entry point to generate sequence of sounds of leader/follower.
    /// </summary>
    /// <param name="previousSounds">Sequence of previous sounds (if any), from which the generated sequence will continue.</param>
    private static GeneratedSequence GenerateSequence(SoundDesign leader)
    {
        leader = leader.DeepClone();
        leader.Followers.SetLeader(leader);

        leader.Strategy.CheckedTimes++;
        if (IsSkipped(leader))
        {
            // skip
            return [];
        }

        if (IsSilenced(leader))
        {
            leader.Generated.IsSilenced = true;
        }

        leader.Strategy.CalledTimes++;

        // in most cases "leaders" will contain single sound (current leader), except for repeat strategy which will clone leader in loop
        var leaders = leader.Strategy.ApplyStrategy(leader);

        // note: followers are generated separately from the leader - meaning leader will not be able to take decisions based on its followers
        var followers = new GeneratedSequence();
        foreach (var currentLeader in leaders)
        {
            if (currentLeader.SoundDesign.Followers.HasItems())
            {
                followers.AddRange(GenerateFollowersSequence(currentLeader.SoundDesign));
            }
        }

        // mix leaders with all followers:
        return leaders.Mix(followers);
    }

    private static GeneratedSequence GenerateFollowersSequence(SoundDesign leader)
    {
        var allFollowers = new GeneratedSequence();
        var injectionMap = new InjectionMap(leader.Followers);
        leader.Followers = injectionMap.Ordered ?? leader.Followers;

        foreach (var follower in leader.Followers)
        {
            follower.PreviousSounds = allFollowers;
            follower.Injected = injectionMap.GetInjectedSequence(follower);

            PropagateFireAndForget(leader, follower);

            // NOTE: separate followers are played independently to allow overlapping sequences (mixed together)
            var nestedFollowers = GenerateSequence(follower);
            if (!nestedFollowers.HasItems())
            {
                continue;
            }

            allFollowers.AddRange(nestedFollowers);
            injectionMap.SetInjectionSequence(follower, nestedFollowers);
        }

        // adjust timestamps of all followers only after nested sequences were generated (because on nested levels timestamps are expected to be relative)
        foreach (var follower in allFollowers)
        {
            // shift timestamp relatively to the leader (so that each sound will have an absolute position from the beginning of the whole sequence):
            follower.Timestamp += leader.Generated.Timestamp;
            SetIterationPath(leader, follower);
        }

        if (leader.Strategy is RepeatStrategy)
        {
            // we can't allow sounds to fall out of the loop
            RemoveSoundsExceedingLoop(leader, allFollowers);
        }

        return allFollowers;
    }

    /// <summary>
    /// Propagates FireAndForget to a direct follower, with one exception:
    /// when the leader is SequenceStart (i.e. FireAndForget is set at the sequence-design level),
    /// only SequenceEnd inherits the flag — so external sounds cannot follow this sequence,
    /// while internal sounds remain unaffected and still chain normally via FollowPreviousSoundStrategy.
    /// </summary>
    private static void PropagateFireAndForget(SoundDesign leader, SoundDesign follower)
    {
        if (!leader.Strategy.FireAndForget)
        {
            return;
        }

        if (leader is not SequenceStart || follower is SequenceEnd)
        {
            follower.Strategy.FireAndForget = true;
        }
    }

    private static void SetIterationPath(SoundDesign leader, GeneratedSound follower)
    {
        if (leader.Generated.Iteration.IsNullOrEmpty())
        {
            // nothing to prepend to the follower
            return;
        }

        if (follower.Iteration.IsNullOrEmpty())
        {
            follower.Iteration = leader.Generated.Iteration;
            return;
        }

        follower.Iteration = $"{leader.Generated.Iteration}.{follower.Iteration}";
    }

    /// <summary>
    /// Any sounds exceeding total duration of repeated sequence are removed here.
    /// </summary>
    private static void RemoveSoundsExceedingLoop(SoundDesign leader, GeneratedSequence seq)
    {
        var repeatStrategy = (RepeatStrategy)leader.Strategy;
        var maxAllowedTimestamp = leader.Generated.Timestamp + repeatStrategy.Interval;
        var outliers = seq.Where(s => s.Timestamp > maxAllowedTimestamp).ToList();
        foreach (var sound in outliers)
        {
            Console.WriteLine($"Trimming sound '{sound.ToString()}' as it exceeds parent loop interval of {repeatStrategy.Interval}ms started at {leader.Generated.Timestamp}");
            seq.Remove(sound);

            if (sound.SoundDesign is SequenceEnd sequenceEnd)
            {
                // add indication that sequence was trimmed (instead of previously deleted SequenceEnd)
                var trimmedEnd = new SequenceEndTrimmed(sequenceEnd);
                trimmedEnd.Generated.Timestamp = maxAllowedTimestamp;
                seq.Add(trimmedEnd.Generated);
            }
            else if (sound.SoundDesign is LoopEnd loopEnd)
            {
                // add indication that the loop was trimmed (instead of previously deleted LoopEnd)
                var trimmedLoop = new LoopEndTrimmed(loopEnd);
                trimmedLoop.Generated.Timestamp = maxAllowedTimestamp;
                seq.Add(trimmedLoop.Generated);
            }
        }
    }

    private static bool IsSilenced(SoundDesign sound)
    {
        return Numbers.IsXOutOf(sound.Strategy.SilenceEveryXOutOf, sound.Strategy.CheckedTimes);
    }

    private static bool IsSkipped(SoundDesign sound)
    {
        if (sound.Strategy.PlayEveryXOutOf.HasValue())
        {
            return !Numbers.IsXOutOf(sound.Strategy.PlayEveryXOutOf, sound.Strategy.CheckedTimes);
        }

        // count every call
        return sound.Strategy.CheckedTimes % sound.Strategy.PlayEveryX > 0;
    }
}
