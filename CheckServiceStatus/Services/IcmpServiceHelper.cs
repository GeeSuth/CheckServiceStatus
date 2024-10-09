using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class IcmpServiceHelper
{
       internal static async Task<ServiceResponse> CheckIcmpService(ServiceModel service)
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
                            return new ServiceResponse()
                            {
                                IsSuccess = (int)reply.Status == expectedCode,
                                ErrorMessage = reply.Status.ToString()
                            };
                        }
                        break;
                    case SuccessExpressionType.Contains:
                    case SuccessExpressionType.StartsWith:
                    case SuccessExpressionType.EndsWith:
                    case SuccessExpressionType.Regex:
                        return new ServiceResponse()
                        {
                            IsSuccess = ServiceHelper.CheckContentExpression(reply.Status.ToString(), service.SuccessExpression),
                            ErrorMessage = reply.Status.ToString()
                        };
                }
            }

            Logs.WriteToLog($"ICMP request to {service.ServiceName} ({service.ServicePath}) successful. Status: {reply.Status}");
            return new ServiceResponse()
            {
                IsSuccess = reply.Status == System.Net.NetworkInformation.IPStatus.Success,
                ErrorMessage = reply.Status.ToString()
            };
        }
        catch (Exception ex)
        {
            Logs.WriteToLog($"ICMP request to {service.ServiceName} ({service.ServicePath}) failed: {ex.Message}");
            return new ServiceResponse()
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
