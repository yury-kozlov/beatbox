namespace Beater;

/// <summary>
/// Main beat sound.
/// </summary>
public record Kick : SoundDesign
{
    public Kick() : base(Name)
    { }

    public override string? ToString() => base.ToString();

    public static new string Name = "k";
}
