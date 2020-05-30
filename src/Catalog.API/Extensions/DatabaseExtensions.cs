using Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddCatalogContext(this IServiceCollection services)
        {
            return services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<CatalogContext>(contextOptions =>
                {
                    contextOptions.UseSqlServer(
                        "Server=localhost,1433;Initial Catalog=Store;User Id=catalog_srv;Password=P@ssw0rd",
                        serverOptions => {
                            serverOptions.MigrationsAssembly
                            (typeof(Startup).Assembly.FullName);
                        }
                    );
                });
        }
    }
}