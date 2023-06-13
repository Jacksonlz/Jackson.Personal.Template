using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Personal.Common.Enum;
using Personal.Common.Repository;
using Personal.Repository.Share;
using Microsoft.EntityFrameworkCore;

namespace Personal.Repository.SqlServer.DatabaseRegistrations
{
    public class SqlServerDatabaseRegistration : IDatabaseRegistration
    {
        public bool SupportedDatabaseType(DatabaseTypeEnum databaseType)
        {
            return databaseType == DatabaseTypeEnum.SQLSERVER;
        }

        public void RegisterDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = $"Server={configuration["SqlServer:Server"]};Database={configuration["DatabaseName"]};MultipleActiveResultSets=true;User Id={configuration["SqlServer:UserId"]};Password={configuration["SqlServer:Password"]};Trust Server Certificate=true";
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString, context => context.MigrationsAssembly(typeof(SqlServerDatabaseRegistration).Assembly.GetName().Name)));
        }
    }
}