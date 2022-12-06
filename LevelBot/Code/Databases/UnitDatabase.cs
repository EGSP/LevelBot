using System.Linq.Expressions;
using Automata.IO;
using Microsoft.EntityFrameworkCore;

namespace LevelBot.Code.Databases;

/// <summary>
/// Объект для доступа к членам базы данных с кешированием.
/// Он нужен для более удобного взаимодействия с базой данных. 
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class UnitDatabase<TModel> where TModel : class
{
    public SingleContext<TModel> Data { get; }

    public UnitDatabase(IDirectory container, string databaseName)
    {
        Data = new SingleContext<TModel>(container, databaseName);
    }
}

public static class UnitDatabaseExtensions
{
    /// <summary>
    /// Добавляет элемент в память и базу данных.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static async Task Add<TModel>(this UnitDatabase<TModel> db, TModel model)
        where TModel : class
    {
        if (db.Data.Entities.Contains(model))
            return;
        
        await db.Data.Entities.AddAsync(model);
        await db.Data.SaveChangesAsync();
    }

    /// <summary>
    /// Удаленяет элемент из памяти и базы данных.
    /// </summary>
    public static async Task Delete<TModel>(this UnitDatabase<TModel> db,TModel model)
        where TModel : class
    {
        if (db.Data.Entities.Contains(model) == false)
            return;
        
        db.Data.Entities.Remove(model);
        await db.Data.SaveChangesAsync();
    }

    public static async Task Delete<TModel>(this UnitDatabase<TModel> db,Expression<Func<TModel,bool>> predicate)
        where TModel : class
    {
        var coin = await db.Data.Entities.FirstOrDefaultAsync(predicate);
        if (coin is null)
            return;

        await Delete(db, coin);
    }

    public static async Task<TModel?> FindOrNull<TModel>(this UnitDatabase<TModel> db,Expression<Func<TModel,bool>> predicate)
        where TModel : class
    {
        return db.Data.Entities.FirstOrDefault(predicate);
    }
}