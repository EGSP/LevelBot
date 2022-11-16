using Discord;
using Discord.WebSocket;

namespace LevelBot.Code.Discord;

public abstract class SlashCommand
{
    public abstract Task<SlashCommandProperties> Build(SlashCommandBuilder builder);
    
    public abstract Task Run(SocketSlashCommand command);
}

public class PingSlash : SlashCommand
{
    public override Task<SlashCommandProperties> Build(SlashCommandBuilder builder)
    {
        var guildCommand = new SlashCommandBuilder();
        
        // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
        guildCommand.WithName("ping");
        // Descriptions can have a max length of 100.
        guildCommand.WithDescription("check life status");

        return Task.FromResult(guildCommand.Build());
    }

    public override async Task Run(SocketSlashCommand command)
    {
        await command.RespondAsync("Пинг получен", ephemeral: true);
    }
}