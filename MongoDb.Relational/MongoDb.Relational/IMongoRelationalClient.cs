using MongoDb.Relational.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace MongoDb.Relational
{
    public interface IMongoRelationalClient
    {
        Wrappers.IMongoClient RawClient { get; }

        Task DeleteOneAsync<T>(FilterDefinition<BsonDocument> filter) where T : MongoModel;
        Task<T> GetOneAsync<T>(FilterDefinition<BsonDocument> filter) where T : MongoModel, new();
        Task SaveOneAsync<T>(T model) where T : MongoModel, new();
    }
}