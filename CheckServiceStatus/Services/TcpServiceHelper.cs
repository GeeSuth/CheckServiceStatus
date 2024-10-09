using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class TcpServiceHelper
{
       public static async Task<ServiceResponse> CheckTcpService(ServiceModel service)
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
                Logs.WriteToLog($"TCP connection to {service.ServiceName} ({hostname}:{port}) successful.");
                return new ServiceResponse()
                {
                    IsSuccess = true,
                    ErrorMessage = "TCP connection successful"
                };
            }
            catch (Exception ex)
            {
                Logs.WriteToLog($"TCP connection to {service.ServiceName} ({hostname}:{port}) failed: {ex.Message}");
                return new ServiceResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
