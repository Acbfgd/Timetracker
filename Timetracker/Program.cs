// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Serialization;

const string fileName = "Arbeitszeit.json";
const double restPeriodHours = 11.0;

var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
var file = Path.Combine(dir, fileName);



var files = Directory.GetFiles(dir);

if (!files.Contains(file))
{
    File.WriteAllText(file, "[]");
}

var text = File.ReadAllText(file);
var days = JsonSerializer.Deserialize<List<Day>>(text);

var today = days.FirstOrDefault(d => d.Start.Day == DateTime.Today.Day);

if (today is null)
{
    //start work
    var t = new Day();

    CheckRestPeriod(t.Start);

    days.Add(t);
    File.WriteAllText(file, JsonSerializer.Serialize(days, new JsonSerializerOptions { WriteIndented = true }));

    Console.Write("Started working at ");
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine(t.Start.ToString("HH:mm dd.MM.yyyy"));
}
else
{
    //Stop work day
    today.End = DateTime.Now.AddHours(3);
    File.WriteAllText(file, JsonSerializer.Serialize(days, new JsonSerializerOptions { WriteIndented = true }));


    var work = today.GetWorkingHours();


    Console.Write("You worked ");
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.Write($"{work:hh\\:mm}");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(" hours today");


    CalculateBreak(today);
}

Console.ForegroundColor = ConsoleColor.White;
Environment.Exit(0);
return;

void CalculateBreak(Day day)
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

    Console.WriteLine("==================================================");
    Console.WriteLine($"You must take a break of at least {breakTime} minutes");
}

void CheckRestPeriod(DateTime start)
{
    var yesterday = days.FirstOrDefault(d => d.Start.Day == (DateTime.Today.Day - 1));
    if (yesterday is null)
    {
        return;
    }

    var restPeriod = start - yesterday.End;

    if (restPeriod.TotalHours >= restPeriodHours)
    {
        return;
    }

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Rest period is only {restPeriod.Hours}:{restPeriod.Minutes}!");
    Console.WriteLine($"You can only start work at {yesterday.End.AddHours(restPeriodHours)}");
    Console.ForegroundColor = ConsoleColor.White;
    Environment.Exit(-1);
}


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
}



//TODO 
// Mehrere Zeiten pro Tag
// Pausenausgabe was eingetragen werden muss
// Config mit format directory, datei etc
// Parametisierter start (--help etc.)