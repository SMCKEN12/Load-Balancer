using LoadBalancer.Models;
using LoadBalancer.Services;
using System.Net.Sockets;
using System.Net;

namespace LoadBalancerTests;

public class TcpHealthCheckerShould
{
    [Fact]
    public async Task MarksBackendHealthy_WhenItIsReachable()
    {
        // Arrange
        var port = 9200;
        var backend = new BackendServer { Ip = "127.0.0.1", Port = port, IsHealthy = false };
        var backends = new List<BackendServer> { backend };

        var cts = new CancellationTokenSource();

        var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();

        var checker = new TcpHealthChecker(backends, TimeSpan.FromMilliseconds(100), cts.Token);

        // Act
        var task = checker.StartAsync();

        await WaitUntilAsync(() => backend.IsHealthy, TimeSpan.FromSeconds(2));

        // Cleanup
        cts.Cancel();
        listener.Stop();

        // Assert
        Assert.True(backend.IsHealthy);
    }

    [Fact]
    public async Task MarksBackendUnhealthy_WhenItIsNotReachable()
    {
        // Arrange
        var port = 9201;
        var backend = new BackendServer { Ip = "127.0.0.1", Port = port, IsHealthy = true };
        var backends = new List<BackendServer> { backend };

        var cts = new CancellationTokenSource();
        var checker = new TcpHealthChecker(backends, TimeSpan.FromMilliseconds(100), cts.Token);

        // Act
        var task = checker.StartAsync();

        await WaitUntilAsync(() => backend.IsHealthy == false, TimeSpan.FromSeconds(2));

        // Cleanup
        cts.Cancel();

        // Assert
        Assert.False(backend.IsHealthy);
    }

    [Fact]
    public async Task BackendBecomesHealthy_AfterInitiallyUnreachable()
    {
        // Arrange
        var port = 9202;
        var backend = new BackendServer { Ip = "127.0.0.1", Port = port, IsHealthy = false };
        var backends = new List<BackendServer> { backend };

        var cts = new CancellationTokenSource();
        var checker = new TcpHealthChecker(backends, TimeSpan.FromMilliseconds(100), cts.Token);

        var task = checker.StartAsync();

        await Task.Delay(500, cts.Token);

        // Act
        var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();

        await WaitUntilAsync(() => backend.IsHealthy, TimeSpan.FromSeconds(2));

        // Cleanup
        await cts.CancelAsync();
        listener.Stop();

        // Assert
        Assert.True(backend.IsHealthy);
    }

    private async Task WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
    {
        var start = DateTime.UtcNow;
        while (DateTime.UtcNow - start < timeout)
        {
            if (condition())
                return;

            await Task.Delay(100);
        }

        throw new TimeoutException("Condition not met within timeout.");
    }
}