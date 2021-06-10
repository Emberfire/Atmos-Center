using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Atmos.Web.Data
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using IServiceScope scope = host.Services.CreateScope();
            using AtmosContext appContext = scope.ServiceProvider.GetRequiredService<AtmosContext>();

            appContext.Database.Migrate();

            return host;
        }
    }
}