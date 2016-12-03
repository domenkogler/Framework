using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Kogler.Standard.EfCore
{
    public static class MigrationOptionsExtensions
    {
        public static DbContextOptionsBuilder UseMigrationOptions(this DbContextOptionsBuilder builder, string migrationsHistoryTableName = null, string migrationsHistoryTableSchema = null)
        {
            var sqlEx = builder.Options.FindExtension<SqlServerOptionsExtension>();
            if (sqlEx == null) return builder;
            sqlEx.MigrationsHistoryTableName = migrationsHistoryTableName;
            sqlEx.MigrationsHistoryTableSchema = migrationsHistoryTableSchema;
            return builder;
        }
    }
}