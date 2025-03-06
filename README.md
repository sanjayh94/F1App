# F1 API

A Formula 1 data API built in .NET 8 (C#). The App parses data from json files into a local PostgreSQL database using EFCore migrations on the first start up. The API exposes the data through two primary endpoints:

- `/api/circuits` - Get all circuits with their respective races and results
- `/api/drivers` - Get all drivers with their respective teams and results

The API shows summary data for each circuit and driver which can be reached at `/api/circuits/summary` and `/api/drivers/summary` respectively.

The API also includes a Swagger UI for easy API testing and documentation.

The API is containerized and can be run using Docker Compose.

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
# Start only the database for local development
docker-compose up -d postgres pgadmin

# Run the API locally
cd F1Api
dotnet run
```

The application will automatically apply migrations and seed the database with data from the JSON files on first run.

## How to reach the API

The API documentation is available at http://localhost:8080/swagger and can also be used to test the API using the Swagger UI.

Alternatively, the API can be reached using curl or any other HTTP client such as Postman. The following endpoints are available:

- Circuits
    - `/api/circuits` - Get all circuits with their respective races and results
    - `/api/circuits/{circuitId}` - Get a specific circuit by ID
    - `/api/circuits/summary` - Get summary data for all circuits
    - `/api/circuits/{circuitId}/summary` - Get summary data for a specific circuit
- Drivers
    - `/api/drivers` - Get all drivers with their respective teams and results
    - `/api/drivers/{driverId}` - Get a specific driver by ID
    - `/api/drivers/summary` - Get summary data for all drivers
    - `/api/drivers/{driverId}/summary` - Get summary data for a specific driver
- HealthCheck
    - `/health` - Get the health of the API

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

## Health Checks

The API includes a health check endpoint at `/health` which returns a 200 OK status if the database is reachable.

## App Structure

The .NET Solution is structured as follows:

- `F1Api` - The main API project
- `F1Api.Test.Unit` - The unit test project
- `F1Api.Test.Acceptance` - The acceptance test project

## How to Run Tests

```bash
dotnet test
```

## Unit Tests

The Unit tests are written using NUnit and are located in the `F1Api.Test.Unit` project. The tests primarily test the service layer as that is where the business logic is implemented and should be thoroughly tested. The Unit tests will run indepedently of the application.

## Acceptance Tests

The Acceptance tests are also written using NUnit and are located in the `F1Api.Test.Acceptance` project. The tests are designed to test the API as a whole from the perspective of an end user and to test against business acceptance criterias.

The acceptance tests are designed to seed and create it's own state. Currently, the tests replaces the DBContext with a mock instance of the DBContext to avoid writing to the actual database and instead writes to an in-memory database. Ideally, the tests should run on another acceptance test database to avoid any potential side effects.

## Integration Tests and Smoke Tests

Integration tests and Smoke tests are not implemented in this project as the application is not deployed. However, the project is designed with a testable architecture and could be easily extended to include integration and smoke tests.

## Troubleshooting

### Initial database setup taking too long

The initial database migration and seeding can take several minutes due to the large volume of lap time data. You can monitor progress with:

```bash
docker-compose logs -f f1api
``` 