// See https://aka.ms/new-console-template for more information

using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;

TcpListener incoming = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
TcpListener outcoming = new TcpListener(IPAddress.Parse("127.0.0.1"), 5001);

outcoming.Start();
incoming.Start();

var outcomingTask = Task.Factory.StartNew( async () =>
{
    while (true)
    {
        //var acceptTcpClient = await incoming.AcceptTcpClientAsync();

    }
});

var incomingTask = Task.Factory.StartNew(async () =>
{
    while (true)
    {
        Console.Out.WriteLine("Waiting for client");
        var acceptTcpClient = await incoming.AcceptTcpClientAsync();
        Console.Out.WriteLine($"Connected {acceptTcpClient.Client.LocalEndPoint}");

        while (acceptTcpClient.Client.Connected)
        {
            if (acceptTcpClient.Available > 0)
            {
                //await using var networkStream = acceptTcpClient.GetStream();
                var buffer = ArrayPool<byte>.Shared.Rent(acceptTcpClient.Available);
                Memory<byte> mem = buffer;
                var read = await acceptTcpClient.Client.ReceiveAsync(mem, SocketFlags.Peek).ConfigureAwait(false);
                
                if(read > 0)
                {
                    Console.Out.WriteLine($"Input data: {Encoding.ASCII.GetString(mem.ToArray())}");
                }
                
                ArrayPool<byte>.Shared.Return(buffer);
            }
            else
            {
                await Task.Delay(100);
            }
        }
    }
});

Task.WaitAll(incomingTask, outcomingTask);
Console.Out.WriteLine("Finish");


