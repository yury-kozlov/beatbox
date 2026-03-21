namespace Beater;

/// <summary>
/// Snare sound.
/// </summary>
public record Snare : Sound
{
    public Snare() : base(Name)
    { }

    public override string? ToString() => base.ToString();

    public static new string Name = "s";
}

/// <summary>
/// Snare sound.
/// </summary>
public record Snare1 : Sound
{
    public Snare1() : base(Name)
    { }

    public override string? ToString() => base.ToString();

    public static new string Name = "ts1";
}
