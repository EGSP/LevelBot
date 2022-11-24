using LevelBot.Code.Databases;
using LevelBot.Code.Databases.Contexts;
using LevelBot.Code.Files;
using LevelBot.Code.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace LevelBot.Code.Drivers;

public class GuildDriver
{
    public readonly Guild Guild;

    private GuildDriver(Guild guild)
    {
        Guild = guild;
    }
    static GuildDriver()
    {
        Databases = new Dictionary<ulong, UserGuildDatabase>();
    }

    public static Dictionary<ulong, UserGuildDatabase> Databases { get; }
    public static IDirectory Root { get; set; }

    public static async Task<GuildDriver> OpenAsync(ulong guildId)
    {
        var guild = await GetGuildAsync(guildId);
        var driver = new GuildDriver(guild);
        return driver;
    }

    private static async Task<UserGuildDatabase> CreateDatabasetForGuild(ulong guildId)
    {
        var guild = new Guild(guildId);

        var database = new UserGuildDatabase(Root.Directory($"{guildId}"), "user-guild-context", NullLogger.Instance);
        await database.Database.EnsureCreatedAsync();
        
        Databases.Add(guildId, database);
        
        await database.GetOrCreateGuild(guild);
        return database;
    }
    
    private static async Task<UserGuildDatabase> GetDatabaseAsync(ulong guildId)
    {
        var (key, database) = Databases.FirstOrDefault(x => x.Key == guildId);

        if (database == null)
            database = await CreateDatabasetForGuild(guildId);
        return database;
    }
    
    private static async Task<Guild> GetGuildAsync(ulong guildId)
    {
        var database = await GetDatabaseAsync(guildId);

        var guild = database.Guilds.FirstOrDefault(x => x.GuildId == guildId);
        if (guild == null)
            throw new InvalidDataException();

        return guild;
    }
    
    public static IDirectory GetUserDirectory(UserDriver driver) =>
        Root.Directory(driver.User.Guild.GuildId.ToString()).Directory(driver.User.UserId.ToString());
    
    public async Task<GuildUser> CreateGuildUserAsync(ulong guildId, ulong userId)
    {
        var database = await GetDatabaseAsync(guildId);
        var guild = await GetGuildAsync(guildId);
        var guildUser = new GuildUser(guild, userId);
        await database.AddUserAsync(guildUser);

        return guildUser;
    }

    public async Task<UserDriver> OpenUserAsync(ulong userId)
    {
        var guildUser = Guild.GuildUsers.FirstOrDefault(x=>x.UserId == userId);
        if (guildUser == null)
            guildUser = await CreateGuildUserAsync(Guild.GuildId, userId);

        var driver = new UserDriver(guildUser);
        return driver;
    }
}