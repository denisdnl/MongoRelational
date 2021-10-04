using MongoDb.Relational.Helpers;
using MongoDb.Relational.Models;
using MongoDb.Relational.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace MongoDb.Relational
{
    public class MongoRelationalClient : IMongoRelationalClient
    {
        private readonly IMongoConnectionProvider mongoConnectionProvider;

        public Wrappers.IMongoClient RawClient { get; }


        public MongoRelationalClient(Wrappers.IMongoClient client, IMongoConnectionProvider mongoConnectionProvider)
        {
            RawClient = client;
            this.mongoConnectionProvider = mongoConnectionProvider;
        }

        public async Task SaveOneAsync<T>(T model) where T : MongoModel, new()
        {
            var convertionResult = ModelToBsonConverter.Convert(model);

            foreach (var child in convertionResult.Childs) // Save childs
            {
                await SaveOneAsync(child);
            }

            FilterDefinition<BsonDocument> filter = $"{{ _id : \"{ convertionResult.ModelId }\" }}";

            await RawClient.GetDatabase(mongoConnectionProvider.Database)
                    .GetCollection<BsonDocument>(model.GetType().Name)
                    .ReplaceOneAsync(filter, convertionResult.BsonDocument, new ReplaceOptions { IsUpsert = true });
        }

        public async Task<T> GetOneAsync<T>(FilterDefinition<BsonDocument> filter) where T : MongoModel, new()
        {
            var bson = (await (await RawClient.GetDatabase(mongoConnectionProvider.Database)
                .GetCollection<BsonDocument>(typeof(T).Name)
                .FindAsync(filter)).SingleOrDefaultAsync());

            return bson is null ? null :
                await BsonToModelConverter.Convert<T>(bson, typeof(MongoRelationalClient).GetMethod("GetOneAsync"), this);
        }

        public async Task DeleteOneAsync<T>(FilterDefinition<BsonDocument> filter) where T : MongoModel
        {
            await RawClient.GetDatabase(mongoConnectionProvider.Database)
                .GetCollection<BsonDocument>(typeof(T).Name)
                .DeleteOneAsync(filter);

            // @ToDo : Delete all references to this document 
        }

    }
}
