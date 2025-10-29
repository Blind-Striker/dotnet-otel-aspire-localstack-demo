#pragma warning disable RCS1170, S1144
using System.Diagnostics.CodeAnalysis;

namespace OpenTelemetry.Demo.Infrastructure.Domain.User.Entities;

public class UserEntity
{
    private readonly List<RegistrationEntity> _registrations;
    private readonly List<TicketEntity> _tickets;

    private UserEntity()
    {
        _registrations = [];
        _tickets = [];
    }

    [SetsRequiredMembers]
    public UserEntity(string firstName, string lastName, string email)
        : this()
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public int Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public IReadOnlyCollection<RegistrationEntity> Registrations => _registrations;
    public IReadOnlyCollection<TicketEntity> Tickets => _tickets;
}
