using MongoDb.Relational.Models;

namespace Mongo.Relational.UnitTests.TestModels
{
    internal class Child: MongoModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
