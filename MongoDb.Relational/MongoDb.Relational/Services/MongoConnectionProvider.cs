namespace MongoDb.Relational.Services
{
    public interface IMongoConnectionProvider
    {
        string Url { get; set; }
        public string Database { get; set; }
    }

    public class MongoConnectionProvider : IMongoConnectionProvider
    {
        public string Url { get; set; }
        public string Database { get; set; }
    }
}
