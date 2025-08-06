using LoadBalancer.Interfaces;
using LoadBalancer.Models;
using System.Net.Sockets;

namespace LoadBalancer.Services;

public class TcpConnectionHandler : ITcpConnectionHandler
{
    private const int MaxRetries = 3;
    private const int InitialDelayMs = 200;

    public async Task HandleAsync(TcpClient client, BackendServer backend)
    {
        using var server = new TcpClient();
        var connected = false;

        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                await server.ConnectAsync(backend.Ip, backend.Port);
                connected = true;
                break;
            }
            catch
            {
                var delay = InitialDelayMs * (int)Math.Pow(2, attempt - 1);
                await Task.Delay(delay);
            }
        }

        if (!connected)
        {
            client.Close();
            return;
        }

        backend.IncrementConnections();

        try
        {
            var clientStream = client.GetStream();
            var serverStream = server.GetStream();

            var t1 = clientStream.CopyToAsync(serverStream);
            var t2 = serverStream.CopyToAsync(clientStream);
            await Task.WhenAny(t1, t2);
        }
        finally
        {
            client.Close();
            server.Close();
            backend.DecrementConnections();
        }
    }
}