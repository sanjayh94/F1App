# F1 API

A Formula 1 data API with PostgreSQL database support.

## Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose

## Getting Started

### Option 1: Run everything with Docker Compose

```bash
docker-compose up -d
```

This will start:
- PostgreSQL on port 5432
- pgAdmin on port 5050 (access via http://localhost:5050 with email: admin@admin.com, password: pgadmin)
- F1Api on port 8080 (access via http://localhost:8080/swagger)

### Option 2: Run only the database with Docker and the API locally

```bash
# Start only the database
docker-compose up -d postgres pgadmin

# Run the API locally
cd F1Api
dotnet run
```

The application will automatically apply migrations and seed the database with data from the JSON files on first run.

## Database Details

- Host: localhost
- Port: 5432
- Database: f1db
- Username: f1user
- Password: f1password

## Migration Information

The project now uses EF Core migrations to create and seed the PostgreSQL database. The seeding process:

1. Reads F1 data from JSON files in the Data folder
2. Efficiently processes and loads data into PostgreSQL
3. Uses batching for large data sets to minimize memory usage

This approach is significantly faster than the previous in-memory database approach.

## Development

If you need to recreate migrations:

```bash
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
```

To update the database manually:

```bash
dotnet ef database update
```

## Troubleshooting

### Initial database setup taking too long

The initial database migration and seeding can take several minutes due to the large volume of lap time data. You can monitor progress with:

```bash
docker-compose logs -f f1api
``` 