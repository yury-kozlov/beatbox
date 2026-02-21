namespace Beater;

/// <summary>
/// Main beat sound.
/// </summary>
public record Kick : Sound
{
    public Kick() : base(Name)
    { }

    public override string? ToString() => base.ToString();

    public static new string Name = "k";
}
