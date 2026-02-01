namespace Beater;

/// <summary>
/// Acts like a loop grid without any sound.
/// The same as NoSound, used for better developer experience.
/// </summary>
public record Metronome : NoSound
{
    public Metronome()
    {
        FriendlyName = "metronome";
    }

    public override string? ToString() => FriendlyName;
}
