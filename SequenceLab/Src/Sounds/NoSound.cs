namespace Beater;

public record NoSound : SoundDesign
{
    public NoSound() : base(Name)
    { }

    public override string? ToString() => base.ToString();

    public static new string Name = "no-sound";
}