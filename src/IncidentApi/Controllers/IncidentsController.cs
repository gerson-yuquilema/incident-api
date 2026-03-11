using IncidentApi.Models;
using IncidentApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IncidentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Esto hace que la URL sea: http://localhost:puerto/api/incidents
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentService _incidentService;

        // Inyectamos el servicio que creamos en el paso anterior
        public IncidentsController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        // Endpoint: POST /api/incidents
        [HttpPost]
        public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentRequest request)
        {
            // Validación básica (DevSecOps: nunca confíes en los datos del usuario)
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Severity) || string.IsNullOrWhiteSpace(request.ServiceId))
            {
                return BadRequest("Title, Severity, and ServiceId are required.");
            }

            try
            {
                // Llamamos a nuestro servicio para que haga toda la orquestación (SQL, Mongo, Mock)
                var result = await _incidentService.CreateIncidentAsync(request);
                
                // Retornamos un HTTP 201 (Created) que es la buena práctica al crear un recurso
                return CreatedAtAction(nameof(GetIncidentById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                // Si algo catastrófico falla (ej. base de datos caída), devolvemos 500
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint: GET /api/incidents
        [HttpGet]
        public async Task<IActionResult> GetIncidents([FromQuery] IncidentQueryParameters parameters)
        {
            try
            {
                var result = await _incidentService.GetIncidentsAsync(parameters);
                return Ok(result); // Devuelve HTTP 200 con nuestro PagedResponse
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint: GET /api/incidents/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncidentById(Guid id)
        {
            try
            {
                var result = await _incidentService.GetIncidentByIdAsync(id);
                
                // Si el servicio nos devuelve nulo, es porque el ID no existe en la base de datos
                if (result == null)
                {
                    return NotFound(new { Message = $"Incident with ID {id} not found." }); // Código HTTP 404
                }

                return Ok(result); // Código HTTP 200 con nuestro IncidentDetailResponse
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint: PATCH /api/incidents/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateIncidentStatus(Guid id, [FromBody] UpdateIncidentStatusRequest request)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest("The 'status' field is required.");
            }

            try
            {
                var result = await _incidentService.UpdateIncidentStatusAsync(id, request.Status);

                if (result == null)
                {
                    return NotFound($"Incident with ID {id} not found.");
                }

                return Ok(result); // Devolvemos 200 OK con el incidente actualizado
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}