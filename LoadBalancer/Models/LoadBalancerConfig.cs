namespace LoadBalancer.Models;

public class LoadBalancerConfig
{
    public int ListenPort { get; set; }
    public int HealthCheckIntervalSeconds { get; set; }
    public List<BackendConfig> Backends { get; set; } = new();
}

public class BackendConfig
{
    public string Ip { get; set; } = string.Empty;
    public int Port { get; set; }
}