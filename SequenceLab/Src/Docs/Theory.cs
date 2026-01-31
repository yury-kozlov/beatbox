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
    }

    public void HowToCombineTwoSequences()
    {
        // take a small sequence containing just few sounds and inject it into the longer sequence as a follower of one of its sounds
    }

    public void Sound()
    {
        // each sound has a strategy
        // it depends on the strategy when this sound should play
        
        // each sound may have followers
        // it depends on the followers which sounds will be played after the current sound
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
