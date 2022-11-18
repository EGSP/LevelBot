using LevelBot.Code.Databases.Contexts;
using LevelBot.Code.Files;
using LevelBot.Code.Models;

namespace LevelBot.Code.Discord;

public partial class DiscordRouter
{
    private readonly IDirectory _guilds;
    private Dictionary<ulong, UserGuildDatabase> Databases { get; }

    public IDirectory GetUserDirectory(GuildUser user) =>
        _guilds.Directory(user.Guild.GuildId.ToString()).Directory(user.UserId.ToString());

    private async Task<UserGuildDatabase> CreateDatabasetAndGuild(ulong guildId)
    {
        var guild = new Guild(guildId);

        var database = new UserGuildDatabase(_guilds.Directory($"{guildId}"), "user-guild-context", Logger);
        await database.Database.EnsureCreatedAsync();
        Databases.Add(guildId, database);
        await database.GetOrCreateGuild(guild);
        return database;
    }

    private async Task<UserGuildDatabase> GetDatabaseAsync(ulong guildId)
    {
        var (key, database) = Databases.FirstOrDefault(x => x.Key == guildId);

        if (database == null)
            database = await CreateDatabasetAndGuild(guildId);
        return database;
    }

    public async Task<Guild> GetGuildAsync(ulong guildId)
    {
        var database = await GetDatabaseAsync(guildId);

        var guild = database.Guilds.FirstOrDefault(x => x.GuildId == guildId);
        if (guild == null)
            throw new InvalidDataException();

        return guild;
    }
    
    public async Task<GuildUser> CreateGuildUserAsync(ulong guildId, ulong userId)
    {
        var database = await GetDatabaseAsync(guildId);
        var guild = await GetGuildAsync(guildId);
        var guildUser = new GuildUser(guild, userId);
        await database.AddUserAsync(guildUser);

        return guildUser;
    }
}