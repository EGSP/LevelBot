using Discord;
using Discord.WebSocket;
using LevelBot.Code.Databases.Contexts;
using LevelBot.Code.Models;

namespace LevelBot.Code.Discord.Commands;

public class AddLevel : SlashCommand
{
    public AddLevel(DiscordRouter router) : base(router)
    {
    }
    
    public override Task<SlashCommandProperties> Build(SlashCommandBuilder builder)
    {
        builder.WithName("add");
        builder.WithDescription("Добавляет опыт пользовтелю");

        builder.AddOption("expirience", ApplicationCommandOptionType.Integer,
            description: "количество добавляемоего опыта", isRequired: true,
            minValue: 0, maxValue: int.MaxValue - 1);
        builder.AddOption("user", ApplicationCommandOptionType.User, 
            "Получатель опыта", isRequired: true);
        
        return Task.FromResult(builder.Build());
    }

    public override async Task Run(SocketSlashCommand command)
    {
        var exp = (long)command.Data.Options.ElementAt(0).Value;
        var user = command.Data.Options.ElementAt(1).Value as SocketGuildUser;

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
        
        var guild = await Router.GetGuildAsync(guildId.Value);
        var guildUser = guild.GuildUsers.FirstOrDefault(x=>x.UserId == user.Id);
        if (guildUser == null)
        {
            await Router.CreateGuildUserAsync(guildId.Value, user.Id);
        }
        guildUser = guild.GuildUsers.FirstOrDefault(x=>x.UserId == user.Id);

        var userDirectory = Router.GetUserDirectory(guildUser);
        var userExpHistory = new UserExperienceDatabase(userDirectory, "experience-history", Router.Logger);
        await userExpHistory.Database.EnsureCreatedAsync();
        
        var experienceOperation = new ExperienceOperation(new Experience(exp), OperationType.Add, DateTime.Now);
        await userExpHistory.AddAsync(experienceOperation);
            

        await command.RespondAsync($"Вы добавили {exp} опыта пользовтелю {user.DisplayName}");
    }
}