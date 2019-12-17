using StreamJsonRpc;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new TcpClient();
            await client.ConnectAsync("localhost", 5150);
            var jsonRpc = JsonRpc.Attach(client.GetStream());
            var now = DateTimeOffset.Now;
            Console.WriteLine($"{now.Minute} + {now.Second}");
            var sum = await jsonRpc.InvokeAsync<int>("Add", now.Minute, now.Second);
            Console.WriteLine(sum);
        }
    }
}
