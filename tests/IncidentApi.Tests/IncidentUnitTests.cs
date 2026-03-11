using Xunit;
using IncidentApi.Models;

namespace IncidentApi.Tests
{
    public class IncidentUnitTests
    {
        [Fact]
        public void CreateIncidentRequest_ShouldCorrectStoreData()
        {
            // 1. Arrange: Preparamos los datos de prueba
            var testTitle = "Error crítico de base de datos";
            var testSeverity = "HIGH";

            // 2. Act: Instanciamos el modelo que usas en el POST
            var request = new CreateIncidentRequest 
            { 
                Title = testTitle, 
                Severity = testSeverity, 
                ServiceId = "payments-api" 
            };

            // 3. Assert: Verificamos que los datos se guarden correctamente
            Assert.Equal(testTitle, request.Title);
            Assert.Equal(testSeverity, request.Severity);
            Assert.NotNull(request.ServiceId);
        }
    }
}