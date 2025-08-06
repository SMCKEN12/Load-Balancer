namespace LoadBalancer.Models;

public class BackendServer
{
    public string Ip { get; set; }
    public int Port { get; set; }
    public bool IsHealthy { get; set; } = true;

    private int _activeConnections;
    public int ActiveConnections => _activeConnections;

    public void IncrementConnections() => Interlocked.Increment(ref _activeConnections);

    public void DecrementConnections() => Interlocked.Decrement(ref _activeConnections);
}