namespace Beater;

/// <summary>
/// Main beat sound.
/// </summary>
public record Kick : Sound
{
    public Kick() : base(KickSound)
    { }

    public override string? ToString() => base.ToString();
}
