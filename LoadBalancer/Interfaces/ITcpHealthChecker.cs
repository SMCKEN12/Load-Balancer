namespace LoadBalancer.Interfaces;

public interface ITcpHealthChecker
{
    Task StartAsync();
}