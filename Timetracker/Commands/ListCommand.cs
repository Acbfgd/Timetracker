using Spectre.Console;
using Spectre.Console.Cli;
using System.Text.Json;
using Timetracker.Settings;

namespace Timetracker.Commands;
internal class ListCommand : Command<ListSettings>
{
    const string fileName = "Arbeitszeit.json";


    protected override int Execute(CommandContext context, ListSettings settings, CancellationToken cancellationToken)
    {
        var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var file = Path.Combine(dir, fileName);

        var text = File.ReadAllText(file);
        var days = JsonSerializer.Deserialize<List<Day>>(text);

        days.ForEach(d =>
        {
            AnsiConsole.MarkupLine(d.ToString());
        });

        return 0;
    }
}
