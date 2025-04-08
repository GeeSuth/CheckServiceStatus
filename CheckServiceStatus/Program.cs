// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;
using CheckServiceStatus.Models;
using CheckServiceStatus.Services;
using CheckServiceStatus.Services.FileServices;
using CheckServiceStatus.Styles;
using Spectre.Console;

bool forcementScan = false;
ToolInformation.PrintToolOwner();
ToolInformation.PrintToolInformation();

AnsiConsole.Write(new Markup("Version: v2025.04.08 \n\r", new Style(Color.White)));




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



scan:
    await AnsiConsole.Live(table)
        .StartAsync(async ctx =>
        {
            int serviceIndex = 0;
            foreach (var service in services)
            {
                serviceIndex++;

                try
                {
                    if(!service.Enabled && !forcementScan)
                    {
                        table.AddRow(
                            new Markup($"[bold]{serviceIndex}[/]"),
                            new Markup($"[bold]{service.ServiceName}[/]"),
                            new Markup($"[Yellow][bold]Skipped[/][/]"),
                            new Markup($"[italic]{service.CommunicationType}[/]"),
                            new Markup($"[underline]{(service.ServiceRequired?.CommunicationMethod != CommunicationMethod.Basic ? service.ServiceRequired?.CommunicationMethod.ToString() + "@" : "")}{service.ServicePath}[/]"),
                            new Markup($"-"),
                            new Markup($"Service Not Enabled, if you want to scan it please ensure to make this service enabled from JSON file or scan with /f to force scan.")
                        );
                        continue;
                    }

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
                        new Markup($"[underline]{(service.ServiceRequired?.CommunicationMethod != CommunicationMethod.Basic ? service.ServiceRequired?.CommunicationMethod.ToString()+"@" : "")}{service.ServicePath}[/]"),
                        new Markup($"[dim]{timeSpent.ToString(@"mm\:ss\.fff")}[/]"),
                        new Markup($"{result.ErrorMessage}")
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
                
            }
        });

    AnsiConsole.Write(new Markup("[bold]Finished Scan!![/]", new Style(Color.White, Color.Green, new Decoration())));
    AnsiConsole.WriteLine();
    AnsiConsole.Write(new Markup("[yellow] ** If you want more details, please go to logs or scan file.[/]"));

    AnsiConsole.WriteLine();

    
    
    while(true)
    {
        forcementScan = false;
        var ask = AnsiConsole.Ask<string>("\n\r Want to scan Again? write / for normal scan /f for force scan and press Enter? ");
        if (ask == "/" || ask == "/f")
        {
            services = JsonFileService.ReadJsonFile();
            table.Rows.Clear();

            if(ask == "/f")
            {
                forcementScan = true;
            }
            goto scan;
        }
    }


}
catch (System.Text.Json.JsonException ex)
{
    Console.WriteLine($"Error deserializing JSON: {ex.Message}");
    Console.WriteLine("JSON content:");
    Console.WriteLine(JsonFileService.ReadJsonFile());
}

Console.ReadLine();
