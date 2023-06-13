using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Personal.Common.Enum;

namespace Personal.Common.Repository
{
    public interface IDatabaseRegistration
    {
        bool SupportedDatabaseType(DatabaseTypeEnum databaseType);

        void RegisterDatabase(IServiceCollection services, IConfiguration configuration);
    }
}