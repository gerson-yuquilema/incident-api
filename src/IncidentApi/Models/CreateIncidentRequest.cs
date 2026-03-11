namespace IncidentApi.Models
{
    // Esta clase representa exactamente el JSON que el frontend o Postman nos va a enviar
    public class CreateIncidentRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // HIGH, MEDIUM, LOW
        public string ServiceId { get; set; } = string.Empty;
    }
}