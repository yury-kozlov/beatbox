namespace Beater;

public record NoSound : Sound
{
    public NoSound() : base(NoSound)
    { }
}