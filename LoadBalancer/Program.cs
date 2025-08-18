using Microsoft.Extensions.Configuration;
using LoadBalancer.Interfaces;
using LoadBalancer.Models;
using LoadBalancer.Services;

namespace LoadBalancer
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

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var lbConfig = configuration.GetSection("LoadBalancer").Get<LoadBalancerConfig>();

            var backendServers = lbConfig.Backends
                .Select(b => new BackendServer { Ip = b.Ip, Port = b.Port })
                .ToList();

            IBackendSelector backendSelector = new BackendSelector(backendServers);
            ITcpHealthChecker healthChecker = new TcpHealthChecker(backendServers, TimeSpan.FromSeconds(lbConfig.HealthCheckIntervalSeconds), cts.Token);
            ITcpConnectionHandler connectionHandler = new TcpConnectionHandler();

            ITcpLoadBalancer loadBalancer = new TcpLoadBalancer(
                listenPort: lbConfig.ListenPort,
                backendSelector: backendSelector,
                healthChecker: healthChecker,
                connectionHandler: connectionHandler,
                cts.Token
            );

            Console.WriteLine("Starting TCP Load Balancer on port 8080...");
            await loadBalancer.StartAsync(100);
        }
    }
}
