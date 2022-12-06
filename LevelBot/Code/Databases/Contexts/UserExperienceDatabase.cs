using Automata.IO;
using LevelBot.Code.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelBot.Code.Databases.Contexts;

public class UserExperienceDatabase : Database
{
    public DbSet<ExperienceOperation> ExperienceHistory { get; set; }

    public UserExperienceDatabase(IDirectory container, string name, ILogger logger) : base(container, name, logger)
    {
    }

    protected override void ModelCreate(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExperienceOperation>().Navigation<Experience>(x => x.Experience).AutoInclude();
    }

    public async Task AddAsync(ExperienceOperation experienceOperation)
    {
        await ExperienceHistory.AddAsync(experienceOperation);
        await Save();
    }

    public async Task<ExperienceOperation> Last(ExperienceOperation experienceOperation)
    {
        return await ExperienceHistory.LastAsync();
    }

    public async Task<ulong> Count()
    {
        return (ulong) ExperienceHistory.Count();
    }
}