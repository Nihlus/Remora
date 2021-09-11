Remora.EntityFrameworkCore.Modular
==============

Remora.EntityFrameworkCore.Modular provides a modular approach to EF Core databases, fitting perfectly together with the
concept of a plugin system or isolated portions of the database. Individual modules have their own migration paths,
their own schemas, and their own completely isolated contexts.

Entities can be shared between the contexts and referenced from one to another in a completely transparent way, and
dependencies are handled in a typical C# fashion - no extra considerations needed.

A more detailed explanation of the mechanisms involved and the techniques behind it can be read [here][2].

## Usage
```c#
public class MyDatabaseContext : SchemaAwareDbContext
{
    private const string SchemaName = "MySchema";

    public DbSet<MyEntity> Entities { get; set; }

    public MyDatabaseContext(DbContextOptions<MyDatabaseContext> contextOptions)
        : base(SchemaName, contextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
```

```c#
var services = new ServiceCollection()
    .AddSingleton<SchemaAwareDbContextService>()
    .AddSchemaAwareDbContextPool<MyDatabaseContext>((services, options) => ConfigureMyDatabaseContext(options))
    .BuildServiceProvider();
```

## Building
The library does not require anything out of the ordinary to compile.

```bash
cd $SOLUTION_DIR
dotnet build
dotnet pack -c Release
```

## Downloading
Get it on [NuGet][1].


[1]: https://www.nuget.org/packages/Remora.EntityFrameworkCore.Modular/
[2]: https://algiz.nu/blog/modularizing-your-database-with-ef-core/
