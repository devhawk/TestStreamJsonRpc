using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StreamJsonRpc;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TestStreamJsonRpc
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly TcpListener listener;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            listener = new TcpListener(IPAddress.Any, 5150);
        }

        private static async Task RunClient(TcpClient client, Server instance, CancellationToken token)
        {
            var jsonRpc = JsonRpc.Attach(client.GetStream(), instance);
            token.Register(() => jsonRpc.Dispose());
            await jsonRpc.Completion;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            listener.Start();
            logger.LogInformation("Listening on {0}", listener.LocalEndpoint);
            token.Register(() => listener.Stop());

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var client = await Task.Run(() => listener.AcceptTcpClientAsync(), token).ConfigureAwait(false);
                    var instance = ActivatorUtilities.CreateInstance<Server>(serviceProvider);
                    var _ = Task.Run(() => RunClient(client, instance, token));
                }
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
