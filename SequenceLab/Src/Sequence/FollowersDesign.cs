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

    public void TryAddInitialSounds(List<SoundDesign> newSounds)
    {
        if (newSounds.HasItems() && !InitialSounds.HasItems())
        {
            // this is the first time we are initializing the sequence
            InitialSounds.AddRange(newSounds);
        }
    }

    public void AddInitialSound(SoundDesign newSound)
    {
        InitialSounds.Add(newSound);
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
        foreach (var initialSound in InitialSounds)
        {
            clone.AddInitialSound(initialSound.DeepClone());
        }
        return clone;
    }
}
