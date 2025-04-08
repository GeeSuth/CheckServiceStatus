using System.Net.Sockets;
using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class TelnetServiceHelper
{
     internal static async Task<ServiceResponse> CheckTelnetService(ServiceModel service)
    {
        try
        {
            using var client = new TcpClient();
            var timeout = service.Timeout ?? 5000; // Default to 5 seconds if not specified
            await client.ConnectAsync(service.ServicePath, 23).WaitAsync(TimeSpan.FromMilliseconds(timeout));

            if (client.Connected)
            {

                if (service.writeScanLogs)
                {
                    Logs.WriteTheScan(service, new { TelnetStatus = client.Connected });
                }

                if (service.SuccessExpression != null)
                {
                    using var stream = client.GetStream();
                    var buffer = new byte[256];
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    var response = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    return new ServiceResponse()
                    {
                        IsSuccess = ServiceHelper.CheckContentExpression(response, service.SuccessExpression),
                        ErrorMessage = response
                    };
                }

                Logs.WriteToLog($"Telnet connection to {service.ServiceName} ({service.ServicePath}) successful.");
                return new ServiceResponse()
                {
                    IsSuccess = true,
                    ErrorMessage = "Telnet connection successful"
                };
            }
            else
            {
                Logs.WriteToLog($"Telnet connection to {service.ServiceName} ({service.ServicePath}) failed.");
                return new ServiceResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = "Telnet connection failed"
                };
            }
        }
        catch (Exception ex)
        {
            Logs.WriteToLog($"Telnet connection to {service.ServiceName} ({service.ServicePath}) failed: {ex.Message}");
            return new ServiceResponse()
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
