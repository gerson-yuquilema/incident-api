namespace IncidentApi.Models
{
    public class IncidentDetailResponse
    {
        // Los datos core que vienen de SQL Server
        public Incident? Incident { get; set; } 
        
        // El historial de auditoría que viene de MongoDB
        public IEnumerable<IncidentEvent> Timeline { get; set; } = new List<IncidentEvent>();
    }
}