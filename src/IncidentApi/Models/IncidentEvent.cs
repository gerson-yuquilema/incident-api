using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IncidentApi.Models
{
    public class IncidentEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Le decimos a Mongo que guarde el Guid como un String
        [BsonRepresentation(BsonType.String)]
        public Guid IncidentId { get; set; }
        
        // Tipo de evento: "incident_created", "incident_status_changed", "service_catalog_snapshot"
        public string EventType { get; set; } = string.Empty; 
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Aquí guardaremos cualquier dato adicional en formato libre (como la respuesta del mock)
        public object? Payload { get; set; } 
    }
}