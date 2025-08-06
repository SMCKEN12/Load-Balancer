namespace Load_Balancer.Interfaces;

public interface ITcpLoadBalancer
{
    Task StartAsync(int maxConnections);
}