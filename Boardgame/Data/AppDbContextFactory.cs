using Boardgame.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Boardgame.Data;

/// <summary>
/// Design-time factory for EF Core CLI tools.
/// Usage: dotnet ef migrations add InitialCreate
///        dotnet ef database update
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionProvider = new DefaultConnectionProvider();
        var connectionString = connectionProvider.GetConnectionString();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
