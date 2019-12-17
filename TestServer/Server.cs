using Microsoft.Extensions.Logging;

namespace TestServer
{
    class Server
    {
        private readonly ILogger<Server> logger;

        public Server(ILogger<Server> logger)
        {
            this.logger = logger;
        }

        public int Add(int a, int b)
        {
            logger.LogInformation("Received request: {0} + {1}", a, b);
            return a + b;
        }
    }
}
