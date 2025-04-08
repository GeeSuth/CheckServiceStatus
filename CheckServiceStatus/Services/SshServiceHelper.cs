using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class SshServiceHelper
{
       internal static async Task<ServiceResponse> CheckSshService(ServiceModel service)
    {
        try
        {
            using var client = new Renci.SshNet.SshClient(service.ServicePath, service.AuthenticationValue);
            var timeout = service.Timeout ?? 5000; // Default to 5 seconds if not specified
            client.ConnectionInfo.Timeout = TimeSpan.FromMilliseconds(timeout);

            await Task.Run(() => client.Connect());

            if (service.writeScanLogs)
            {
                Logs.WriteTheScan(service,  new { SshConnect= client.IsConnected });
            }

            if (client.IsConnected)
            {
                if (service.SuccessExpression != null)
                {
                    var commandResult = client.RunCommand("echo SSH connection successful");
                    return new ServiceResponse()
                    {
                        IsSuccess = ServiceHelper.CheckContentExpression(commandResult.Result, service.SuccessExpression),
                        ErrorMessage = commandResult.Result
                    };
                }

                Logs.WriteToLog($"SSH connection to {service.ServiceName} ({service.ServicePath}) successful.");
                return new ServiceResponse()
                {
                    IsSuccess = true,
                    ErrorMessage = "SSH connection successful"
                };
            }
            else
            {
                Logs.WriteToLog($"SSH connection to {service.ServiceName} ({service.ServicePath}) failed.");
                return new ServiceResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = "SSH connection failed"
                };
            }
        }
        catch (Exception ex)
        {
            Logs.WriteToLog($"SSH connection to {service.ServiceName} ({service.ServicePath}) failed: {ex.Message}");
            return new ServiceResponse()
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
