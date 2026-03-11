using IncidentApi.Controllers;
using IncidentApi.Models;
using IncidentApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IncidentApi.Tests
{
    public class IncidentsControllerTests
    {
        private readonly Mock<IIncidentService> _mockService;
        private readonly IncidentsController _controller;

        public IncidentsControllerTests()
        {
            // 1. Arrange: Preparamos el entorno simulado (Mock)
            _mockService = new Mock<IIncidentService>();
            _controller = new IncidentsController(_mockService.Object);
        }

        [Fact]
        public async Task CreateIncident_WithValidData_ReturnsCreatedResult()
        {
            // Arrange: Preparamos los datos falsos
            var request = new CreateIncidentRequest
            {
                Title = "Error en base de datos",
                Description = "Timeout al conectar",
                Severity = "HIGH",
                ServiceId = "db-service"
            };

            var expectedIncident = new Incident
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Severity = request.Severity,
                ServiceId = request.ServiceId,
                Status = "OPEN"
            };

            // Le decimos al mock: "Cuando alguien llame a CreateIncidentAsync, devuelve este expectedIncident"
            _mockService.Setup(s => s.CreateIncidentAsync(request))
                        .ReturnsAsync(expectedIncident);

            // 2. Act: Ejecutamos el método del controlador
            var result = await _controller.CreateIncident(request);

            // 3. Assert: Verificamos que el resultado sea el esperado
            var createdResult = Assert.IsType<CreatedAtActionResult>(result); // Verificamos que sea un HTTP 201
            var returnedIncident = Assert.IsType<Incident>(createdResult.Value); // Verificamos que devuelva un Incidente
            
            Assert.Equal("OPEN", returnedIncident.Status);
            Assert.Equal(expectedIncident.Id, returnedIncident.Id);
        }
    }
}