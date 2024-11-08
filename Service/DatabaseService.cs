using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace ProgEntLib.Service
{
    public class DatabaseService
    {
        private readonly IMongoDatabase _database;
        private readonly string _backupFolderPath;

        public DatabaseService(IConfiguration configuration, IMongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase(configuration["MongoDB:DatabaseName"]);
            _backupFolderPath = configuration["MongoDB:BackupSettings:BackupFolderPath"];
        }

        // Backup del database come file JSON
        public async Task BackupDatabaseAsync()
        {
            var collections = await _database.ListCollectionNamesAsync();
            foreach (var collectionName in collections.ToList())
            {
                var collection = _database.GetCollection<BsonDocument>(collectionName);
                var documents = await collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync();
                
                // Crea una cartella di backup se non esiste
                var backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), _backupFolderPath);
                if (!Directory.Exists(backupFolderPath))
                {
                    Directory.CreateDirectory(backupFolderPath);
                }

                // Salva i documenti come file JSON
                var backupFilePath = Path.Combine(backupFolderPath, $"{collectionName}_backup.json");
                var json = documents.ToJson();
                await File.WriteAllTextAsync(backupFilePath, json);
            }
        }

        // Restore del database dai file JSON
        public async Task RestoreDatabaseAsync(string backupFileName)
        {
            // Percorso del backup
            var backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), _backupFolderPath);
            var backupFilePath = Path.Combine(backupFolderPath, backupFileName);

            if (!File.Exists(backupFilePath))
            {
                throw new FileNotFoundException("File di backup non trovato.");
            }

            var json = await File.ReadAllTextAsync(backupFilePath);
            var documents = BsonSerializer.Deserialize<List<BsonDocument>>(json);

            // Salva i documenti nelle collezioni del database
            var collectionName = Path.GetFileNameWithoutExtension(backupFileName).Replace("_backup", "");
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            await collection.InsertManyAsync(documents);
        }
    }
}
