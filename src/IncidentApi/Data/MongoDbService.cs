using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using IncidentApi.Models;

namespace IncidentApi.Data
{
    public class MongoDbService
    {
        private readonly IMongoCollection<IncidentEvent> _eventsCollection;

        public MongoDbService(IConfiguration configuration)
        {
            // 1. Obtenemos la cadena de conexión. 
            // Si no existe en la config, usamos el nombre del servicio de Docker 'mongodb'
            var connectionString = configuration.GetConnectionString("MongoDb") 
                                   ?? "mongodb://mongodb:27017";

            // 2. Obtenemos el nombre de la base de datos.
            // Si es null en la configuración, usamos "IncidentDB" por defecto.
            var databaseName = configuration["MongoDbSettings:DatabaseName"] ?? "IncidentDB";

            // 3. Obtenemos el nombre de la colección.
            var collectionName = configuration["MongoDbSettings:EventsCollectionName"] ?? "IncidentEvents";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            
            _eventsCollection = database.GetCollection<IncidentEvent>(collectionName);
        }

        // Exponemos la colección para poder insertar datos desde otras partes de la app
        public IMongoCollection<IncidentEvent> Events => _eventsCollection;
    }
}