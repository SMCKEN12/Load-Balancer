using LoadBalancer.Interfaces;
using LoadBalancer.Models;
using LoadBalancer.Services;
using System.Net.Sockets;
using System.Net;
using NSubstitute;

namespace LoadBalancerTests;

public class TcpLoadBalancerShould
{
    [Fact]
    public async Task StartAsync_DelegatesClientToHandler_WhenBackendIsAvailable()
    {
        // Arrange
        var backend = Substitute.For<BackendServer>();
        var backendSelector = Substitute.For<IBackendSelector>();
        var healthChecker = Substitute.For<ITcpHealthChecker>();
        var connectionHandler = Substitute.For<ITcpConnectionHandler>();

        backendSelector.GetNextBackend().Returns(backend);

        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();

        var loadBalancer = new TcpLoadBalancer(
            port,
            backendSelector,
            healthChecker,
            connectionHandler
        );

        _ = Task.Run(async () =>
        {
            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, port);
        });

        // Act
        await loadBalancer.StartAsync(maxConnections: 1);

        // Assert
        await healthChecker.Received(1).StartAsync();
        await connectionHandler.Received(1)
            .HandleAsync(Arg.Any<TcpClient>(), backend);
    }

    [Fact]
    public async Task StartAsync_ClosesClient_WhenNoBackendAvailable()
    {
        // Arrange
        var backendSelector = Substitute.For<IBackendSelector>();
        var healthChecker = Substitute.For<ITcpHealthChecker>();
        var connectionHandler = Substitute.For<ITcpConnectionHandler>();

        backendSelector.GetNextBackend().Returns((BackendServer?)null);

        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();

        var loadBalancer = new TcpLoadBalancer(
            port,
            backendSelector,
            healthChecker,
            connectionHandler
        );

        var clientConnected = new TaskCompletionSource<TcpClient>();

        _ = Task.Run(async () =>
        {
            var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, port);
            clientConnected.SetResult(client);
        });

        // Act
        await loadBalancer.StartAsync(maxConnections: 1);

        // Assert
        await healthChecker.Received(1).StartAsync();
        await connectionHandler.DidNotReceiveWithAnyArgs()
            .HandleAsync(default!, default!);
    }
}