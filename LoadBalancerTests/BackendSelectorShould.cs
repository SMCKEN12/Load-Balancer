using LoadBalancer.Models;
using LoadBalancer.Services;

namespace LoadBalancerTests;

public class BackendSelectorShould
{
    [Fact]
    public void GetNextBackend_ReturnsBackendWithFewestConnections()
    {
        // Arrange
        var backends = new List<BackendServer>
            {
                new BackendServer { Ip = "127.0.0.1", Port = 9001, IsHealthy = true },
                new BackendServer { Ip = "127.0.0.1", Port = 9002, IsHealthy = true }
            };

        backends[0].IncrementConnections();
        backends[1].IncrementConnections();
        backends[1].IncrementConnections();

        var selector = new BackendSelector(backends);

        // Act
        var selected = selector.GetNextBackend();

        // Assert
        Assert.Equal(9001, selected?.Port);
    }
}
