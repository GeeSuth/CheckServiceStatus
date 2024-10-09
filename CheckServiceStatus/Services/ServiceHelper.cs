using System.Net;
using System.Net.Sockets;
using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class ServiceHelper
{
    internal static bool CheckContentExpression(string content, SuccessExpression successExpression)
    {
        if (string.IsNullOrEmpty(content) || successExpression == null || string.IsNullOrEmpty(successExpression.SuccessValue))
        {
            return false;
        }

        switch (successExpression.SuccessExpressionType)
        {
            case SuccessExpressionType.Contains:
                return content.Contains(successExpression.SuccessValue);

            case SuccessExpressionType.StartsWith:
                return content.StartsWith(successExpression.SuccessValue);

            case SuccessExpressionType.EndsWith:
                return content.EndsWith(successExpression.SuccessValue);

            case SuccessExpressionType.Regex:
                return System.Text.RegularExpressions.Regex.IsMatch(content, successExpression.SuccessValue);

            default:
                throw new ArgumentException($"Unsupported SuccessExpressionType: {successExpression.SuccessExpressionType}");
        }
    }

}
