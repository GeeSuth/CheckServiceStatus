using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class SshServiceHelper
{
       internal static async Task<bool> CheckSshService(ServiceModel service)
    {
        try
        {
            using var client = new Renci.SshNet.SshClient(service.ServicePath, service.AuthenticationValue);
            var timeout = service.Timeout ?? 5000; // Default to 5 seconds if not specified
            client.ConnectionInfo.Timeout = TimeSpan.FromMilliseconds(timeout);

            await Task.Run(() => client.Connect());

            if (client.IsConnected)
            {
                if (service.SuccessExpression != null)
                {
                    var commandResult = client.RunCommand("echo SSH connection successful");
                    return ServiceHelper.CheckContentExpression(commandResult.Result, service.SuccessExpression);
                }

                Console.WriteLine($"SSH connection to {service.ServiceName} ({service.ServicePath}) successful.");
                return true;
            }
            else
            {
                Console.WriteLine($"SSH connection to {service.ServiceName} ({service.ServicePath}) failed.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SSH connection to {service.ServiceName} ({service.ServicePath}) failed: {ex.Message}");
            return false;
        }
    }
}
