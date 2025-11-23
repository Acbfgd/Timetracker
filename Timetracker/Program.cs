using Spectre.Console.Cli;
using System.Text.Json.Serialization;
using Timetracker.Commands;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<ListCommand>("list")
        .WithAlias("l")
        .WithDescription("lists all existing working days")
        .WithExample("l", "list");

    config.AddCommand<StartCommand>("start")
        .WithAlias("s")
        .WithDescription("start tracking your work day")
        .WithExample("s", "start");

    config.AddCommand<EndCommand>("end")
        .WithAlias("e")
        .WithDescription("end your work day")
        .WithExample("e", "end");
});

return app.Run(args);

internal class Day
{
    public DateTime Start { get; }
    public DateTime End { get; set; }

    public Day()
    {
        Start = DateTime.Now;
    }

    [JsonConstructor]
    public Day(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public TimeSpan GetWorkingHours()
    {
        return End - Start;
    }

    public override string ToString()
        => $"{Start:ddd dd.MM.yy}\t{Start:HH:mm} - {End:HH:mm}";

}
