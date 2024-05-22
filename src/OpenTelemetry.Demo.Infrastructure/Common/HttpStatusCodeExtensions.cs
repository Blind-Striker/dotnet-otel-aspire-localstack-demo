﻿namespace OpenTelemetry.Demo.Infrastructure.Common;

internal static class HttpStatusCodeExtensions
{
    public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
    {
        return (int)statusCode >= 200 && (int)statusCode <= 299;
    }
}