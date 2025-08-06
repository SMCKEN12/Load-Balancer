using Load_Balancer.Interfaces;
using Load_Balancer.Models;
using Load_Balancer.Services;

namespace Load_Balancer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Console.WriteLine("Shutting down...");
                eventArgs.Cancel = true;
                cts.Cancel();
            };

            var backendServers = new List<BackendServer>
            {
                new BackendServer { Ip = "127.0.0.1", Port = 9001 },
                new BackendServer { Ip = "127.0.0.1", Port = 9002 },
                new BackendServer { Ip = "127.0.0.1", Port = 9003 }
            };

            IBackendSelector backendSelector = new BackendSelector(backendServers);
            ITcpHealthChecker healthChecker = new TcpHealthChecker(backendServers, TimeSpan.FromSeconds(5), cts.Token);
            ITcpConnectionHandler connectionHandler = new TcpConnectionHandler();

            ITcpLoadBalancer loadBalancer = new TcpLoadBalancer(
                listenPort: 8080,
                backendSelector: backendSelector,
                healthChecker: healthChecker,
                connectionHandler: connectionHandler
            );

            Console.WriteLine("Starting TCP Load Balancer on port 8080...");
            await loadBalancer.StartAsync(100);
        }
    }
}