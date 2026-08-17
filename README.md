# Event Management API

## Overview

This solution is implemented using **.NET 10** and follows a **Clean Architecture / DDD-style approach**.

The primary objective was to produce a maintainable API with a clear separation of concerns while keeping the implementation appropriately simple for the scope of the assessment.

> **Framework Note:** The original intention was to target .NET 5. However, .NET 5 was not available in my development environment, so the solution was implemented using .NET 10 instead. The architectural principles and implementation approach remain applicable to the requested solution.

---

## Architecture

The solution follows a **Clean Architecture / DDD-style structure**, with responsibilities separated across the application layers.

The general separation is:

* **API** – HTTP endpoints and request/response handling
* **Application** – Business logic and application services
* **Domain** – Domain models and business concepts
* **Infrastructure** – Database access, repositories, EF Core configuration and external infrastructure concerns

The intention is to keep business logic independent of the API and infrastructure implementations wherever practical.

---

## Minimal APIs

I chose **ASP.NET Minimal APIs** for the API layer rather than traditional MVC Controllers.

The main reasons for this decision are:

* Reduced boilerplate
* Cleaner endpoint definitions
* Faster development
* Explicit request/response contracts
* Easier separation of endpoint configuration from business logic

When Minimal APIs are structured correctly, they also encourage the API layer to remain thin. Endpoints should primarily be responsible for:

1. Receiving the request
2. Validating or binding the input
3. Calling the appropriate application service
4. Returning the appropriate HTTP response

Business rules and processing should remain outside the endpoint itself.

For this reason, the endpoint configuration has been separated into dedicated endpoint classes rather than placing all endpoints directly in `Program.cs`.

---

## Application Layer

For this assessment, I deliberately chose a **service-based application layer** rather than introducing a full CQRS implementation.

A full CQRS structure would introduce additional commands, handlers, mappings and supporting infrastructure. While CQRS can be valuable for larger systems, I did not believe the additional complexity was justified for the scope of this assessment.

The application layer therefore uses services to encapsulate business logic.

This keeps the API layer thin while avoiding unnecessary architectural overhead.

### Why not CQRS?

The decision was primarily based on proportionality.

The goal was to demonstrate:

* Separation of concerns
* Business logic isolation
* Dependency inversion
* Testable application services
* A maintainable architecture

without introducing patterns purely for the sake of demonstrating them.

For a larger or more complex domain, CQRS could be introduced later without fundamentally changing the overall architecture.

---

## Entity Framework Core

**Entity Framework Core** is used as the ORM for database access.

The primary reasons for choosing EF Core are:

* Strong integration with .NET
* Built-in dependency injection support
* Automatic connection and `DbContext` lifecycle management
* LINQ-based querying
* Strongly typed entity mapping
* Reduced manual data-access boilerplate
* Support for connection pooling through the underlying database provider
* Good support for transactions and change tracking

EF Core also provides a clean abstraction over database access while still allowing SQL to be used directly when required.

For this solution, EF Core provides a good balance between development speed, maintainability and type safety.

---

## Database-First Approach

I chose a **database-first approach** for this assessment rather than EF Core migrations.

The primary reason is to keep the database schema explicit and independently reviewable.

Database changes are represented as SQL scripts rather than being generated implicitly through application code.

This provides several benefits:

* SQL can be reviewed independently of the application
* Database changes are explicit
* Database scripts can be version controlled
* DBAs can review and execute the SQL independently
* The database schema is not dependent on the EF Core migration history

I have intentionally avoided introducing EF Core migrations into this assessment. In real-world projects, migrations should still be managed through a controlled database deployment process.

### Future Improvement

For a production implementation, I would introduce a dedicated database migration/versioning tool such as **Flyway** or an equivalent solution.

This would provide controlled database versioning and allow changes to be promoted consistently across environments.

For the purposes of this assessment, the required SQL scripts are included in the solution under the database/SQL directory.

---

## Event History and Auditability

To support **data preservation and audit logging**, an event history table has been introduced.

Rather than relying exclusively on the current state of an entity, important events can be retained as historical records.

This provides the ability to:

* Preserve historical changes
* Understand what happened to an entity
* Support auditing
* Investigate previous states or actions
* Maintain an immutable record of relevant events

This approach also leaves room for the system to evolve towards a more comprehensive event-driven or event-sourced design if the requirements eventually justify it.

---

## Structured Logging

The solution uses **Serilog** for structured application logging.

