using LoadBalancer.Models;
using System.Net.Sockets;

namespace LoadBalancer.Interfaces;

public interface ITcpConnectionHandler
{
    Task HandleAsync(TcpClient client, BackendServer backend);
}