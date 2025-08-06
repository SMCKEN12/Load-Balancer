namespace LoadBalancer.Interfaces;

public interface ITcpLoadBalancer
{
    Task StartAsync(int maxConnections);
}