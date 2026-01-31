namespace Beater;

public record NoSound : Sound
{
    public NoSound() : base(NoSound)
    { }

    public override string? ToString() => base.ToString();
}