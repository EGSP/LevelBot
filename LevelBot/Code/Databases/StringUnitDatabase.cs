using LevelBot.Code.Databases.Models;
using LevelBot.Code.Files;
using Microsoft.EntityFrameworkCore;

namespace LevelBot.Code.Databases;

public class StringUnitDatabase : UnitDatabase<StringUnitProperty>
{
    public StringUnitDatabase(IDirectory container, string databaseName) : base(container, databaseName)
    {
    }

    public async Task Set(string key, string value)
    {
        var property = Data.Entities.FirstOrDefault(x => x.UnitId == key);

        if (property == null)
        {
            Data.Entities.Add(new StringUnitProperty(key, value));
        }
        else
        {
            property.UnitValue = value;
            Data.Entities.Update(property);
            await Data.SaveChangesAsync();
        }
    }

    public async Task<string> Get(string key)
    {
        var property = await Data.Entities.FirstOrDefaultAsync(x => x.UnitId == key);

        return property?.UnitValue ?? string.Empty;
    }  
}