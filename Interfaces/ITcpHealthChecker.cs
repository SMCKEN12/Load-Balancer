namespace Load_Balancer.Interfaces;

public interface ITcpHealthChecker
{
    Task StartAsync();
}