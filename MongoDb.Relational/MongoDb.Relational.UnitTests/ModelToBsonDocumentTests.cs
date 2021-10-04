using Mongo.Relational.UnitTests.TestModels;
using MongoDb.Relational.Helpers;
using MongoDB.Bson;
using Xunit;

namespace Mongo.Relational.UnitTests
{
    public class ModelToBsonConverterTests
    {
        [Fact]
        public void Convert_ShouldConvertCorrectly()
        {
            var result = ModelToBsonConverter.Convert(new Father
            {
                ID = "test",
                Name = "testName",
                Child = new Child
                {
                    Name = "abc",
                    Age = 14
                }
            });

            Assert.Equal(1, result.Childs.Count);
            Assert.Equal("test", result.ModelId);
            Assert.Equal("testName", result.BsonDocument.GetValue("Name").AsString);
            Assert.Equal("test", result.BsonDocument.GetValue("_id").AsString);
            Assert.NotNull(result.BsonDocument.GetValue("Child").AsBsonDocument.GetValue("ID").AsNullableObjectId);
        }
    }
}
