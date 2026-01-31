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
        var seq0 = Minimal.TechnoBeat1().Generate();
        await _transport.PlayRepeated(seq0);

        var seq1 = Minimal.TechnoBeat2().Generate();
        await _transport.PlayRepeated(seq1);

        var seq2 = Minimal.BrokenBeat1().Generate();
        await _transport.PlayRepeated(seq2);

        var seq3 = Minimal.SlowBeat1WithRepeats().Generate();
        await _transport.PlayRepeated(seq3);

        Console.WriteLine("");
    }
}
