namespace Beater;

/// <summary>
/// Design-time list of a sound's followers, as authored (before generation runs).
/// </summary>
public class FollowersDesign : List<SoundDesign>
{
    /// <summary>
    /// Sounds after initialization (at the point when current list 
    /// was  assigned to a leader, but before any sound besides initial list was added).
    /// Used by <see cref="InjectionMap"/> to detect sounds added later (injected).
    /// The list is chosen over a primitive counter for better debugging.
    /// The goal is to distinguish sounds that are part of the main sequence from other sounds injected afterwards.
    /// </summary>
    public List<SoundDesign> InitialSounds { get; } = [];

    public FollowersDesign AddInitialSounds(List<SoundDesign> newSounds)
    {
        if (!newSounds.HasItems())
        {
            // nothing to add
            return this;
        }

        if (!InitialSounds.HasItems())
        {
            // this is the first time we are initializing the sequence            
            // without checking for already initialized sounds we may incorrectly reset them when cloning a sound
            InitialSounds.AddRange(newSounds);
        }
        else if (newSounds.FirstOrDefault() is SequenceEnd sequenceEnd)
        {
            // end of sequence is a system sound (not part of explicit design) and is added separately from the main list
            InitialSounds.Add(sequenceEnd);
        }

        return this;
    }

    public FollowersDesign()
    { }

    public FollowersDesign(IEnumerable<SoundDesign> source) : base(source)
    { }

    public void SetLeader(SoundDesign leader)
    {
        foreach (var sound in this)
        {
            sound.Leader = leader;
        }
    }

    internal FollowersDesign DeepClone()
    {
        var clone = new FollowersDesign(this.Select(f => f.DeepClone()));
        clone.AddInitialSounds([.. InitialSounds.Select(s => s.DeepClone())]);
        return clone;
    }
}
