using LoadBalancer.Models;

namespace LoadBalancer.Interfaces;

public interface IBackendSelector
{
    BackendServer? GetNextBackend();
}