using Automata.IO;
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