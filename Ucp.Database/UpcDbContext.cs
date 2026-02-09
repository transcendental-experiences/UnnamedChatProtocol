using Microsoft.EntityFrameworkCore;

namespace Ucp.Database;

public sealed partial class UpcDbContext(DbContextOptions<UpcDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
         // ...
    }
}