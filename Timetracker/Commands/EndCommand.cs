using Spectre.Console;
using Spectre.Console.Cli;
using System.Text.Json;

namespace Timetracker.Commands;
internal class EndCommand : Command
{
    private const string FileName = "Arbeitszeit.json";
    private List<Day> _days = [];

    protected override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        //End work day
        var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var file = Path.Combine(dir, FileName);

        var text = File.ReadAllText(file);
        _days = JsonSerializer.Deserialize<List<Day>>(text);


        var today = _days.FirstOrDefault(d => d.Start.Day == DateTime.Today.Day);
        today.End = DateTime.Now;
        File.WriteAllText(file, JsonSerializer.Serialize(_days, new JsonSerializerOptions { WriteIndented = true }));

        var work = today.GetWorkingHours();

        AnsiConsole.MarkupLine($"You worked [green]{work:hh\\:mm}[/] hours today");
        CalculateBreak(today);

        return 0;
    }


    private void CalculateBreak(Day day)
    {

        var work = day.GetWorkingHours();
        var breakTime = 0;
        switch (work.TotalHours)
        {
            case <= 6.0:
                breakTime = 0;
                break;
            case <= 9.0:
                breakTime = 30;
                break;
            default:
                breakTime = 45;
                break;
        }

        AnsiConsole.WriteLine("==================================================");
        AnsiConsole.MarkupLine($"You must take a break of at least [yellow]{breakTime}[/] minutes");
    }

}
