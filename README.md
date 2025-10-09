# Academy.Users

- [Español](#español)
  - [Descripción general](#descripción-general)
  - [Diseño de la solución](#diseño-de-la-solución)
  - [Requisitos previos](#requisitos-previos)
  - [Configuración inicial](#configuración-inicial)
  - [Compilar y ejecutar](#compilar-y-ejecutar)
  - [Endpoint expuesto](#endpoint-expuesto)
  - [Proveedores de base de datos](#proveedores-de-base-de-datos)
- [English](#english)
  - [Overview](#overview)
  - [Solution layout](#solution-layout)
  - [Prerequisites](#prerequisites)
  - [Initial setup](#initial-setup)
  - [Build and run](#build-and-run)
  - [API endpoint](#api-endpoint)
  - [Database providers](#database-providers)

## Español

### Descripción general
Academy.Users es una solución .NET 8 basada en Clean Architecture para administrar datos personales de usuarios dentro de un escenario de comercio electrónico. El proyecto está dividido en capas (`Domain`, `Application`, `Infrastructure`, `Presentation`, `API`) y cuenta con proyectos de pruebas por capa en `tests/*`.

### Diseño de la solución
- `src/Academy.Users.Domain`: entidades núcleo (`User`).
- `src/Academy.Users.Application`: casos de uso/CQRS (por ejemplo `UpdateUserCommandHandler`).
- `src/Academy.Users.Infrastructure`: configuración de EF Core y repositorios (SQLite/SQL Server).
- `src/Academy.Users.Presentation`: endpoints/DTOs minimal API.
- `src/Academy.Users.API`: host y composición general.
- `tests/*`: proyectos xUnit (unitarios por capa).

### Requisitos previos
- .NET SDK 8.0+
- Acceso a SQL Server (principal) o a un archivo SQLite si usas la opción alternativa.

### Configuración inicial
1. Restaurar dependencias
   ```bash
   dotnet restore
   ```
2. Configurar el proveedor y cadena de conexión mediante user-secrets. La configuración principal apunta a SQL Server hospedado en `sql.bsite.net`:
   ```bash
   dotnet user-secrets init --project src/Academy.Users.API/Academy.Users.API.csproj
   dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "DatabaseOptions:Provider" "sqlserver"
   dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "ConnectionStrings:DefaultConnection" "Server=sql.bsite.net\MSSQL2016;Database=academynet_AcademyDB;User Id=academynet_AcademyDB;TrustServerCertificate=True"
   ```
   - Después de `User Id=academynet_AcademyDB;` agrega manualmente la propriedad `Password=`, con la contraseña del servidor, antes de ejecutar el comando.
   
3. Preparar la base de datos ejecutando el siguiente script SQL Server:
   ```sql
   DELETE FROM dbo.Users WHERE Email IN ('ana.lopez@example.com', 'carlos.mendez@example.com');
   GO

   INSERT INTO dbo.Users
   (
       FirstName,
       LastName,
       Email,
       PhoneNumber,
       Address,
       Password,
       Status,
       CreatedAt,
       UpdatedAt
   )
   VALUES
   (
       'Ana',
       'López',
       'ana.lopez@example.com',
       '5511122233',
       'Av. Reforma 123, Ciudad de México',
       'HASH-ANA-001',
       'ACTIVE',
       SYSUTCDATETIME(),
       SYSUTCDATETIME()
   ),
   (
       'Carlos',
       'Méndez',
       'carlos.mendez@example.com',
       '5522233344',
       'Calle Hidalgo 456, Ciudad de México',
       'HASH-CARLOS-001',
       'ACTIVE',
       SYSUTCDATETIME(),
       SYSUTCDATETIME()
   );
   GO
   ```

### Compilar y ejecutar
```bash
dotnet build
dotnet run --project src/Academy.Users.API/Academy.Users.API.csproj
```

### Endpoint expuesto
| Método | Ruta                    | Descripción                                              |
|--------|-------------------------|----------------------------------------------------------|
| PUT    | `/api/v1/users/{userId}` | Actualiza los datos personales del usuario indicado |

#### Ejemplo de petición
```json
{
  "firstName": "Ana María",
  "lastName": "López Hernández",
  "phoneNumber": "+525511122233",
  "address": "Av. Reforma 500, Piso 12, Ciudad de México"
}
```

### Proveedores de base de datos
La aplicación lee dos claves de configuración:
1. `DatabaseOptions:Provider` – `sqlserver` (por defecto), `sqlite` o `sqlserverexpress`.
2. `ConnectionStrings:DefaultConnection` – cadena correspondiente al proveedor.

#### Principal: SQL Server hospedado en sql.bsite.net
```bash
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "DatabaseOptions:Provider" "sqlserver"
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "ConnectionStrings:DefaultConnection" "Server=sql.bsite.net\MSSQL2016;Database=academynet_AcademyDB;User Id=academynet_AcademyDB;TrustServerCertificate=True"
```
- Después de `User Id=academynet_AcademyDB;` agrega manualmente la propriedad `Password=`, con la contraseña del servidor, antes de ejecutar el comando.

#### Alternativa: SQLite local
```bash
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "DatabaseOptions:Provider" "sqlite"
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "ConnectionStrings:DefaultConnection" "Data Source=C:\Users\DSANVICE\OneDrive - Capgemini\Documents\academia_kof_sqlite"
```

#### Alternativa: SQL Server Express (instancia nombrada)
```bash
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "DatabaseOptions:Provider" "sqlserverexpress"
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "ConnectionStrings:DefaultConnection" "Server=TU_EQUIPO\SQLEXPRESS;Database=CartDB;Trusted_Connection=True;TrustServerCertificate=True;"
```
Reinicia la API cada vez que cambies de proveedor.

---

## English

### Overview
Academy.Users is a .NET 8 solution structured with Clean Architecture to manage user personal data. It relies on modular layers (`Domain`, `Application`, `Infrastructure`, `Presentation`, `API`) plus per-layer xUnit test projects under `tests/*`.

### Solution layout
- `src/Academy.Users.Domain`: core entities (`User`).
- `src/Academy.Users.Application`: CQRS handlers (e.g., `UpdateUserCommandHandler`).
- `src/Academy.Users.Infrastructure`: EF Core configuration/repositories for SQLite or SQL Server.
- `src/Academy.Users.Presentation`: minimal API endpoints and DTOs.
- `src/Academy.Users.API`: host/composition root.
- `tests/*`: xUnit projects.

### Prerequisites
- .NET SDK 8.0+
- Access to SQL Server (primary option) or a local SQLite file for the alternate provider.

### Initial setup
1. Restore dependencies
   ```bash
   dotnet restore
   ```
2. Configure provider and connection via user secrets. Primary option targets SQL Server at `sql.bsite.net`:
   ```bash
   dotnet user-secrets init --project src/Academy.Users.API/Academy.Users.API.csproj
   dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "DatabaseOptions:Provider" "sqlserver"
   dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "ConnectionStrings:DefaultConnection" "Server=sql.bsite.net\MSSQL2016;Database=academynet_AcademyDB;User Id=academynet_AcademyDB;TrustServerCertificate=True"
   ```
   - Add the `Password=` property, with the server's password, immediately after `User Id=academynet_AcademyDB;` when you run the command locally.

3. Optional: run the SQL Server script above to recreate the `Users` table and seed sample data if needed.

### Build and run
```bash
dotnet build
dotnet run --project src/Academy.Users.API/Academy.Users.API.csproj
```

### API endpoint
| Method | Route                    | Description                                   |
|--------|-------------------------|-----------------------------------------------|
| PUT    | `/api/v1/users/{userId}` | Updates personal information for that user |

#### Sample request
```json
{
  "firstName": "Ana María",
  "lastName": "López Hernández",
  "phoneNumber": "+525511122233",
  "address": "Av. Reforma 500, Piso 12, Ciudad de México"
}
```

### Database providers
The app reads:
1. `DatabaseOptions:Provider` – defaults to `sqlserver`; alternatives `sqlite`, `sqlserverexpress`.
2. `ConnectionStrings:DefaultConnection` – provider-specific connection string.

#### Primary (SQL Server hosted at sql.bsite.net)
```bash
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "DatabaseOptions:Provider" "sqlserver"
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "ConnectionStrings:DefaultConnection" "Server=sql.bsite.net\MSSQL2016;Database=academynet_AcademyDB;User Id=academynet_AcademyDB;TrustServerCertificate=True"
```
- Add the `Password=` property, with the server's password, immediately after `User Id=academynet_AcademyDB;` when you run the command locally.

#### Alternate: local SQLite
```bash
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "DatabaseOptions:Provider" "sqlite"
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "ConnectionStrings:DefaultConnection" "Data Source=C:\Users\DSANVICE\OneDrive - Capgemini\Documents\academia_kof_sqlite"
```

#### Alternate: SQL Server Express (named instance)
```bash
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "DatabaseOptions:Provider" "sqlserverexpress"
dotnet user-secrets set --project src/Academy.Users.API/Academy.Users.API.csproj "ConnectionStrings:DefaultConnection" "Server=YOUR_MACHINE\SQLEXPRESS;Database=CartDB;Trusted_Connection=True;TrustServerCertificate=True;"
```
Restart the API after changing secrets to load the new provider/connection.
