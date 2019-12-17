using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TestServer
{
    internal class Server
    {
        private readonly ILogger<Server> logger;

        public Server(ILogger<Server> logger)
        {
            this.logger = logger;
        }

        public int Add(int a, int b)
        {
            logger.LogInformation("Received Add: {0} + {1}", a, b);
            return a + b;
        }

        public event EventHandler<string>? MessageArrived;

        public void SendMessage(string message)
        {
            logger.LogInformation("Received SendMessage: {0}", message);

            Task.Run(async () =>
            {
                await Task.Delay(2000).ConfigureAwait(false);
                MessageArrived?.Invoke(this, message);
            });
        }
    }
}
