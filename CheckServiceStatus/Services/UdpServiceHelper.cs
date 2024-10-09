using System.Net;
using System.Net.Sockets;
using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class UdpServiceHelper
{
     internal static async Task<bool> CheckUdpService(ServiceModel service)
    {
        try
        {
            using var udpClient = new UdpClient();
            var timeout = service.Timeout ?? 5000; // Default to 5 seconds if not specified
            var uri = new Uri(service.ServicePath);
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port);

            await udpClient.SendAsync(new byte[1], 1, ipEndPoint);

            var receiveTask = udpClient.ReceiveAsync();
            var timeoutTask = Task.Delay(timeout);

            var completedTask = await Task.WhenAny(receiveTask, timeoutTask);

            if (completedTask == receiveTask)
            {
                var result = await receiveTask;
                var response = System.Text.Encoding.ASCII.GetString(result.Buffer);

                if (service.SuccessExpression != null)
                {
                    return ServiceHelper.CheckContentExpression(response, service.SuccessExpression);
                }

                Console.WriteLine($"UDP connection to {service.ServiceName} ({service.ServicePath}) successful.");
                return true;
            }
            else
            {
                Console.WriteLine($"UDP connection to {service.ServiceName} ({service.ServicePath}) timed out.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UDP connection to {service.ServiceName} ({service.ServicePath}) failed: {ex.Message}");
            return false;
        }
    }
}
