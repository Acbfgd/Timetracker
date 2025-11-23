using Spectre.Console;
using Spectre.Console.Cli;
using System.Text.Json;

namespace Timetracker.Commands;
internal class StartCommand : Command
{
    private const string FileName = "Arbeitszeit.json";
    private const double RestPeriodHours = 11.0;
    private List<Day> _days = [];

    protected override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var file = Path.Combine(dir, FileName);


        var files = Directory.GetFiles(dir);
        if (!files.Contains(file))
        {
            File.WriteAllText(file, "[]");
        }

        var text = File.ReadAllText(file);
        _days = JsonSerializer.Deserialize<List<Day>>(text);



        var today = _days.FirstOrDefault(d => d.Start.Day == DateTime.Today.Day);
        if (today is null)
        {
            //start work
            var t = new Day();

            CheckRestPeriod(t.Start);

            _days.Add(t);
            File.WriteAllText(file, JsonSerializer.Serialize(_days, new JsonSerializerOptions { WriteIndented = true }));

            AnsiConsole.MarkupLine($"Started working at [green]{t.Start:HH:mm dd.MM.yyyy}[/]");
        }
        Console.ReadKey();
        return 0;
    }

    private void CheckRestPeriod(DateTime start)
    {
        var yesterday = _days.FirstOrDefault(d => d.Start.Day == (DateTime.Today.Day - 1));
        if (yesterday is null)
        {
            return;
        }

        var restPeriod = start - yesterday.End;

        if (restPeriod.TotalHours >= RestPeriodHours)
        {
            return;
        }

        AnsiConsole.Markup("[red]");
        AnsiConsole.MarkupLine($"Rest period is only {restPeriod.Hours}:{restPeriod.Minutes}!");
        AnsiConsole.MarkupLine($"You can only start work at {yesterday.End.AddHours(RestPeriodHours)}");
        AnsiConsole.Markup("[/]");

        Console.ReadKey();
        Environment.Exit(-1);
    }
}
