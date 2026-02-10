using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ucp.Database;

public sealed partial class UpcDbContext
{
    public const int MAX_USERNAME_LENGTH = 50;
    public const int MAX_NICKNAME_LENGTH = 50;
    
    public DbSet<User> Users { get; set; } = null!;
    
    [Table("users")]
    [PrimaryKey(nameof(UserId))]
    [Index(nameof(UserName))]
    public class User
    {
        public Snowflake UserId { get; set; }
        
        [MaxLength(MAX_USERNAME_LENGTH)]
        public string UserName { get; set; } = null!;

        [MaxLength(MAX_NICKNAME_LENGTH)]
        public string UserNickName { get; set; } = null!;
    }
}

