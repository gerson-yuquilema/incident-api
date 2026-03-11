namespace IncidentApi.Models
{
    // Molde para recibir los parámetros por la URL (Query String)
    public class IncidentQueryParameters
    {
        public string? Status { get; set; }
        public string? Severity { get; set; }
        public string? ServiceId { get; set; }
        public string? Q { get; set; } // Búsqueda por título
        public int Page { get; set; } = 1;      // Por defecto página 1
        public int PageSize { get; set; } = 10; // Por defecto 10 items por página
    }
}