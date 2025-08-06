using Load_Balancer.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace Load_Balancer.Services;

public class TcpLoadBalancer : ITcpLoadBalancer
{
    private readonly TcpListener _listener;
    private readonly IBackendSelector _backendSelector;
    private readonly ITcpHealthChecker _healthChecker;
    private readonly ITcpConnectionHandler _connectionHandler;

    public TcpLoadBalancer(
        int listenPort,
        IBackendSelector backendSelector,
        ITcpHealthChecker healthChecker,
        ITcpConnectionHandler connectionHandler)
    {
        _listener = new TcpListener(IPAddress.Any, listenPort);
        _backendSelector = backendSelector;
        _healthChecker = healthChecker;
        _connectionHandler = connectionHandler;
    }

    public async Task StartAsync(int maxConnections)
    {
        _listener.Start();
        _ = _healthChecker.StartAsync();

        int count = 0;
        while (count++ < maxConnections)
        {
            var client = await _listener.AcceptTcpClientAsync();
            var backend = _backendSelector.GetNextBackend();

            if (backend == null)
            {
                client.Close();
                continue;
            }

            _ = Task.Run(() => _connectionHandler.HandleAsync(client, backend));
        }
    }
}