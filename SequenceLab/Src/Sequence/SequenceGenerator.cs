
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

            // NOTE: separate followers are played independently to allow overlapping sequences (mixed together)
            var nestedFollowers = GenerateSequence(follower);
            allFollowers.AddRange(nestedFollowers);
        }

        foreach (var nestedFollower in allFollowers)
        {
            // shift timestamp relatively to the leader (so that each sound will have an absolute position from the beginning of the whole sequence):
            nestedFollower.Timestamp += leader.Timestamp;
        }

        if (leader is SequenceStart sequenceStart)
        {
            // this is only to improve debugging experience
            AlignSequenceStartTimestamp(sequenceStart, allFollowers);
        }

        RemoveSoundsExceedingSequenceDuration(leader, allFollowers);

        return allFollowers;
    }

    private static void AlignSequenceStartTimestamp(SequenceStart sequenceStart, Sequence mixedSequence)
    {
        // TODO: delete this whole method because it leads to confusion (when reading broken range of shifted sequences in the final output)
        // this alignment only happens when the first sound has explicit DelayAfterLeader which is incorrect
        // because this property should be defined on the SequenceStart itself not on the first sound

        // copy delay from the first sound of the sequence, so that they will always come together in the final output
        sequenceStart.Timestamp = mixedSequence.FirstOrDefault()?.Timestamp ?? 0;
    }

    private static void RemoveSoundsExceedingSequenceDuration(Sound leader, Sequence seq)
    {
        var unmatchedSequence = seq.Where(HasOtherSequenceDuration(leader)).ToList();
        if (unmatchedSequence.HasItems())
        {
            foreach (var sound in unmatchedSequence)
            {
                var maxAllowedTimestamp = leader.Timestamp + leader.Sequence!.Duration;
                if (sound.Timestamp > maxAllowedTimestamp)
                {
                    seq.Remove(sound);
                }
            }
        }
    }

    /// <summary>
    /// A delegate for checking if sound's sequence has a different duration than its leader's sequence.
    /// </summary>
    private static Func<Sound, bool> HasOtherSequenceDuration(Sound leader)
    {
        return sound => leader.Sequence?.Duration > 0
                      && sound.Sequence?.Duration > 0
                      && leader.Sequence.Duration != sound.Sequence.Duration;
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
