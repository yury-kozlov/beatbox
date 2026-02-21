namespace Beater;

public record NoSound : Sound
{
    public NoSound() : base(Name)
    { }

    public override string? ToString() => base.ToString();

    public static new string Name = "no-sound";
}