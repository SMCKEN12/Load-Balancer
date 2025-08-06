using Load_Balancer.Models;

namespace Load_Balancer.Interfaces;

public interface IBackendSelector
{
    BackendServer? GetNextBackend();
}