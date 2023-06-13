using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Personal.Common.Enum;
using Personal.Common.Repository;
using Personal.Repository.Mysql.DatabaseRegistrations;
using Personal.Repository.SqlServer.DatabaseRegistrations;

namespace Personal.Repository.Extensions
{
    public static class DatabaseExtension
    {
        public static void AddDatabaseSupport(this IServiceCollection service, IConfiguration configuration)
        {
            var databaseType = default(DatabaseTypeEnum);
            Enum.TryParse<DatabaseTypeEnum>(configuration["UsingDatabaseType"], true, out databaseType);

            var instances = GetInstances();
            foreach (var instance in instances)
            {
                if (instance.SupportedDatabaseType(databaseType))
                {
                    instance.RegisterDatabase(service, configuration);
                }
            }
        }

        public static async Task ApplyDatabaseMigrationsAsync(this DbContext dbContext)
        {
            await dbContext.Database.MigrateAsync();
        }

        private static IEnumerable<IDatabaseRegistration> GetInstances()
        {
            yield return new SqlServerDatabaseRegistration();
            yield return new MySqlDatabaseRegistration();
        }
    }
}