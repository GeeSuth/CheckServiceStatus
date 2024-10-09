// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;
using CheckServiceStatus.Models;
using CheckServiceStatus.Services;
using CheckServiceStatus.Styles;
using Spectre.Console;


ToolInformation.PrintToolInformation();

AnsiConsole.Markup("[underline red]Hello[/] World!");

var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "ServiceList.json"));
var options = new System.Text.Json.JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
};
try
{
    var services = System.Text.Json.JsonSerializer.Deserialize<List<ServiceModel>>(json, options);
    var serviceTalk = new ServiceTalk();
    foreach (var service in services)
    {
        try
        {
            await serviceTalk.CheckServiceStatus(service);
        }
        catch (NotImplementedException ex)
        {
            Console.WriteLine($"NOT_SUPPORT Service {service.ServiceName}: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{service.ServiceName}: {ex.Message}");
        }
    }
}
catch (System.Text.Json.JsonException ex)
{
    Console.WriteLine($"Error deserializing JSON: {ex.Message}");
    Console.WriteLine("JSON content:");
    Console.WriteLine(json);
}

Console.ReadLine();
