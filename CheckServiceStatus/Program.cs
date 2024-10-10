// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;
using CheckServiceStatus.Models;
using CheckServiceStatus.Services;
using CheckServiceStatus.Services.FileServices;
using CheckServiceStatus.Styles;
using Spectre.Console;


ToolInformation.PrintToolOwner();
ToolInformation.PrintToolInformation();

var services = JsonFileService.ReadJsonFile();

try
{
    var serviceTalk = new ServiceTalk();
    var table = new Table()
        .AddColumn("#", c => c.Width(3))
        .AddColumn("Service Name")
        .AddColumn("Status")
        .AddColumn("Communication Type")
        .AddColumn("Path")
        .AddColumn("Time Spent")
        .AddColumn("More");

    table.Border(TableBorder.Rounded);

    await AnsiConsole.Live(table)
        .StartAsync(async ctx =>
        {
            int serviceIndex = 1;
            foreach (var service in services)
            {
                try
                {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var result = await serviceTalk.CheckServiceStatus(service);
                    stopwatch.Stop();
                    var timeSpent = stopwatch.Elapsed;
                    var status = result.IsSuccess ? "UP" : "DOWN";
                    var color = result.IsSuccess ? "green" : "red";

                    table.AddRow(
                        new Markup($"[bold]{serviceIndex}[/]"),
                        new Markup($"[bold]{service.ServiceName}[/]"),
                        new Markup($"[{color}][bold]{status}[/][/]"),
                        new Markup($"[italic]{service.CommunicationType}[/]"),
                        new Markup($"[underline]{service.ServicePath}[/]"),
                        new Markup($"[dim]{timeSpent.ToString(@"mm\:ss\.fff")}[/]"),
                        new Markup($"[invert]{result.ErrorMessage}[/]")
                    );
                }
                catch (NotImplementedException ex)
                {
                    table.AddRow(
                        new Markup($"[bold]{serviceIndex}[/]"),
                        new Markup($"[bold]{service.ServiceName}[/]"),
                        new Markup("[yellow]NOT_SUPPORTED[/]"),
                        new Markup($"[italic]{service.CommunicationType}[/]"),
                        new Markup($"[underline]{service.ServicePath}[/]"),
                        new Markup("N/A"),
                        new Markup(ex.Message)
                    );
                }
                catch (Exception ex)
                {
                    table.AddRow(
                        new Markup($"[bold]{serviceIndex}[/]"),
                        new Markup($"[bold]{service.ServiceName}[/]"),
                        new Markup("[red]ERROR[/]"),
                        new Markup($"[italic]{service.CommunicationType}[/]"),
                        new Markup($"[underline]{service.ServicePath}[/]"),
                        new Markup("N/A"),
                        new Markup(ex.Message)
                    );
                }

                ctx.Refresh();
                //await Task.Delay(100); // Add a small delay to make the live update visible
                serviceIndex++;
            }
        });
}
catch (System.Text.Json.JsonException ex)
{
    Console.WriteLine($"Error deserializing JSON: {ex.Message}");
    Console.WriteLine("JSON content:");
    Console.WriteLine(JsonFileService.ReadJsonFile());
}

Console.ReadLine();
