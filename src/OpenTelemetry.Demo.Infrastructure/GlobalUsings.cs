global using System.Diagnostics;
global using System.Net;
global using System.Net.Http.Json;
global using System.Text.Json;
global using System.Text.Json.Serialization;

global using Amazon.Runtime;

global using AWS.Messaging;
global using AWS.Messaging.Publishers;

global using FluentValidation;
global using FluentValidation.Results;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;

global using OneOf;
global using OneOf.Types;

global using OpenTelemetry.Demo.Infrastructure.Common;
global using OpenTelemetry.Demo.Infrastructure.Data;
global using OpenTelemetry.Demo.Infrastructure.Domain.Event.Entities;
global using OpenTelemetry.Demo.Infrastructure.Domain.Event.Interfaces;
global using OpenTelemetry.Demo.Infrastructure.Domain.Event.Models;
global using OpenTelemetry.Demo.Infrastructure.Domain.Ticket.Entities;
global using OpenTelemetry.Demo.Infrastructure.Domain.Ticket.Interfaces;
global using OpenTelemetry.Demo.Infrastructure.Domain.Ticket.Models;
global using OpenTelemetry.Demo.Infrastructure.Domain.User.Entities;
global using OpenTelemetry.Demo.Infrastructure.Domain.User.Interfaces;
global using OpenTelemetry.Demo.Infrastructure.Domain.User.Models;
global using OpenTelemetry.Demo.Infrastructure.Integrations;
global using OpenTelemetry.Demo.Infrastructure.Json;