using Discord;
using Discord.WebSocket;
using LevelBot.Code.Drivers;

namespace LevelBot.Code.Discord.Commands;

public class StepLevel : SlashCommand
{
    public StepLevel(DiscordRouter router) : base(router)
    {
    }

    public override Task<SlashCommandProperties> Build(SlashCommandBuilder builder)
    {
        builder.WithName("step");
        builder.WithDescription("Устанавливает шаг повышения уровня");

        builder.AddOption("step", ApplicationCommandOptionType.Integer,
            description: "Шаг повышения уровня", isRequired: true,
            minValue: 1, maxValue: 10000);
        
        return Task.FromResult(builder.Build());
    }

    public override async Task Run(SocketSlashCommand command)
    {
        var step = Convert.ToUInt64(command.Data.Options.ElementAt(0).Value);

        var guildId = command.GuildId;
        if (guildId == null)
        {
            await command.RespondAsync("Ошибка получения guildId",ephemeral: true);
            return;
        }

        var guild = await GuildDriver.OpenAsync(guildId.Value);
        await guild.SetPropertyAsync("step", step.ToString(), Router.Logger);

        await command.RespondAsync($"Шаг установлен на значение {step}");
    }
}