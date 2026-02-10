using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ucp.Database;

public sealed class DesignTimeContextFactoryPostgres : IDesignTimeDbContextFactory<UpcDbContext>
{
    public UpcDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UpcDbContext>();
        optionsBuilder.UseNpgsql("Server=localhost");
        return new UpcDbContext(optionsBuilder.Options);
    }
}