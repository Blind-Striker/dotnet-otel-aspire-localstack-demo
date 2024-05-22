// Global using directives

global using System.Collections.Concurrent;
global using System.Runtime.ExceptionServices;

global using Amazon;
global using Amazon.CloudFormation;

global using Aspire.Hosting.AWS.CloudFormation;
global using Aspire.Hosting.Lifecycle;
global using HealthChecks.NpgSql;
global using HealthChecks.SqlServer;
global using HealthChecks.Uris;

global using LocalStack.Client;
global using LocalStack.Client.Contracts;
global using LocalStack.Client.Options;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Logging;

global using OpenTelemetry.Demo.AspireHost.LocalStack;

global using Polly;
global using Polly.Retry;