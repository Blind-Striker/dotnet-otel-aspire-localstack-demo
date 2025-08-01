# Tracing the Future: Enhanced Observability in .NET with OpenTelemetry

This repository hosts the demonstration from the "Tracing the Future" presentation, delivered at [Devnot's .NET Conference 2024](https://dotnet.devnot.com/index.html). The project showcases practical implementations of OpenTelemetry to improve observability within .NET applications as well as using Aspire with AWS and LocalStack for local development.

The demo includes two main scenarios: HTTP and SNS/SQS, highlighting synchronous and asynchronous communication between microservices.

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

## Key Technologies and Packages

- [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview): A cloud-ready stack for building observable, distributed applications in .NET.
- [LocalStack](https://www.localstack.cloud/): Provides a local development environment for AWS cloud stack, allowing for full functionality without requiring actual AWS services.
- [OpenTelemetry](https://opentelemetry.io/): An open-source observability framework for cloud-native software, providing metrics, logs, and traces for applications.
- [AWS.Messaging](https://github.com/awslabs/aws-dotnet-messaging): Facilitates message processing with AWS services like SQS, SNS, and EventBridge.
- [LocalStack.NET](https://github.com/localstack-dotnet/localstack-dotnet-client): A .NET client for LocalStack, offering a simplified wrapper for [aws-sdk-net](https://github.com/aws/aws-sdk-net) that configures endpoints to use LocalStack, facilitating local AWS cloud development. Version 2.0 provides consistent client configuration and enhanced AWS SDK v4 compatibility for streamlined development workflows.
- [OneOf](https://github.com/mcintyre321/OneOf): Implements F# style discriminated unions in C#, simplifying complex conditional logic.
- [Serilog.Sinks.OpenTelemetry](https://github.com/serilog/serilog-sinks-opentelemetry): A Serilog sink transforms Serilog events into OpenTelemetry LogRecords and sends them to an OTLP (gRPC or HTTP) endpoint.

## Setup and Local Development

Follow these steps to set up the project locally:

### Prerequisites

Ensure the following prerequisites are met before proceeding with the local setup:

- **.NET 9.0:** Install from [official .NET download page](https://dotnet.microsoft.com/en-us/download)
- **.NET Aspire:** Uses the new NuGet SDK model for simplified setup. See [.NET Aspire setup and tooling](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling) for more information.
- **Container Runtime:** An OCI-compliant container runtime like Docker Desktop or Podman is necessary. For more information, refer to the [Container Runtime documentation](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=linux&pivots=visual-studio#container-runtime).
- **IDE or Code Editor:** While optional, it's beneficial to use:
  - Visual Studio 2022 version 17.11 or higher.
  - Visual Studio Code.
  - JetBrains Rider with the [.NET Aspire plugin](https://plugins.jetbrains.com/plugin/23289--net-aspire) installed.

## Usage

The `OpenTelemetry.Demo.AspireHost` is an Aspire Host project that launches and configures all necessary .NET services, including EventApi, TicketApi, and TicketProcessor, as well as essential containers for MSSQL, PostgreSQL, and LocalStack. Simply running this project is sufficient to initialize and operate the entire application environment.

### Configuration

In the `launchSettings.json` of the `OpenTelemetry.Demo.AspireHost` project, there are two key environment variables:

- `EventSystem:DatabaseType:` Determines the type of database used, options include "npgsql" for PostgreSQL or "sqlserver" for Microsoft SQL Server.
- `EventSystem:TicketIntegration:` Specifies the communication strategy between EventApi and ticketing services. Set to "aws" to use AWS SNS and SQS, or "http" for direct HTTP calls.

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
- You can also access the EventApi's Swagger UI to interact with the API directly. The URL to the Swagger UI will be displayed in the terminal or in the IDE's output window.

### Interacting with the API

- Create Users:
  - Use the `POST /user` endpoint to create new users. This can be done via the Swagger UI or using a tool like Postman.
- Register Users to Events:
  - With users created, use the `POST /event/register` endpoint to register these users to events. Events are pre-seeded and available for registration.

### Observability and Monitoring

- Access the Aspire dashboard to review logs, distributed traces, and metrics to gain insights into the application's performance and behavior.
- The dashboard provides a comprehensive view of the system's operations, making it easier to troubleshoot issues or understand the flow of data through your application.

### Additional Tips

- Ensure that all configurations in `launchSettings.json` are set according to your local setup needs.
- Experiment with different configurations to see how the system behaves under various conditions, especially when switching between database types or integration strategies.

## Next Steps

To further enhance and expand this project, upcoming enhancements are planned as follows:

- **Database Separation:** Transition from a single database setup to distinct databases for users, events, and tickets to optimize architecture and scalability.
- **Detailed Setup Instructions:** Extend the setup guide to include Docker Compose and AWS CDK for more complex deployment scenarios.
- **Observability Tools Integration:** Add comprehensive examples and setup guidance for tools like Jaeger, Prometheus, and Zipkin.

Additionally, the [localstack-dotnet](https://github.com/localstack-dotnet) organization will develop more extensive example projects that leverage technologies such as Aspire, OpenTelemetry, AWS, and [LocalStack.NET](https://github.com/localstack-dotnet/localstack-dotnet-client).

## Contribution and License

Contributions to this project are welcome! Feel free to submit issues or pull requests to improve the project.

This project is released under the [MIT License](LICENSE.md)
