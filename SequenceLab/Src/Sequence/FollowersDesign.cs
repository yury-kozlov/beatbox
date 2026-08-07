namespace Beater;

/// <summary>
/// Design-time list of a sound's followers, as authored (before generation runs).
/// </summary>
public class FollowersDesign : List<SoundDesign>
{
    /// <summary>
    /// Number of sounds in the list after initialization (at the point when current list 
    /// was  assigned to a leader, but before any sound besides initial list was added).
    /// Used by <see cref="InjectionMap"/> to detect sounds added later (injected).
    /// </summary>
    public int InitialLength;

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
}
