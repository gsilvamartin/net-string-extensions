using System;
using Microsoft.Extensions.DependencyInjection;
using NetPlus.ServiceAbstractions.Database.NoSQL.MongoDB.Connection;
using NetPlus.ServiceAbstractions.Database.NoSQL.MongoDB.Entity;
using NetPlus.ServiceAbstractions.Database.NoSQL.MongoDB.Interfaces;

namespace NetPlus.ServiceAbstractions.Database.NoSQL.MongoDB.Extensions
{
    /// <summary>
    /// A static class providing extension methods for configuring MongoDB.
    /// </summary>
    public static class MongoRepositoryExtensions
    {
        /// <summary>
        /// Configures the MongoDB connection using the specified configuration.
        ///
        /// This method receives a configuration object and registers it as a singleton in the service collection.
        /// This configuration object is used to connect to the MongoDB database.
        ///
        /// If you don't want to use this configuration method, you can directly create a instance of the
        /// <see cref="MongoDbConfiguration"/> class and register it as a singleton in the service collection.
        ///
        /// Reminder: If you don't call this method, you'll need to specify the configuration when you create a
        /// <see cref="MongoRepository{T}"/> instance.
        /// 
        /// </summary>
        /// <param name="service">Service Collection</param>
        /// <param name="configure">MongoDb Configuration</param>
        public static void ConfigureMongoDb(this IServiceCollection service, Action<MongoDbConfiguration> configure)
        {
            var config = new MongoDbConfiguration();
            configure(config);

            if (string.IsNullOrEmpty(config.DatabaseName))
                throw new ArgumentNullException(nameof(config.DatabaseName), "The database name must be specified.");

            if (string.IsNullOrEmpty(config.ConnectionString))
                throw new ArgumentNullException(nameof(config.ConnectionString),
                    "The connection string must be specified.");

            service.AddSingleton(config);
        }

        public static void AddMongoRepository<T>(
            this IServiceCollection service,
            Action<MongoDbConfiguration>? configure = null) where T : BaseEntity
        {
            var config = new MongoDbConfiguration();
            configure?.Invoke(config);

            service.AddScoped<IMongoRepository<T>, MongoRepository<T>>(options =>
                string.IsNullOrEmpty(config.ConnectionString) || string.IsNullOrEmpty(config.DatabaseName)
                    ? new MongoRepository<T>()
                    : new MongoRepository<T>(config));
        }
    }
}