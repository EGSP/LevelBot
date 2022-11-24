using LevelBot.Code.Databases;
using LevelBot.Code.Databases.Contexts;
using LevelBot.Code.Files;
using LevelBot.Code.Models;

namespace LevelBot.Code.Drivers;

public class UserDriver
{
    public readonly GuildUser User;

    public readonly IDirectory Root;
    
    public UserDriver(GuildUser user)
    {
        User = user;

        Root = GuildDriver.GetUserDirectory(this);
    }
}

public static class UserDriverExtensions
{
    public static async Task NewExperienceAsync(this UserDriver driver, ExperienceOperation operation, ILogger logger)
    {
        var userExpHistory = new UserExperienceDatabase(driver.Root, "experience-history", logger);
        await userExpHistory.Database.EnsureCreatedAsync();

        await userExpHistory.AddAsync(operation);
    }

    public static async Task CalculateExperienceAsync(this UserDriver driver, ILogger logger)
    {
        var properties = new StringUnitDatabase(driver.Root, "properties", logger);
        await properties.Database.EnsureCreatedAsync();
        
        var userExpHistory = new UserExperienceDatabase(driver.Root, "experience-history", logger);
        await userExpHistory.Database.EnsureCreatedAsync();   
        
        ulong sum = 0;
        
        await foreach (var experienceOperation in userExpHistory.ExperienceHistory)
        {
            switch (experienceOperation.Operation)
            {
                case OperationType.Add:
                    sum += experienceOperation.Experience.Value;
                    break;
                case OperationType.Remove:
                    sum -= experienceOperation.Experience.Value;
                    break;
                case OperationType.Set:
                    sum = experienceOperation.Experience.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        await properties.SetAsync("sum", sum.ToString());
    }

    public static async Task NewCalcExperienceAsync(this UserDriver driver, ExperienceOperation operation, ILogger logger)
    {
        await NewExperienceAsync(driver, operation, logger);
        await CalculateExperienceAsync(driver, logger);
    }

    public static async Task<Experience> GetExperienceAsync(this UserDriver driver, ILogger logger)
    {
        var properties = new StringUnitDatabase(driver.Root, "properties", logger);
        await properties.Database.EnsureCreatedAsync();

        var sumProperty = await properties.GetAsync("sum");
        ulong sum = 0;
        if (!string.IsNullOrEmpty(sumProperty))
            sum = ulong.Parse(sumProperty);

        return new Experience(sum);
    } 
}

