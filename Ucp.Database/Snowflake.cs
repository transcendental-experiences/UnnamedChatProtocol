using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ucp.Database;

public readonly record struct Snowflake(long Inner);

public sealed class SnowflakeConverter : ValueConverter<Snowflake, long>
{
    public SnowflakeConverter()
        : base(
            v => v.Inner,
            v => new Snowflake(v))
    {
    }
}