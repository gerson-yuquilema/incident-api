using IncidentApi.Data;
using IncidentApi.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace IncidentApi.Services
{
    public interface IIncidentService
    {
        Task<Incident> CreateIncidentAsync(CreateIncidentRequest request);
        Task<PagedResponse<Incident>> GetIncidentsAsync(IncidentQueryParameters parameters);
        Task<IncidentDetailResponse?> GetIncidentByIdAsync(Guid id);
        Task<Incident?> UpdateIncidentStatusAsync(Guid id, string newStatus);
    }
    
    public class IncidentService : IIncidentService
    {
        private readonly IncidentDbContext _sqlContext;
        private readonly MongoDbService _mongoService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public IncidentService(IncidentDbContext sqlContext, MongoDbService mongoService, HttpClient httpClient, IConfiguration config)
        {
            _sqlContext = sqlContext;
            _mongoService = mongoService;
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<Incident> CreateIncidentAsync(CreateIncidentRequest request)
        {
            // 1. Guardar el incidente en SQL Server
            var incident = new Incident
            {
                Title = request.Title,
                Description = request.Description,
                Severity = request.Severity,
                Status = "OPEN",
                ServiceId = request.ServiceId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _sqlContext.Incidents.Add(incident);
            await _sqlContext.SaveChangesAsync();

            // 2. Registrar evento inicial en MongoDB
            var createdEvent = new IncidentEvent
            {
                IncidentId = incident.Id,
                EventType = "incident_created",
                Payload = request
            };
            await _mongoService.Events.InsertOneAsync(createdEvent);

            // 3. Llamar al mock Service Catalog con Resiliencia
            // AJUSTE: Si no viene la URL de config, usamos el nombre del contenedor de Docker
            var mockBaseUrl = _config["MockService:BaseUrl"] ?? "http://incident_mock:8080";
            object? mockResponse = null;

            try
            {
                // Establecemos un timeout para que la API no se cuelgue si el Mock falla
                _httpClient.Timeout = TimeSpan.FromSeconds(5);
                
                var response = await _httpClient.GetAsync($"{mockBaseUrl}/services/{request.ServiceId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    mockResponse = JsonSerializer.Deserialize<object>(content);
                }
                else
                {
                    mockResponse = new { Error = $"External Service Error: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                mockResponse = new { Error = $"Service unreachable: {ex.Message}" };
            }

            // 4. Registrar snapshot en MongoDB
            var snapshotEvent = new IncidentEvent
            {
                IncidentId = incident.Id,
                EventType = "service_catalog_snapshot",
                Payload = mockResponse
            };
            await _mongoService.Events.InsertOneAsync(snapshotEvent);

            return incident;
        }

        public async Task<PagedResponse<Incident>> GetIncidentsAsync(IncidentQueryParameters parameters)
        {
            var query = _sqlContext.Incidents.AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.Status))
                query = query.Where(i => i.Status == parameters.Status);
                
            if (!string.IsNullOrWhiteSpace(parameters.Severity))
                query = query.Where(i => i.Severity == parameters.Severity);
                
            if (!string.IsNullOrWhiteSpace(parameters.ServiceId))
                query = query.Where(i => i.ServiceId == parameters.ServiceId);

            if (!string.IsNullOrWhiteSpace(parameters.Q))
                query = query.Where(i => i.Title.Contains(parameters.Q));

            var totalRecords = await query.CountAsync();

            var incidents = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResponse<Incident>
            {
                Data = incidents,
                TotalRecords = totalRecords,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }

        public async Task<IncidentDetailResponse?> GetIncidentByIdAsync(Guid id)
        {
            var incident = await _sqlContext.Incidents.FindAsync(id);
            if (incident == null) return null;

            var filter = Builders<IncidentEvent>.Filter.Eq(e => e.IncidentId, id);
            
            var timeline = await _mongoService.Events
                .Find(filter)
                .SortBy(e => e.Timestamp)
                .ToListAsync();

            return new IncidentDetailResponse
            {
                Incident = incident,
                Timeline = timeline
            };
        }

        public async Task<Incident?> UpdateIncidentStatusAsync(Guid id, string newStatus)
        {
            var incident = await _sqlContext.Incidents.FindAsync(id);
            if (incident == null) return null;

            var oldStatus = incident.Status;
            incident.Status = newStatus;
            incident.UpdatedAt = DateTime.UtcNow;

            await _sqlContext.SaveChangesAsync();

            var statusChangedEvent = new IncidentEvent
            {
                IncidentId = incident.Id,
                EventType = "incident_status_changed",
                Payload = new 
                { 
                    OldStatus = oldStatus, 
                    NewStatus = newStatus,
                    ChangedAt = incident.UpdatedAt
                }
            };
            await _mongoService.Events.InsertOneAsync(statusChangedEvent);

            return incident;
        }
    }
}