using System;

namespace IncidentApi.Models
{
    public class Incident
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // HIGH, MEDIUM, LOW
        public string Status { get; set; } = string.Empty;   // OPEN, IN_PROGRESS, RESOLVED
        public string ServiceId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}