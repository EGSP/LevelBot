using System.Linq.Expressions;
using LevelBot.Code.Files;
using Microsoft.EntityFrameworkCore;

namespace LevelBot.Code.Databases;


public sealed class SingleContext<TModel> : DbContext
    where TModel : class
{
    public DbSet<TModel> Entities { get; set; }

    private IDirectory Container { get; }
    private string DatabaseName { get; init; }

    public SingleContext(IDirectory container,string databaseName){
        DatabaseName = databaseName;
        Container = container;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Можно использовать как Filename, так и DataSource или Data Source,
        // т.к. они считатся одинаковыми.
        optionsBuilder.UseSqlite($"Filename={Container.Path}/{DatabaseName}.db;Mode=ReadWriteCreate;");
    }
}

/// <summary>
/// Объект для доступа к членам базы данных с кешированием.
/// Он нужен для более удобного взаимодействия с базой данных. 
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class UnitDatabase<TModel> where TModel : class
{
    public SingleContext<TModel> Data { get; set; }

    public UnitDatabase(IDirectory container, string databaseName)
    {
        Data = new SingleContext<TModel>(container, databaseName);
    }
    
    /// <summary>
    /// Добавляет элемент в память и базу данных.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task Add(TModel model)
    {
        if (Data.Entities.Contains(model))
            return;
        
        await Data.Entities.AddAsync(model);
        await Data.SaveChangesAsync();
    }

    /// <summary>
    /// Удаленяет элемент из памяти и базы данных.
    /// </summary>
    public async Task Delete(TModel model)
    {
        if (Data.Entities.Contains(model) == false)
            return;
        
        Data.Entities.Remove(model);
        await Data.SaveChangesAsync();
    }

    public async Task Delete(Expression<Func<TModel,bool>> predicate)
    {
        var coin = await Data.Entities.FirstOrDefaultAsync(predicate);
        if (coin is null)
            return;

        await Delete(coin);
    }

    public async Task<TModel?> FindOrNull(Expression<Func<TModel,bool>> predicate)
    {
        return Data.Entities.FirstOrDefault(predicate);
    }
}