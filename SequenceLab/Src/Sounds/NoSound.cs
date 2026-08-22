namespace Beater;

public record NoSound : SoundDesign
{
    public NoSound() : base(Name)
    { }

    public override string? ToString() => base.ToString();

    public new const string Name = "no-sound";
}