using LoadBalancer.Interfaces;
using LoadBalancer.Models;

namespace LoadBalancer.Services;

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