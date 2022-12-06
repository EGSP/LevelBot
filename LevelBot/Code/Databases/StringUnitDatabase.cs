using Automata.IO;
using LevelBot.Code.Databases.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelBot.Code.Databases;

public class StringUnitDatabase : Database
{
    public DbSet<StringUnitProperty> Properties { get; set; }

    public StringUnitDatabase(IDirectory container, string name, ILogger logger) : base(container, name, logger)
    {
    }

    protected override void ModelCreate(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StringUnitProperty>().HasKey(x => x.UnitId);
    }

    public async Task SetAsync(string key, string value)
    {
        var property = Properties.FirstOrDefault(x => x.UnitId == key);

        if (property == null)
        {
            Properties.Add(new StringUnitProperty(key, value));
        }
        else
        {
            property.UnitValue = value;
            Properties.Update(property);
        }
        
        await SaveChangesAsync();
    }

    public async Task<string> GetAsync(string key)
    {
        var property = await Properties.FirstOrDefaultAsync(x => x.UnitId == key);

        return property?.UnitValue ?? string.Empty;
    }
}