using CheckServiceStatus.Ext;
using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public static class Logs
{
    public static void WriteToLog(string message)
    {
        string logFileName = $"logs_{DateTime.Now:yyyyMMdd}.txt";
        string logFilePath = Path.Combine(AppContext.BaseDirectory, "logs" , logFileName);

        if (!Directory.Exists("logs")) Directory.CreateDirectory("logs");

        try
        {
            using (StreamWriter writer = File.AppendText(logFilePath))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }


    public static void WriteTheScan(ServiceModel serviceModel, object response)
    {
        WriteTheScan(serviceModel, response.ToStringJson());
    }

    public static void WriteTheScan(ServiceModel serviceModel, string response)
    {
        string logFileName = $"scan_logs_{DateTime.Now:yyyyMMdd}.txt";
        string logFilePath = Path.Combine(AppContext.BaseDirectory, "scans", logFileName);

        if (!Directory.Exists("scans")) Directory.CreateDirectory("scans");

        try
        {
            using (StreamWriter writer = File.AppendText(logFilePath))
            {
                writer.WriteLine($"\n\r ============================= Start Scan Service: {serviceModel.ServiceName} {DateTime.Now} ============================== \n\r" +
                    $"Outcoming: {serviceModel.ToStringJson()}\n\r - - - - - - - - - - - - - - - - - \n\r" +
                    $"InComing: {response}\n\r " +
                    $"=================================== Finished Scan Service: {serviceModel.ServiceName} ================================================ ");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }
}
