# Tracing the Future: Enhanced Observability in .NET with OpenTelemetry

This repository hosts a demonstration from the "Tracing the Future" presentation, delivered at [Devnot's .NET Conference 2024](https://dotnet.devnot.com/index.html). The project showcases practical implementations of OpenTelemetry to improve observability within .NET applications, and uses .NET Aspire with AWS and LocalStack for local development.

The demo includes two main scenarios: HTTP and SNS/SQS, highlighting synchronous and asynchronous communication between microservices.

## 🚀 Quick Start

```bash
# Prerequisites: .NET 10 SDK, Docker Desktop
git clone https://github.com/Blind-Striker/dotnet-otel-aspire-localstack-demo
cd dotnet-otel-aspire-localstack-demo
dotnet run --project local/aspire/OpenTelemetry.Demo.AspireHost
```

Then:

1. Open Aspire Dashboard (auto-launches in browser)
2. Navigate to EventApi Scalar UI (URL shown in terminal)
3. Create users → Register to events → View distributed traces in Aspire Dashboard!

## Why This Project?

This demo addresses common challenges in modern .NET cloud development:

- ✅ **Local AWS Development** - LocalStack integration via [LocalStack.Aspire.Hosting](https://github.com/localstack-dotnet/dotnet-aspire-for-localstack) eliminates AWS costs and network latency
- ✅ **Observable by Default** - Consolidated OpenTelemetry setup with ASP.NET Core, HttpClient, AWS SDK, and AWS.Messaging instrumentation
- ✅ **Aspire Orchestration** - AppHost manages all services and containers; no docker-compose needed
- ✅ **Infrastructure as Code** - Embedded AWS CDK stack provisions SNS topics and SQS queues, automatically wired to services
- ✅ **Modern AWS SDK** - Upgraded to AWS SDK for .NET v4 and [LocalStack.NET v2](https://github.com/localstack-dotnet/localstack-dotnet-client) for consistent client configuration
- ✅ **Two Integration Modes** - Compare synchronous (HTTP) vs asynchronous (SNS/SQS) distributed tracing patterns

## Architecture and Scenarios

This demo includes two main scenarios:

### HTTP Scenario

Demonstrates tracing a synchronous flow where services communicate directly over HTTP. This scenario highlights the real-time interactions between microservices.

![Scenario1](https://raw.githubusercontent.com/Blind-Striker/dotnet-otel-aspire-localstack-demo/master/assets/scenario1.png)

### SNS/SQS Scenario

Demonstrates  asynchronous communication using AWS SNS for notifications and SQS for message queuing. This scenario highlights the decoupling of services and the benefits of message-based communication.

![Scenario2](https://raw.githubusercontent.com/Blind-Striker/dotnet-otel-aspire-localstack-demo/master/assets/scenario2.png)

### Project Components

- **OpenTelemetry.Demo.AspireHost**: Aspire Host project automatically launches and configures all necessary .NET services along with Docker containers.
- **OpenTelemetry.Demo.Local.Database**: Responsible for creating the database and seeding it with initial data. It supports both MSSQL and PostgreSQL databases. See [configuration](#configuration) section for more details.
- **OpenTelemetry.Demo.EventApi:** Manages user creation, event listing, and registration to events. Following a user registration, it initiates ticket creation requests either through direct HTTP calls or via SNS+SQS. See [Interacting with the API](#interacting-with-the-api) and [configuration](#configuration) sections for more details.
- **OpenTelemetry.Demo.TicketApi:** Processes ticket creation post-event registration when operating over HTTP. It receives ticket creation requests from EventApi and processes them accordingly.
- **OpenTelemetry.Demo.TicketProcessor:** Processes ticket creation post-event registration when operating over SNS+SQS. It listens to the SQS queue for ticket creation requests and processes them accordingly.
- **AWS CDK in AppHost:** An embedded CDK stack provisions the `TicketTopic` (SNS) and `TicketQueue` (SQS) and wires their outputs into the AppHost so services can publish/consume without manual setup.

## Key Technologies and Packages

- [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview): A cloud-ready stack for building observable, distributed applications in .NET.
- [LocalStack](https://www.localstack.cloud/): Provides a local development environment for AWS cloud stack, allowing for full functionality without requiring actual AWS services.
- [OpenTelemetry](https://opentelemetry.io/): An open-source observability framework for cloud-native software, providing metrics, logs, and traces for applications.
- [AWS.Messaging](https://github.com/awslabs/aws-dotnet-messaging): Facilitates message processing with AWS services like SQS, SNS, and EventBridge.
- [LocalStack.NET](https://github.com/localstack-dotnet/localstack-dotnet-client): A .NET client for LocalStack, offering a simplified wrapper for [aws-sdk-net](https://github.com/aws/aws-sdk-net) that configures endpoints to use LocalStack, facilitating local AWS cloud development. Version 2.0 provides consistent client configuration and enhanced AWS SDK v4 compatibility for streamlined development workflows.
- [.NET Aspire Integrations for LocalStack](https://github.com/localstack-dotnet/dotnet-aspire-for-localstack): Integration library that enables first-class LocalStack management from .NET Aspire (used here via `LocalStack.Aspire.Hosting`).
- [OneOf](https://github.com/mcintyre321/OneOf): Implements F# style discriminated unions in C#, simplifying complex conditional logic.
- [Serilog.Sinks.OpenTelemetry](https://github.com/serilog/serilog-sinks-opentelemetry): A Serilog sink transforms Serilog events into OpenTelemetry LogRecords and sends them to an OTLP (gRPC or HTTP) endpoint.

## Setup and Local Development

Follow these steps to set up the project locally:

### Prerequisites

Ensure the following prerequisites are met before proceeding with the local setup:

- **.NET 10.0:** Install from [official .NET download page](https://dotnet.microsoft.com/en-us/download)
- **.NET Aspire:** Uses the new NuGet SDK model for simplified setup. See [.NET Aspire setup and tooling](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling) for more information.
- **Container Runtime:** An OCI-compliant container runtime like Docker Desktop or Podman is necessary. For more information, refer to the [Container Runtime documentation](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=linux&pivots=visual-studio#container-runtime).
- **IDE or Code Editor:** While optional, it's beneficial to use:
  - Visual Studio 2022 version 17.13 or higher.
  - Visual Studio Code.
  - JetBrains Rider with the [.NET Aspire plugin](https://plugins.jetbrains.com/plugin/23289--net-aspire) installed.

## Usage

The `OpenTelemetry.Demo.AspireHost` is an Aspire Host project that launches and configures all necessary .NET services, including EventApi, TicketApi, and TicketProcessor, as well as essential containers for MSSQL, PostgreSQL, and LocalStack. Simply running this project is sufficient to initialize and operate the entire application environment.

### Configuration

In the `launchSettings.json` of the `OpenTelemetry.Demo.AspireHost` project (or via environment variables at runtime), two keys drive the composition:

- `EventSystem:DatabaseType` — Database engine: `npgsql` (PostgreSQL, default) or `sqlserver` (Microsoft SQL Server).
- `EventSystem:TicketIntegration` — Integration mode between EventApi and ticketing services: `aws` (SNS+SQS, default) or `http` (direct HTTP to TicketApi).

Defaults are set to PostgreSQL + AWS. When set to `aws`, AppHost runs the `TicketProcessor` worker and wires SNS/SQS via the embedded CDK stack. When set to `http`, AppHost runs the `TicketApi` and wires EventApi’s HttpClient via Aspire service discovery.

Environment variables can be provided using the standard .NET double-underscore format. For example (PowerShell):

```powershell
$env:EventSystem__DatabaseType = "npgsql"
$env:EventSystem__TicketIntegration = "aws"
dotnet run --launch-profile "http"
```

### Running the Application

The `OpenTelemetry.Demo.AspireHost` project can be run either directly through your IDE or via the command line:

#### IDE

1. Open your IDE (Visual Studio, Visual Studio Code, JetBrains Rider, etc.).
2. Navigate to the OpenTelemetry.Demo.AspireHost project.
3. Start the project using the relevant IDE's run/debug functionality.

#### Command Line

- Open a terminal and navigate to the `OpenTelemetry.Demo.AspireHost` project directory.
- To run the application with HTTPS (requires configured developer certificates), execute:

```bash
dotnet run --launch-profile "https"
```

If you encounter issues with HTTPS, ensure that your developer certificates are properly installed and configured. For more information, see [dotnet-dev-certs](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs).

- Alternatively, to run the application with HTTP

```bash
dotnet run --launch-profile "http"
```

Once the services are up:

- The Aspire dashboard will be accessible, providing an overview of service health and metrics.
- You can also access the EventApi's Scalar UI to interact with the API directly. The URL to the Scalar UI will be displayed in the terminal or in the IDE's output window.

### LocalStack and AWS Resources

- LocalStack is managed by the AppHost via `LocalStack.Aspire.Hosting`; no separate docker-compose is required.
- The AppHost synthesizes an AWS CDK stack that creates:
  - `TicketTopic` (SNS) and `TicketQueue` (SQS)
  - A subscription that routes topic messages to the queue
  - Necessary queue policy for SNS to publish
- Topic ARN and Queue URL are exported and injected into services through Aspire references.

### Interacting with the API

- Create Users:
  - Use the `POST /user` endpoint to create new users. This can be done via the Scalar UI or using a tool like Postman.
- Register Users to Events:
  - With users created, use the `POST /event/register` endpoint to register these users to events. Events are pre-seeded and available for registration.
  - In `aws` mode, EventApi publishes a message to SNS; TicketProcessor consumes from SQS and creates the ticket.
  - In `http` mode, EventApi calls TicketApi directly over HTTP to create the ticket.

### Observability and Monitoring

- Access the Aspire dashboard to review logs, distributed traces, and metrics to gain insights into the application's performance and behavior.
- The dashboard provides a comprehensive view of the system's operations, making it easier to troubleshoot issues or understand the flow of data through your application.

### Additional Tips

- Ensure that all configurations in `launchSettings.json` are set according to your local setup needs.
- Experiment with different configurations to see how the system behaves under various conditions, especially when switching between database types or integration strategies.

## Next Steps / Roadmap

### Must-Fix Before Production

- [ ] **Add Automated Tests** - Implement unit, integration, and [Aspire-specific tests](https://learn.microsoft.com/en-us/dotnet/aspire/testing/overview) to validate distributed scenarios
- [ ] **Database Migrations** - Replace `EnsureCreated()` with proper EF Core migrations for production-safe schema management
- [ ] **Database Separation** - Transition from a single database setup to distinct databases per service (users, events, tickets) for better isolation and scalability
- [ ] **Error Handling** - Improve global exception handling and add retry policies for AWS operations

### Enhancements

- [ ] **Observability Tools Integration** - Add examples for external OTLP collectors (Jaeger, Prometheus, Zipkin)
- [ ] **Detailed Setup Instructions** - Extend documentation to include AWS CDK customization guidance and troubleshooting
- [ ] **Additional Scenarios** - Demonstrate saga patterns, compensation logic, or CQRS with event sourcing

Additionally, the [localstack-dotnet](https://github.com/localstack-dotnet) organization will develop more extensive example projects that leverage technologies such as Aspire, OpenTelemetry, AWS, and [LocalStack.NET](https://github.com/localstack-dotnet/localstack-dotnet-client).

## Contribution and License

Contributions to this project are welcome! Feel free to submit issues or pull requests to improve the project.

This project is released under the [MIT License](LICENSE.md)
