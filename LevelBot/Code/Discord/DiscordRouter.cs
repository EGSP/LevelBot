using Discord;
using Discord.WebSocket;
using LevelBot.Code.Databases.Contexts;
using LevelBot.Code.Discord.Commands;
using LevelBot.Code.Drivers;
using LevelBot.Code.Files;
using LevelBot.Code.Models;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace LevelBot.Code.Discord;

public partial class DiscordRouter
{
    private readonly IDirectory _root;

    private DiscordSocketClient _client;

    private readonly Dictionary<string, SlashCommand> _slashCommands;

    public ILogger Logger { get; set; }

    public DiscordRouter(IDirectory root, ILogger logger)
    {
        _root = root;
        
        Logger = logger;

        GuildDriver.StaticRoot = root.Directory("guilds");
        
        _slashCommands = new Dictionary<string, SlashCommand>();
        _slashCommands.Add("ping", new PingSlash(this));
        _slashCommands.Add("add", new AddLevel(this));
        _slashCommands.Add("set", new SetLevel(this));
        _slashCommands.Add("get", new GetLevel(this));
    }

    public async Task Ini()
    {
        _client = new DiscordSocketClient();

        _client.Log += Log;
        _client.Ready += BuildCommands;
        _client.SlashCommandExecuted += SlashCommandHandler;

        //  You can assign your bot token to a string, and pass that in to connect.
        //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
        var token = await _root.File("discord.txt").ReadOrCreate();
        if (string.IsNullOrEmpty(token))
            return;

        // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
        // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
        // var token = File.ReadAllText("token.txt");
        // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    private async Task SlashCommandHandler(SocketSlashCommand arg)
    {

        if (!_slashCommands.ContainsKey(arg.Data.Name))
        {
            await arg.RespondAsync("Команда не создана в боте");
        }
        else
        {
            var command = _slashCommands[arg.Data.Name];
            await command.Run(arg);
        }
    }

    private async Task BuildCommands()
    {
        var guilds = _client.Guilds;
        
        foreach (var (key, value) in _slashCommands)
        {
            foreach (var guild in guilds)
            {
                try
                {
                    await guild.CreateApplicationCommandAsync(await value.Build(new SlashCommandBuilder()));
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.Message);
                }
            }
        }
    }

    private Task Log(LogMessage arg)
    {
        Logger.LogInformation(arg.Message);
        return Task.CompletedTask;
    }
}