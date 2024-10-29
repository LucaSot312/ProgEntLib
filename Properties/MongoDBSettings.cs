using MongoDB.Driver;

namespace ProgEntLib.Properties
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public CollectionNames CollectionNames { get; set; }

    }

    public class CollectionNames
    {
        public string Utenti { get; set; }
        public string Libri { get; set; }
        public string Categorie { get; set; }
    }
}
