using System.Net.Sockets;
using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class TelnetServiceHelper
{
     internal static async Task<bool> CheckTelnetService(ServiceModel service)
    {
        try
        {
            using var client = new TcpClient();
            var timeout = service.Timeout ?? 5000; // Default to 5 seconds if not specified
            await client.ConnectAsync(service.ServicePath, 23).WaitAsync(TimeSpan.FromMilliseconds(timeout));

            if (client.Connected)
            {
                if (service.SuccessExpression != null)
                {
                    using var stream = client.GetStream();
                    var buffer = new byte[256];
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    var response = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    return ServiceHelper.CheckContentExpression(response, service.SuccessExpression);
                }

                Console.WriteLine($"Telnet connection to {service.ServiceName} ({service.ServicePath}) successful.");
                return true;
            }
            else
            {
                Console.WriteLine($"Telnet connection to {service.ServiceName} ({service.ServicePath}) failed.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Telnet connection to {service.ServiceName} ({service.ServicePath}) failed: {ex.Message}");
            return false;
        }
    }
}
