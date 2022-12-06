using Automata.IO;
using LevelBot.Code.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelBot.Code.Databases.Contexts;

public class UserGuildDatabase : Database
{
    public DbSet<Guild> Guilds { get; set; }
    public DbSet<GuildUser> GuildUsers { get; set; }
    public UserGuildDatabase(IDirectory container, string name, ILogger logger) : base(container, name, logger)
    {
    }

    protected override void ModelCreate(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Guild>().HasKey(x => x.GuildId);

        modelBuilder.Entity<GuildUser>().HasKey(x => x.UserId);

        modelBuilder.Entity<Guild>().Navigation(x => x.GuildUsers).AutoInclude();
        
        modelBuilder.Entity<Guild>()
            .HasMany<GuildUser>(x=>x.GuildUsers)
            .WithOne(x=>x.Guild);
    }

    public async Task<Guild> GetOrCreateGuild(Guild guild)
    {
        if (guild is null)
            return null;

        var guildEntry = await Guilds.FirstOrDefaultAsync(x=>x.GuildId == guild.GuildId);

        if (guildEntry is null)
        {
            guildEntry = Add(guild).Entity;
            await Save();
        }

        return guildEntry;
    }

    public async Task AddUserAsync(GuildUser user)
    {
        if (user is null)
            return;
        //
        // await CashDatabase.WriteRun(async () =>
        // {
        var guild = user.Guild;

        var guildEntry = this.Guilds
            .FirstOrDefault(x => x.GuildId == user.Guild.GuildId);
        var userEntry = guildEntry.GuildUsers
            .FirstOrDefault(x => x.UserId == user.UserId);

        if (guildEntry is null)
        {
            guildEntry = this.Add(guild).Entity;
        }

        user.Guild = guildEntry;

        if (userEntry is not null)
        {
            this.Entry(userEntry).CurrentValues.SetValues(user);
        }
        else
        {
            userEntry = Add<GuildUser>(user).Entity;
        }

        await this.Save();

        // });
    }
}

