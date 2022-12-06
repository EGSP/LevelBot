using Automata.IO;
using LevelBot.Code.Data;
using LevelBot.Code.Discord;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Directory = Automata.IO.Directory;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var serilogLogger = CreateSerilog();
var msLogger = new LoggerFactory().AddSerilog(serilogLogger).CreateLogger("main-logger");

msLogger.LogWarning("Test");

var discordrouter = new DiscordRouter(
    new Directory($"{Environment.CurrentDirectory}/content/").Directory("discord"),msLogger);
await discordrouter.Ini();

app.Run();


Serilog.ILogger CreateSerilog()
{
    return new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
        .WriteTo.File($"{Structure.Logs.Path}/logs.txt",rollingInterval: RollingInterval.Day)
        .CreateLogger();
}