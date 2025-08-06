using Load_Balancer.Interfaces;
using Load_Balancer.Models;
using System.Net.Sockets;

namespace Load_Balancer.Services;

public class TcpHealthChecker : ITcpHealthChecker
{
    private readonly IEnumerable<BackendServer> _backends;
    private readonly TimeSpan _interval;
    private readonly CancellationToken _token;

    public TcpHealthChecker(IEnumerable<BackendServer> backends, TimeSpan interval, CancellationToken token)
    {
        _backends = backends;
        _interval = interval;
        _token = token;
    }

    public async Task StartAsync()
    {
        while (!_token.IsCancellationRequested)
        {
            foreach (var backend in _backends)
            {
                try
                {
                    using var client = new TcpClient();
                    var connectTask = client.ConnectAsync(backend.Ip, backend.Port);
                    if (await Task.WhenAny(connectTask, Task.Delay(1000)) == connectTask)
                    {
                        backend.IsHealthy = true;
                    }
                    else
                    {
                        backend.IsHealthy = false;
                    }
                }
                catch
                {
                    backend.IsHealthy = false;
                }
            }

            await Task.Delay(_interval, _token);
        }
    }
}