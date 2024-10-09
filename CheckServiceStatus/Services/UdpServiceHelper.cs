using System.Net;
using System.Net.Sockets;
using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class UdpServiceHelper
{
     internal static async Task<ServiceResponse> CheckUdpService(ServiceModel service)
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
                    return new ServiceResponse()
                    {
                        IsSuccess = ServiceHelper.CheckContentExpression(response, service.SuccessExpression),
                        ErrorMessage = response
                    };
                }

                Logs.WriteToLog($"UDP connection to {service.ServiceName} ({service.ServicePath}) successful.");
                return new ServiceResponse()
                {
                    IsSuccess = true,
                    ErrorMessage = "UDP connection successful"
                };
            }
            else
            {
                Logs.WriteToLog($"UDP connection to {service.ServiceName} ({service.ServicePath}) timed out.");
                return new ServiceResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = "UDP connection timed out"
                };
            }
        }
        catch (Exception ex)
        {
            Logs.WriteToLog($"UDP connection to {service.ServiceName} ({service.ServicePath}) failed: {ex.Message}");
            return new ServiceResponse()
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
