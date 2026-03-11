# ⚙️ Incident API (.NET 8)
### Variables de Entorno
- `ConnectionStrings__SqlServer`: URI de SQL Server.
- `ConnectionStrings__MongoDb`: URI de MongoDB.

### Cómo Probar Endpoints (curl)
- **Listar:** `curl http://localhost:5000/api/incidents?status=OPEN`
- **Crear:** `curl -X POST http://localhost:5000/api/incidents -H "Content-Type: application/json" -d '{"title":"Error","severity":"HIGH","serviceId":"api-1"}'`

### Pruebas
Ejecutar: `dotnet test`