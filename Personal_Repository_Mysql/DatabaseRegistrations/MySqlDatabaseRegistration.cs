using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Personal.Common.Enum;
using Personal.Common.Repository;
using Personal.Repository.Share;

namespace Personal.Repository.Mysql.DatabaseRegistrations
{
    public class MySqlDatabaseRegistration : IDatabaseRegistration
    {
        public bool SupportedDatabaseType(DatabaseTypeEnum databaseType)
        {
            return databaseType == DatabaseTypeEnum.MYSQL;
        }

        public void RegisterDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = $"server={configuration["MySql:Server"]};database={configuration["DatabaseName"]};user={configuration["MySql:UserId"]};password={configuration["MySql:Password"]}";
            services.AddDbContext<AppDbContext>(builder => builder.UseMySQL(connectionString, option => option.MigrationsAssembly(typeof(MySqlDatabaseRegistration).Assembly.GetName().Name)));
        }
    }
}