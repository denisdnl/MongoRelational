using FakeItEasy;
using Mongo.Relational.UnitTests.TestModels;
using MongoDb.Relational;
using MongoDb.Relational.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Mongo.Relational.UnitTests
{
    public class MongoRelationalClientTests
    {
        [Fact]
        public async Task SaveOneAsync_ShouldCallMongoDriverInsert()
        {
            var collection = A.Fake<IMongoCollection<BsonDocument>>();
            var db = A.Fake<IMongoDatabase>();
            var mongoClient = A.Fake<MongoDb.Relational.Wrappers.IMongoClient>();
            var mongoConnectionProvider = A.Fake<IMongoConnectionProvider>();
            A.CallTo(() => mongoClient.GetDatabase(A<string>._, A<MongoDatabaseSettings>._)).Returns(db);
            A.CallTo(() => db.GetCollection<BsonDocument>(A<string>._, default)).Returns(collection);

            var client = new MongoRelationalClient(mongoClient, mongoConnectionProvider);

            await client.SaveOneAsync(new Father
            {
                Name = "testFather",
                Child = new Child
                {
                    Name = "testChild",
                    Age = 2
                }
            });

            A.CallTo(() => collection.ReplaceOneAsync<BsonDocument>(A<Expression<Func<BsonDocument, bool>>>._,
                A<BsonDocument>._,
                A<ReplaceOptions>.That.Matches(ro => ro.IsUpsert == true), default))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SaveOneAsync_ShouldNotModifyIdWhenIdIsSet()
        {
            var collection = A.Fake<IMongoCollection<BsonDocument>>();
            var db = A.Fake<IMongoDatabase>();
            var mongoClient = A.Fake<MongoDb.Relational.Wrappers.IMongoClient>();
            var mongoConnectionProvider = A.Fake<IMongoConnectionProvider>();
            A.CallTo(() => mongoClient.GetDatabase(A<string>._, A<MongoDatabaseSettings>._)).Returns(db);
            A.CallTo(() => db.GetCollection<BsonDocument>(A<string>._, default)).Returns(collection);

            var client = new MongoRelationalClient(mongoClient, mongoConnectionProvider);

            await client.SaveOneAsync(new Father
            {
                ID = "1232444",
                Name = "testFather",
                Child = new Child
                {
                    Name = "testChild",
                    Age = 2
                }
            });

            A.CallTo(() => collection.ReplaceOneAsync<BsonDocument>(A<Expression<Func<BsonDocument, bool>>>._,
                A<BsonDocument>.That.Matches(doc => doc.GetValue("_id") == "1232444"),
                A<ReplaceOptions>.That.Matches(ro => ro.IsUpsert == true), default))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DeleteOneAsync_ShouldCallMongoDriverDelete()
        {
            var collection = A.Fake<IMongoCollection<BsonDocument>>();
            var db = A.Fake<IMongoDatabase>();
            var mongoClient = A.Fake<MongoDb.Relational.Wrappers.IMongoClient>();
            var mongoConnectionProvider = A.Fake<IMongoConnectionProvider>();
            A.CallTo(() => mongoClient.GetDatabase(A<string>._, A<MongoDatabaseSettings>._)).Returns(db);
            A.CallTo(() => db.GetCollection<BsonDocument>(A<string>._, default)).Returns(collection);

            var client = new MongoRelationalClient(mongoClient, mongoConnectionProvider);

            await client.DeleteOneAsync<Father>($"{{ _id = \"test\"}}");

            A.CallTo(() => collection.DeleteOneAsync(A<FilterDefinition<BsonDocument>>.That.Matches(f => 
                                                    f.ToBsonDocument(default, default, default).GetValue("_id").ToString() == "test"), default))
                .MustHaveHappenedOnceExactly();
        }
    }
}
