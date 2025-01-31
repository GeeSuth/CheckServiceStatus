using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class HttpServiceHelper
{
   internal static async Task<ServiceResponse> CheckHttpService(ServiceModel service)
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

                HttpResponseMessage response;
                if (service.ServiceRequired?.CommunicationMethod == CommunicationMethod.Post)
                {
                    var content = new StringContent(service.ServiceRequired.RequiredValue ?? "", System.Text.Encoding.UTF8, "application/json");
                    response = await httpClient.PostAsync(service.ServicePath, content);
                }
                else
                {
                    response = await httpClient.GetAsync(service.ServicePath);
                }

                if (service.SuccessExpression != null)
                {
                    switch (service.SuccessExpression.SuccessExpressionType)
                    {
                        case SuccessExpressionType.ResponseCode:
                            if (int.TryParse(service.SuccessExpression.SuccessValue, out int expectedCode))
                            {
                                return new ServiceResponse()
                                {
                                    IsSuccess = (int)response.StatusCode == expectedCode,
                                    ErrorMessage = response.StatusCode.ToString()
                                };
                            }
                            break;
                        case SuccessExpressionType.Contains:
                        case SuccessExpressionType.StartsWith:
                        case SuccessExpressionType.EndsWith:
                        case SuccessExpressionType.Regex:
                            var content = await response.Content.ReadAsStringAsync();
                            var isSuccess = ServiceHelper.CheckContentExpression(content, service.SuccessExpression);
                            return new ServiceResponse()
                            {
                                IsSuccess = isSuccess,
                                ErrorMessage = isSuccess ? response.StatusCode.ToString() :
                                    $"{service.SuccessExpression.SuccessExpressionType} not matched: {service.SuccessExpression.SuccessValue}"
                            };
                    }
                }

                Logs.WriteToLog($"HTTP request to {service.ServiceName} ({service.ServicePath}) successful. Status code: {response.StatusCode}");
                return new ServiceResponse()
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    ErrorMessage = response.StatusCode.ToString()
                };
            }
            catch (Exception ex)
            {
                Logs.WriteToLog($"HTTP request to {service.ServiceName} ({service.ServicePath}) failed: {ex.Message}");
                return new ServiceResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
