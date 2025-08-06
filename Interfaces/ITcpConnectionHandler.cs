using Load_Balancer.Models;
using System.Net.Sockets;

namespace Load_Balancer.Interfaces;

public interface ITcpConnectionHandler
{
    Task HandleAsync(TcpClient client, BackendServer backend);
}