using System.Net.Sockets;

namespace Beater;

/// <summary>
/// based on https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient
/// </summary>
internal class BeatBox : IDisposable
{
    private TcpClient? _client;
    private NetworkStream? _channel;

    public BeatBox Init()
    {
        _client = new TcpClient("localhost", 3000);
        _channel = _client.GetStream();

        GC.Collect();
        return this;
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    public async Task Run()
    {
        var seq0 = Minimal.TechnoBeat1().Generate();
        await _channel.PlayRepeated(seq0);

        var seq1 = Minimal.TechnoBeat2().Generate();
        await _channel.PlayRepeated(seq1);

        var seq2 = Minimal.BrokenBeat1().Generate();
        await _channel.PlayRepeated(seq2);

        // TODO 1:   implement different sequences with the same sounds and then try to connect them together as an evolution of one sequence into another
        // question: will re-sampling existing tracks into sequences help to learn evolution of sequences using the same sounds ?
        //           it might not help create new sequences but instead will help to repeat existing patterns

        // question: do we need exact time scheduling for each sound or it's ok to have time shifting due to inaccuracy of .net delays?

        // TODO 2:   think how to simplify the sequence creation process and make it more readable
        //             e.g.: create strategy that may use another strategy as a follower
        //             e.g.: can we avoid creating empty sounds just to be able to stick other sound strategies to them?

        Console.WriteLine("");
    }
}
