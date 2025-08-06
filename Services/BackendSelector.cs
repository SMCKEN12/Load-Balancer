using Load_Balancer.Interfaces;
using Load_Balancer.Models;

namespace Load_Balancer.Services;

public class BackendSelector : IBackendSelector
{
    private readonly List<BackendServer> _backends;

    public BackendSelector(List<BackendServer> backends)
    {
        _backends = backends;
    }

    public BackendServer? GetNextBackend()
    {
        return _backends
            .Where(b => b.IsHealthy).MinBy(b => b.ActiveConnections);
    }
}