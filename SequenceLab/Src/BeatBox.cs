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
        var sequence1 = new Sequence
        {
            Leader = new Sound("b1")
            {
                Strategy = new RepeatStrategy { Count = 16, Interval = 500 },
                Followers = new() {
                  new Sound("ts1") {
                      Strategy = new RepeatStrategy { DelayAfterLeader = 150, Count = 2, Interval = 80 },
                  },
                  new Sound("ts2") { Strategy = new PlayOnceStrategy { PlayEveryX = 4 } },
                  new Sound("b2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 250, PlayEveryX = 4 } },
                  new Sound("ts3") { Strategy = new RepeatStrategy { DelayAfterLeader = 80, Count = 4, Interval = 80, LinearIncrement = -10, PlayEveryX = 8 } },
                  // new Sound("b1") { Strategy = new RepeatStrategy { DelayAfterLeader = 80, Count = 4, Interval = 80, LinearIncrement = -10, PlayEveryX = 16 } },
               },
            },
        };
        var sequenceMessages1 = sequence1.Generate();
        await _channel.Play(sequenceMessages1);


        // TODO: implement different sequences with the same sounds and then try to connect them together as an evolution of one sequence into another
        


        Console.WriteLine();
    }
}
