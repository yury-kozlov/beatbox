namespace Beater;

/// <summary>
/// based on https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient
/// </summary>
internal class SequencePlayerExample : IDisposable
{
    private TcpTransport? _transport;

    public SequencePlayerExample Init()
    {
        _transport = new TcpTransport();

        GC.Collect();
        return this;
    }

    public void Dispose()
    {
        _transport?.Dispose();
    }

    public async Task Run()
    {
        var seq0 = SequenceGenerator.Generate(Minimal.TechnoBeat1());
        await _transport.PlayRepeated(seq0);

        var seq1 = SequenceGenerator.Generate(Minimal.TechnoBeat2());
        await _transport.PlayRepeated(seq1);

        var seq2 = SequenceGenerator.Generate(Minimal.BrokenBeat1());
        await _transport.PlayRepeated(seq2);

        var seq3 = SequenceGenerator.Generate(Minimal.SlowBeat1WithoutRepeats());
        await _transport.PlayRepeated(seq3);

        Console.WriteLine("");
    }
}
