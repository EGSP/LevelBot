using LevelBot.Code.Data;
using LevelBot.Code.Discord;
using LevelBot.Code.Files;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var serilogLogger = CreateSerilog();
var msLogger = new LoggerFactory().AddSerilog(serilogLogger).CreateLogger("main-logger");

msLogger.LogWarning("Test");

var discordrouter = new DiscordRouter(
    new DirectoryContainer($"{Environment.CurrentDirectory}/content/").Directory("discord"),msLogger);
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