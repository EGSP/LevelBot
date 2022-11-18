using Discord;
using Discord.WebSocket;

namespace LevelBot.Code.Discord;

public abstract class SlashCommand
{
    protected DiscordRouter Router;

    protected SlashCommand(DiscordRouter router)
    {
        Router = router;
    }

    public abstract Task<SlashCommandProperties> Build(SlashCommandBuilder builder);
    
    public abstract Task Run(SocketSlashCommand command);
}

public class PingSlash : SlashCommand
{
    public override Task<SlashCommandProperties> Build(SlashCommandBuilder builder)
    {
        // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
        builder.WithName("ping");
        // Descriptions can have a max length of 100.
        builder.WithDescription("check life status");

        return Task.FromResult(builder.Build());
    }

    public override async Task Run(SocketSlashCommand command)
    {
        await command.RespondAsync("Пинг получен", ephemeral: true);
    }

    public PingSlash(DiscordRouter router) : base(router)
    {
    }
}