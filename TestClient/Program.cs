using StreamJsonRpc;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TestClient
{
    internal interface IServer
    {
        event EventHandler<string> MessageArrived;
        Task<int> Add(int a, int b);
        Task SendMessage(string message);
    }

    internal static class Program
    {
        private static async Task Main()
        {
            using var client = new TcpClient();
            await client.ConnectAsync("localhost", 5150).ConfigureAwait(false);
            var jsonRpc = JsonRpc.Attach<IServer>(client.GetStream());
            jsonRpc.MessageArrived += JsonRpc_MessageArrived;

            var now = DateTimeOffset.Now;
            Console.WriteLine($"{now.Minute} + {now.Second}");
            var sum = await jsonRpc.Add(now.Minute, now.Second).ConfigureAwait(false);
            Console.WriteLine(sum);

            await jsonRpc.SendMessage("Hello, World!").ConfigureAwait(false);

            Console.WriteLine("About to wait");
            await Task.Delay(4000).ConfigureAwait(false);
            Console.WriteLine("wait Completed");
        }

        private static void JsonRpc_MessageArrived(object? sender, string e)
        {
            Console.WriteLine($"MessageArrived: {e}");
        }
    }
}
