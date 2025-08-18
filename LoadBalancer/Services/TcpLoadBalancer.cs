using LoadBalancer.Interfaces;
using System.Net.Sockets;
using System.Net;

namespace LoadBalancer.Services;

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
        ITcpConnectionHandler connectionHandler, CancellationToken cancellationToken)
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

        using var semaphore = new SemaphoreSlim(maxConnections);

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();

            await semaphore.WaitAsync();

            var backend = _backendSelector.GetNextBackend();
            if (backend == null)
            {
                client.Close();
                semaphore.Release();
                continue;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await _connectionHandler.HandleAsync(client, backend);
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }
    }
}