Serilog was chosen because it provides a more flexible and structured logging approach than relying solely on the default logging providers. In particular, it allows log events to be captured with named properties and contextual information rather than treating logs as simple text messages.

This provides several benefits:

* Structured and searchable log events
* Consistent logging across the application
* Support for enriching logs with contextual properties
* Flexible output sinks, allowing logs to be directed to different destinations
* Better support for production observability and troubleshooting
* Easier integration with centralised logging platforms in a production environment

For this assessment, the primary goal is to establish a consistent structured logging foundation that can be extended with additional sinks and enrichment as the solution evolves.

---

## Global Exception Handling

The API uses a **global exception handler** to centralise exception handling rather than placing repetitive `try/catch` blocks throughout the application.

The handler distinguishes between expected validation errors and unexpected application errors. `ArgumentException` is treated as an expected validation exception and is not logged as an application failure. These exceptions result in an HTTP **400 Bad Request** response.

For unexpected or unhandled exceptions, the implementation logs the full exception details for troubleshooting while returning a generic error message to the client. This prevents internal implementation details or sensitive exception information from being exposed through the API.

This approach provides a consistent error-handling strategy while avoiding unnecessary log noise from expected validation failures. It also keeps the endpoint and application code cleaner by separating exception handling from the core business logic.

---

## API Client Generation

**NSwag.MSBuild** is used to generate the API client from the application's OpenAPI specification.

NSwag is an established tool in the .NET ecosystem for OpenAPI/Swagger generation and client code generation. It supports generating strongly typed C# clients directly from an OpenAPI specification, which helps avoid manually maintaining HTTP client code and DTOs.

The MSBuild integration was chosen so that client generation can be incorporated into the build process rather than requiring developers to manually run a separate code-generation command. The `NSwag.MSBuild` package exposes the NSwag tooling to MSBuild targets and supports executing an `nswag.json` configuration as part of the build.

This provides a few practical benefits:

* Strongly typed API clients
* Reduced manually written HTTP client code
* Generated clients remain aligned with the OpenAPI contract
* Repeatable client generation
* Easier integration into CI/CD pipelines
* Reduced risk of the client implementation drifting from the API contract

For this solution, the decision was primarily driven by maintainability and consistency rather than introducing custom client-generation logic.

---

## Dependency Injection

Dependency Injection registration has been intentionally separated according to architectural responsibility.

Rather than placing all registrations inside `Program.cs`, infrastructure registrations are grouped into their own configuration class.

For example:

```text
Infrastructure
└── DependencyInjection.cs
```

This keeps `Program.cs` focused on application startup and composition while allowing each architectural layer to manage its own dependency registration.

This approach becomes increasingly useful as the application grows and additional infrastructure dependencies are introduced.

---

## Database Connections and Connection Pooling

When using micro-ORMs such as **Dapper**, I generally prefer repositories to be registered as singletons where appropriate, while explicitly managing database connection lifetimes using `using` statements.

The connection itself is disposed after the operation, while the underlying database provider manages connection pooling.

With EF Core, this responsibility is handled differently.

`DbContext` instances are normally registered using their appropriate scoped lifetime, while EF Core and the underlying database provider manage connections and connection pooling.

Connection-pooling behaviour can therefore be configured through the database connection string/provider rather than manually maintaining a pool inside the repository layer.

This allows the application to benefit from connection pooling without requiring custom connection lifecycle management.

---

## User Identity and `CreatedBy`

The `CreatedBy` value is deliberately not accepted as a trusted value from the initial POST request.

Accepting a `UserId` directly from the request body would allow a client to submit another user's ID, for example:

```json
{
  "createdBy": "another-user-id"
}
```

This should not be trusted in a production application.

For the purposes of this assessment, the value is hard-coded to demonstrate the concept and avoid treating client-provided identity information as authoritative.

### Production Implementation

In a production implementation, the authenticated user's identity should be extracted from the **JWT access token claims**.

The flow would be:

```text
Client
   |
   | JWT
   v
API
   |
   | Claims / User Identity
   v
Application Service
   |
   v
Repository
   |
   v
Database
```

The client should therefore not be responsible for determining the identity associated with the created record.

The server should derive this information from the authenticated security context.

---

## Design Principles

The implementation focuses on the following principles:

* **Separation of Concerns**
* **Dependency Inversion**
* **Thin API Layer**
* **Business Logic Isolation**
* **Explicit Database Changes**
* **Auditability**
* **Maintainability**
* **Appropriate Architectural Complexity**

The intention is not to introduce every available architectural pattern, but rather to use patterns where they provide clear value for the size and requirements of the solution.
