﻿using Discord;
using Discord.WebSocket;
using LevelBot.Code.Drivers;

namespace LevelBot.Code.Discord.Commands;

public class GetLevel : SlashCommand
{
    public GetLevel(DiscordRouter router) : base(router)
    {
    }

    public override Task<SlashCommandProperties> Build(SlashCommandBuilder builder)
    {
        builder.WithName("get");
        builder.WithDescription("Показывает опыт пользователя");

        builder.AddOption("user", ApplicationCommandOptionType.User, 
            "Держатель опыта", isRequired: true);
        
        return Task.FromResult(builder.Build());
    }

    public override async Task Run(SocketSlashCommand command)
    {
        var user = command.Data.Options.ElementAt(0).Value as SocketGuildUser;

        if (user == null)
        {
            await command.RespondAsync("Ошибка получения SocketGuildUser", ephemeral: true);
            return;
        }

        var guildId = command.GuildId;
        if (guildId == null)
        {
            await command.RespondAsync("Ошибка получения guildId",ephemeral: true);
            return;
        }

        var guild = await GuildDriver.OpenAsync(guildId.Value);
        var guildUser = await guild.OpenUserAsync(user.Id);

        var experience = await guildUser.GetExperienceAsync(Router.Logger);
        
        await command.RespondAsync($"У пользователя {user.DisplayName} имеется {experience.Value} опыта");
    }
}