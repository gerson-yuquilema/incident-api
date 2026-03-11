namespace IncidentApi.Models
{
    // Una clase genérica estándar para devolver resultados paginados
    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        
        // Calcula automáticamente cuántas páginas hay en total
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize); 
    }
}