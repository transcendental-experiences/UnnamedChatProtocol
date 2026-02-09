using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ucp.Database;

public sealed partial class UpcDbContext
{
    public DbSet<User> Users { get; set; }
    
    [Table("users")]
    [PrimaryKey(nameof(UserId))]
    public sealed class User
    {
        public long UserId { get; set; }
    }
}