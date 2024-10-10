using System.Text.Json;
using CheckServiceStatus.Models;
using Spectre.Console;

namespace CheckServiceStatus.Services.FileServices;

public class JsonFileService
{
    public static List<ServiceModel> ReadJsonFile()
    {
        try
        {
            var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "ServiceList.json"));
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };
            var services = System.Text.Json.JsonSerializer.Deserialize<List<ServiceModel>>(json, options);
            return services;
        }
        catch(FileNotFoundException)
        {
            AnsiConsole.MarkupLine("[red]Error: ServiceList.json file not found.[/]");
            AnsiConsole.MarkupLine("Creating a new ServiceList.json file for you with an example service...");
      
            var exampleService = new ServiceModel
            {
                ServiceName = "Example HTTP Service",
              ServicePath = "https://example.com",
              CommunicationType = CommunicationType.Http,
              Timeout = 30,
              SuccessExpression = new SuccessExpression
              {
                  SuccessExpressionType = SuccessExpressionType.ResponseCode,
                  SuccessValue = "200"
              }
          };
      
          var exampleList = new List<ServiceModel> { exampleService };
          var jsonOptions = new System.Text.Json.JsonSerializerOptions
          {
              WriteIndented = true,
              Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
          };
          var exampleJson = System.Text.Json.JsonSerializer.Serialize(exampleList, jsonOptions);
      
          File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "ServiceList.json"), exampleJson);
      
          AnsiConsole.MarkupLine("[green]ServiceList.json created successfully with an example service.[/]");
          AnsiConsole.MarkupLine("Please edit the file to add your own services and run the program again.");
          Environment.Exit(0);
          return null;
        }
    }
}