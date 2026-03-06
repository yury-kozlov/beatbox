
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

        RemoveSoundsExceedingSequenceDuration(leader, allFollowers);

        return allFollowers;
    }

    private static void RemoveSoundsExceedingSequenceDuration(Sound leader, Sequence seq)
    {
        var unmatchedSequence = seq.Where(HasOtherSequenceDuration(leader)).ToList();
        if (unmatchedSequence.HasItems())
        {
            foreach (var sound in unmatchedSequence)
            {
                var maxAllowedTimestamp = leader.Timestamp + Math.Max(leader.Sequence!.Duration, leader.Sequence!.AutoDuration);
                if (sound.Timestamp > maxAllowedTimestamp)
                {
                    seq.Remove(sound);
                }
            }

            // delete sequence-start if it's the only remaining sound here
            var remainingSounds = unmatchedSequence.Intersect(seq).ToList();
            if (remainingSounds.Count == 1 && remainingSounds[0] is SequenceStart sequenceStart)
            {
                seq.Remove(sequenceStart);
            }
        }
    }

    /// <summary>
    /// A delegate for checking if sound's sequence has a different duration than its leader's sequence.
    /// </summary>
    private static Func<Sound, bool> HasOtherSequenceDuration(Sound leader) => sound =>
    {
        var leaderSequenceDuration = Math.Max(leader.Sequence.Duration, leader.Sequence.AutoDuration);
        var followerSequenceDuration = Math.Max(sound.Sequence.Duration, sound.Sequence.AutoDuration);
        return leaderSequenceDuration > 0 && followerSequenceDuration > 0 && leaderSequenceDuration != followerSequenceDuration;
    };

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
