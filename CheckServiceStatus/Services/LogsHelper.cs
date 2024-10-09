namespace CheckServiceStatus.Services;

public static class Logs
{
    public static void WriteToLog(string message)
    {
        string logFileName = $"logs_{DateTime.Now:yyyyMMdd}.txt";
        string logFilePath = Path.Combine(AppContext.BaseDirectory, logFileName);
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
}
