using MongoDb.Relational.Services;
using MongoDB.Driver;

namespace MongoDb.Relational.Wrappers
{
    public class MongoDriverClientWrapper : IMongoClient
    {
        private readonly MongoDB.Driver.MongoClient client;

        public MongoDriverClientWrapper(IMongoConnectionProvider connectionProvider)
        {
            client = new MongoDB.Driver.MongoClient(connectionProvider.Url);
        }

        public IMongoDatabase GetDatabase(string name, MongoDatabaseSettings settings = null)
        {
            return client.GetDatabase(name, settings);
        }
    }
}
