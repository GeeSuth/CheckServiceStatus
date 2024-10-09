using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class TcpServiceHelper
{
       public static async Task<bool> CheckTcpService(ServiceModel service)
    {
        if (service == null || string.IsNullOrEmpty(service.ServicePath))
        {
            throw new ArgumentException("Invalid service configuration");
        }

        string[] parts = service.ServicePath.Split(':');
        if (parts.Length != 2 || !int.TryParse(parts[1], out int port))
        {
            throw new ArgumentException("Invalid TCP service path. Expected format: hostname:port");
        }

        string hostname = parts[0];
        
        using (var tcpClient = new System.Net.Sockets.TcpClient())
        {
            try
            {
                await tcpClient.ConnectAsync(hostname, port);
                Console.WriteLine($"TCP connection to {service.ServiceName} ({hostname}:{port}) successful.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP connection to {service.ServiceName} ({hostname}:{port}) failed: {ex.Message}");
                return false;
            }
        }
    }
}
