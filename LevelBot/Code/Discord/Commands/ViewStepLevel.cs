using Discord;
using Discord.WebSocket;
using LevelBot.Code.Drivers;

namespace LevelBot.Code.Discord.Commands;

public class ViewStepLevel : SlashCommand
{
    public ViewStepLevel(DiscordRouter router) : base(router)
    {
    }

    public override Task<SlashCommandProperties> Build(SlashCommandBuilder builder)
    {
        builder.WithName("view-step");
        builder.WithDescription("Посмотреть шаг повышения уровня");
        
        return Task.FromResult(builder.Build());
    }

    public override async Task Run(SocketSlashCommand command)
    {
        var guildId = command.GuildId;
        if (guildId == null)
        {
            await command.RespondAsync("Ошибка получения guildId",ephemeral: true);
            return;
        }

        var guild = await GuildDriver.OpenAsync(guildId.Value);
        var step = await guild.GetPropertyAsync("step", Router.Logger);

        await command.RespondAsync($"Установлено значение шага {step}");
    }
}