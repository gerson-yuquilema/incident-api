# ⚙️ Incident Management API (.NET 8)

API robusta para la gestión de tickets de incidentes, diseñada con un enfoque de persistencia políglota y alta disponibilidad.

## 🛠️ Stack Tecnológico
- **Framework:** .NET 8 (ASP.NET Core).
- **Persistencia Transaccional:** SQL Server (Entity Framework Core).
- **Persistencia de Eventos:** MongoDB (Auditoría inmutable).
- **Integraciones:** Consumo de Catálogo de Servicios externos mediante HttpClient y Wiremock.

## 🏗️ Decisiones de Arquitectura
- **Estrategia Políglota:** SQL Server garantiza la integridad referencial para los tickets, mientras que MongoDB actúa como un almacén de eventos (*Event Store*) para auditoría, permitiendo un historial flexible y de alto rendimiento.
- **Resiliencia:** Implementación de políticas de reintento (`EnableRetryOnFailure`) para la conexión a SQL Server, asegurando la estabilidad en entornos de contenedores.
- **Salud del Sistema:** Endpoint de `/health` implementado para monitoreo de infraestructura (Bonus 11).

## 🚀 Cómo Correr (Pasos Exactos)
1. Asegurar que el SDK de .NET 8 esté instalado.
2. Restaurar dependencias: `dotnet restore`
3. Ejecutar: `dotnet run --project src/IncidentApi/IncidentApi.csproj`
4. Documentación interactiva (Swagger): `http://localhost:5000/`

## 🧪 Pruebas y Calidad
- **Unit Tests:** Ejecutar `dotnet test` para validar la lógica de negocio y modelos.
- **CI/CD:** Integración continua mediante **GitHub Actions** que valida compilación y tests en cada Pull Request.

## 📡 Cómo Probar Endpoints (curl)

### Crear Incidente (POST)
```bash
curl -X POST http://localhost:5000/api/incidents \
-H "Content-Type: application/json" \
-d '{"title":"Falla de Red","description":"Error 500 en Login","severity":"HIGH","serviceId":"auth-api"}'


Listar con Filtros y Paginación 

curl "http://localhost:5000/api/incidents?status=OPEN&severity=HIGH&page=1&pageSize=10"

Ver Detalle y Timeline NoSQL (GET)

curl http://localhost:5000/api/incidents/{id}