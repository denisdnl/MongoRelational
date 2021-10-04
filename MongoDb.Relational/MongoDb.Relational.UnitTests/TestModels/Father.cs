using MongoDb.Relational.Models;

namespace Mongo.Relational.UnitTests.TestModels
{
    internal class Father: MongoModel
    {
        public string Name { get; set; }

        public Child Child { get; set; }
    }
}
