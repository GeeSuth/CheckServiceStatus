using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class HttpServiceHelper
{
   internal static async Task<bool> CheckHttpService(ServiceModel service)
    {
        if (service == null || string.IsNullOrEmpty(service.ServicePath))
        {
            throw new ArgumentException("Invalid service configuration");
        }

        using (var httpClient = new HttpClient())
        {
            try
            {
                httpClient.Timeout = TimeSpan.FromSeconds(service.Timeout ?? 30);

                if (service.AuthenticationType.HasValue)
                {
                    switch (service.AuthenticationType.Value)
                    {
                        case AuthenticationType.Basic:
                            var authBytes = System.Text.Encoding.ASCII.GetBytes(service.AuthenticationValue);
                            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
                            break;
                        case AuthenticationType.Bearer:
                            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", service.AuthenticationValue);
                            break;
                        // Add other authentication types as needed
                    }
                }

                var response = await httpClient.GetAsync(service.ServicePath);

                if (service.SuccessExpression != null)
                {
                    switch (service.SuccessExpression.SuccessExpressionType)
                    {
                        case SuccessExpressionType.ResponseCode:
                            if (int.TryParse(service.SuccessExpression.SuccessValue, out int expectedCode))
                            {
                                return (int)response.StatusCode == expectedCode;
                            }
                            break;
                        case SuccessExpressionType.Contains:
                        case SuccessExpressionType.StartsWith:
                        case SuccessExpressionType.EndsWith:
                        case SuccessExpressionType.Regex:
                            var content = await response.Content.ReadAsStringAsync();
                            return ServiceHelper.CheckContentExpression(content, service.SuccessExpression);
                    }
                }

                Console.WriteLine($"HTTP request to {service.ServiceName} ({service.ServicePath}) successful. Status code: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HTTP request to {service.ServiceName} ({service.ServicePath}) failed: {ex.Message}");
                return false;
            }
        }
    }
}
