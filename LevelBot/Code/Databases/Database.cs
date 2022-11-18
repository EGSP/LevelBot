using LevelBot.Code.Files;
using Microsoft.EntityFrameworkCore;

namespace LevelBot.Code.Databases;

public abstract class Database : DbContext
{
    private readonly ILogger _logger;

    public string Name { get;  }
    
    private IDirectory Container { get; }
    
    protected Database(IDirectory container, string name, ILogger logger)
    {
        Container = container;
        Name = name;
        _logger = logger;
        
        Container.Create();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Можно использовать как Filename, так и DataSource или Data Source,
        // т.к. они считатся одинаковыми.
        optionsBuilder.UseSqlite($"Filename={Container.Path}/{Name}.db;Mode=ReadWriteCreate;");
        optionsBuilder.EnableSensitiveDataLogging();
        Configure(optionsBuilder);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelCreate(modelBuilder);
    }

    protected virtual void Configure(DbContextOptionsBuilder optionsBuilder){}
    protected virtual void ModelCreate(ModelBuilder modelBuilder){}

    public async Task Save()
    {
        await SaveChangesAsync();
    }
}