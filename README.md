# Academy.Users

- [English](#english)
- [Español](#espa%c3%b1ol)

## English

### Overview
Academy.Users is a .NET 8 solution structured with Clean Architecture to manage user data for an e-commerce context. It exposes a minimal API for updating user profile information while separating responsibilities across Domain, Application, Infrastructure, and Presentation layers.

### Solution Layout
- `src/Academy.Users.Domain` contains core entities (`User`) that map directly to the SQLite schema.
- `src/Academy.Users.Application` holds use cases and business rules such as `UpdateUserPersonalInformationService`.
- `src/Academy.Users.Infrastructure` wires Entity Framework Core to SQLite and implements repositories.
- `src/Academy.Users.Presentation` defines HTTP contracts and minimal APIs.
- `src/Academy.Users.API` is the composition root that hosts the web app.
- `tests/*` contain xUnit projects (currently scaffolds) for each layer.

### Prerequisites
- .NET SDK 8.0+
- SQLite 3.x
- Access to the database file located at `C:\Users\DSANVICE\OneDrive - Capgemini\Documents\academia_kof_sqlite` (or adjust the connection string).

### Initial Setup
1. Restore dependencies
   ```bash
   dotnet restore
   ```
2. Configure the connection string via user secrets (adjust the path if needed)
   ```bash
   dotnet user-secrets init --project src/Academy.Users.API/Academy.Users.API.csproj
   dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "ConnectionStrings:DefaultConnection" "Data Source=C:\Users\DSANVICE\OneDrive - Capgemini\Documents\academia_kof_sqlite"
   ```
3. Seed sample data for manual testing by executing the SQL below against the SQLite database
   ```sql
   PRAGMA foreign_keys = ON;

   DELETE FROM Users WHERE Email IN ('ana.lopez@example.com', 'carlos.mendez@example.com');

   INSERT INTO Users (
     FirstName,
     LastName,
     Email,
     PhoneNumber,
     Address,
     PasswordHash,
     Status
   )
   VALUES
   (
     'Ana',
     'López',
     'ana.lopez@example.com',
     '5511122233',
     'Av. Reforma 123, Ciudad de México',
     'HASH-ANA-001',
     'ACTIVE'
   ),
   (
     'Carlos',
     'Méndez',
     'carlos.mendez@example.com',
     '5522233344',
     'Calle Hidalgo 456, Ciudad de México',
     'HASH-CARLOS-001',
     'ACTIVE'
   );
   ```

### Build and Run
```bash
# Build
dotnet build

# Run the API (listens on https://localhost:5120 by default)
dotnet run --project src/Academy.Users.API/Academy.Users.API.csproj
```

### API Endpoint
| Method | Route                   | Description                                      |
|--------|------------------------|--------------------------------------------------|
| PUT    | `/api/v1/users/{userId}` | Update personal data for the specified user |

#### Request Body Example
```json
{
  "firstName": "Ana María",
  "lastName": "López Hernández",
  "phoneNumber": "+525511122233",
  "address": "Av. Reforma 500, Piso 12, Ciudad de México"
}
```

#### Possible Responses
- `200 OK`
  ```json
  {
    "userId": 1,
    "firstName": "Ana María",
    "lastName": "López Hernández",
    "phoneNumber": "+525511122233",
    "address": "Av. Reforma 500, Piso 12, Ciudad de México",
    "status": "ACTIVE",
    "message": "User information updated successfully."
  }
  ```
- `400 Bad Request` (validation)
  ```json
  {
    "status": "InvalidData",
    "message": "Invalid user data.",
    "errors": [
      "Phone number is not valid for Mexico."
    ]
  }
  ```
- `400 Bad Request` (user not found)
  ```json
  {
    "status": "UserNotFound",
    "message": "User not found."
  }
  ```
- `500 Internal Server Error`
  ```json
  {
    "status": "ServerError",
    "message": "Could not update user information."
  }
  ```

### Testing the Endpoint
Using Postman or curl:
```bash
curl -X PUT https://localhost:5120/api/v1/users/1 \
     -H "Content-Type: application/json" \
     -d '{
           "firstName": "Ana María",
           "lastName": "López Hernández",
           "phoneNumber": "+525511122233",
           "address": "Av. Reforma 500, Piso 12, Ciudad de México"
         }'
```

### Project Scripts
- `dotnet build` to compile
- `dotnet test` to run every test project (currently placeholders)
- `dotnet watch run --project src/Academy.Users.API/Academy.Users.API.csproj` for hot reload during development

---

## Español

### Descripción General
Academy.Users es una solución .NET 8 basada en la Arquitectura Limpia que permite administrar la información de usuarios para un contexto de comercio electrónico. Expone una API mínima para actualizar los datos personales de un usuario y divide la lógica en capas bien definidas.

### Estructura de la Solución
- `src/Academy.Users.Domain` contiene las entidades centrales (`User`) alineadas con el esquema SQLite.
- `src/Academy.Users.Application` aloja casos de uso y reglas de negocio como `UpdateUserPersonalInformationService`.
- `src/Academy.Users.Infrastructure` integra Entity Framework Core con SQLite y provee repositorios.
- `src/Academy.Users.Presentation` define los contratos HTTP y los endpoints mínimos.
- `src/Academy.Users.API` actúa como punto de composición para hospedar la aplicación web.
- `tests/*` incluye proyectos xUnit (plantillas) para cada capa.

### Requisitos Previos
- .NET SDK 8.0+
- SQLite 3.x
- Acceso al archivo de base de datos `C:\Users\DSANVICE\OneDrive - Capgemini\Documents\academia_kof_sqlite` (ajusta la cadena si cambia).

### Configuración Inicial
1. Restaurar dependencias
   ```bash
   dotnet restore
   ```
2. Configurar la cadena de conexión con secretos de usuario (ajusta la ruta si es necesario)
   ```bash
   dotnet user-secrets init --project src/Academy.Users.API/Academy.Users.API.csproj
   dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "ConnectionStrings:DefaultConnection" "Data Source=C:\Users\DSANVICE\OneDrive - Capgemini\Documents\academia_kof_sqlite"
   ```
3. Ejecutar el siguiente SQL en la base de datos para contar con usuarios de prueba
   ```sql
   PRAGMA foreign_keys = ON;

   DELETE FROM Users WHERE Email IN ('ana.lopez@example.com', 'carlos.mendez@example.com');

   INSERT INTO Users (
     FirstName,
     LastName,
     Email,
     PhoneNumber,
     Address,
     PasswordHash,
     Status
   )
   VALUES
   (
     'Ana',
     'López',
     'ana.lopez@example.com',
     '5511122233',
     'Av. Reforma 123, Ciudad de México',
     'HASH-ANA-001',
     'ACTIVE'
   ),
   (
     'Carlos',
     'Méndez',
     'carlos.mendez@example.com',
     '5522233344',
     'Calle Hidalgo 456, Ciudad de México',
     'HASH-CARLOS-001',
     'ACTIVE'
   );
   ```

### Compilar y Ejecutar
```bash
# Compilar
 dotnet build

# Ejecutar la API (escucha en https://localhost:5120 por defecto)
 dotnet run --project src/Academy.Users.API/Academy.Users.API.csproj
```

### Endpoint Disponibles
| Método | Ruta                      | Descripción                                                  |
|--------|--------------------------|--------------------------------------------------------------|
| PUT    | `/api/v1/users/{userId}` | Actualiza los datos personales del usuario indicado |

#### Ejemplo de Body
```json
{
  "firstName": "Ana María",
  "lastName": "López Hernández",
  "phoneNumber": "+525511122233",
  "address": "Av. Reforma 500, Piso 12, Ciudad de México"
}
```

#### Respuestas Posibles
- `200 OK`
  ```json
  {
    "userId": 1,
    "firstName": "Ana María",
    "lastName": "López Hernández",
    "phoneNumber": "+525511122233",
    "address": "Av. Reforma 500, Piso 12, Ciudad de México",
    "status": "ACTIVE",
    "message": "User information updated successfully."
  }
  ```
- `400 Bad Request` (validación)
  ```json
  {
    "status": "InvalidData",
    "message": "Invalid user data.",
    "errors": [
      "Phone number is not valid for Mexico."
    ]
  }
  ```
- `400 Bad Request` (usuario no encontrado)
  ```json
  {
    "status": "UserNotFound",
    "message": "User not found."
  }
  ```
- `500 Internal Server Error`
  ```json
  {
    "status": "ServerError",
    "message": "Could not update user information."
  }
  ```

### Pruebas con Postman o curl
```bash
curl -X PUT https://localhost:5120/api/v1/users/1 \
     -H "Content-Type: application/json" \
     -d '{
           "firstName": "Ana María",
           "lastName": "López Hernández",
           "phoneNumber": "+525511122233",
           "address": "Av. Reforma 500, Piso 12, Ciudad de México"
         }'
```

### Comandos Útiles
- `dotnet build` para compilar
- `dotnet test` para ejecutar los proyectos de prueba (por ahora contienen plantillas)
- `dotnet watch run --project src/Academy.Users.API/Academy.Users.API.csproj` para recarga en caliente durante el desarrollo
