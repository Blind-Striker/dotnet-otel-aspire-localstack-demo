global using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
global using Aspire.Npgsql.EntityFrameworkCore.PostgreSQL;

global using AWS.Messaging.Telemetry.OpenTelemetry;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Logging;

global using Npgsql.EntityFrameworkCore.PostgreSQL;

global using OpenTelemetry;
global using OpenTelemetry.Demo.Infrastructure.Data;
global using OpenTelemetry.Metrics;
global using OpenTelemetry.Trace;

global using Serilog;
global using Serilog.Enrichers.Span;
global using Serilog.Exceptions;
global using Serilog.Exceptions.Core;
global using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
global using Serilog.Sinks.OpenTelemetry;