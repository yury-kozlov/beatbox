namespace Beater;

/// <summary>
/// Snare sound.
/// </summary>
public record Snare : SoundDesign
{
    public Snare() : base(Name)
    { }

    public override string? ToString() => base.ToString();

    public static new string Name = "s";
}

/// <summary>
/// Snare sound.
/// </summary>
public record Snare1 : SoundDesign
{
    public Snare1() : base(Name)
    { }

    public override string? ToString() => base.ToString();

    public static new string Name = "ts1";
}
