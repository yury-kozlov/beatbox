namespace Beater;

/// <summary>
/// Snare sound.
/// </summary>
public record Snare : Sound
{
    public Snare() : base(SnareSound)
    { }

    public override string? ToString() => base.ToString();
}
