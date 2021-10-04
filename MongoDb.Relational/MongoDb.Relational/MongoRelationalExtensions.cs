using Microsoft.Extensions.DependencyInjection;
using MongoDb.Relational.Services;
using MongoDb.Relational.Wrappers;

namespace MongoDb.Relational
{
    public static class MongoRelationalExtensions
    {
        public static IServiceCollection AddMongoRelational(this IServiceCollection services, MongoConnectionProvider mongoConnectionProvider)
        {
            services.AddTransient<IMongoConnectionProvider>(_ => mongoConnectionProvider);
            services.AddTransient<IMongoClient, MongoDriverClientWrapper>();
            services.AddTransient<IMongoRelationalClient, MongoRelationalClient>();
            return services;
        }
    }
}
