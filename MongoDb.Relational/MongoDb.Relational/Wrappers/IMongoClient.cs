using MongoDB.Driver;

namespace MongoDb.Relational.Wrappers
{
    public interface IMongoClient
    {
        IMongoDatabase GetDatabase(string name, MongoDatabaseSettings settings = null);
    }
}