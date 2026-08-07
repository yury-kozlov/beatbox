namespace Beater;

internal class Theory
{
    public void TwoSequencesPlayedTogether()
    {
        // can 2 sequences play at the same time?
        // - not recommended because same overlapping sounds may easily produce chaos

        // can be played simultaneously:
        // - if each sequence uses distinguishable sounds
        // OR
        // - if they use different timings (no overlapping between individual sounds), which is actually the same as using one sequence
        /// <see cref="Examples.PlaySequencesAtTheSameTime" />
        // this may be useful when playing different sequences in an experiment to find possible patterns between them
    }

    public void HowToCombineTwoSequences()
    {
        // take a small sequence containing just few sounds and inject it into the longer sequence as a follower of one of its sounds
        /// another option is to use <see cref="SequenceDesign.Append"/> or <see cref="SequenceDesign.Combine"/>
    }

    public void HowToPlaySequenceInLoop()
    {
        /// using repeat strategy: <see cref="Examples.RepeatSequenceWithStrategy"/>
        /// using manual loop: <see cref="Examples.RepeatSequenceInLoop"/>
    }

    public void HowAppendSequenceWorks()
    {
        // 1. SequenceEnd sound is added as the last follower of each sequence (to followers of SequenceStart)
        // 2. if sequence has duration - SequenceEnd will have DelayAfterLeader = Duration
        // 3. if sequence has no duration SequenceEnd will have the same timestamp as the last sound of the sequence
        // 4. each appended sequence will use FollowPreviousSoundStrategy and will use SequenceEnd of the previous sequence as its leader

        /// Why do we need SequenceStart and SequenceEnd sounds? to be able to append sequences at correct timings 
        /// to omit specifying explicit duration of a sequence and calculate it on-the-fly
    }

    public void SoundDesign()
    {
        // sound design is a code model of a sound before it's generated and played
        // each sound has a strategy
        // strategy decides when this sound should play

        // each sound may have followers
        // followers are the sounds that will be played after the current sound
    }

    public void Lifecycle()
    {
        // first we define a markup sequence (with rules of follower's strategies)
        // then we pass it to the player
        // player generates sequence of sounds with timestamps based on each sound strategy
        //   (for example, expands repeat strategy into multiple iterations of the same sound)
        // player sends messages with sound timestamps to a sampler
    }
}
