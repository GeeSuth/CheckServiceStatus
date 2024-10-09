using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class IcmpServiceHelper
{
       internal static async Task<bool> CheckIcmpService(ServiceModel service)
    {
        try
        {
            using var ping = new System.Net.NetworkInformation.Ping();
            var timeout = service.Timeout ?? 5000; // Default to 5 seconds if not specified
            var reply = await ping.SendPingAsync(service.ServicePath, timeout);

            if (service.SuccessExpression != null)
            {
                switch (service.SuccessExpression.SuccessExpressionType)
                {
                    case SuccessExpressionType.ResponseCode:
                        if (int.TryParse(service.SuccessExpression.SuccessValue, out int expectedCode))
                        {
                            return (int)reply.Status == expectedCode;
                        }
                        break;
                    case SuccessExpressionType.Contains:
                    case SuccessExpressionType.StartsWith:
                    case SuccessExpressionType.EndsWith:
                    case SuccessExpressionType.Regex:
                        return ServiceHelper.CheckContentExpression(reply.Status.ToString(), service.SuccessExpression);
                }
            }

            Console.WriteLine($"ICMP request to {service.ServiceName} ({service.ServicePath}) successful. Status: {reply.Status}");
            return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ICMP request to {service.ServiceName} ({service.ServicePath}) failed: {ex.Message}");
            return false;
        }
    }
}
