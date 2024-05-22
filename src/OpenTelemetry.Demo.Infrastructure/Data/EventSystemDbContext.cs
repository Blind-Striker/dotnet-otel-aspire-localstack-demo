namespace OpenTelemetry.Demo.Infrastructure.Data;

public class EventSystemDbContext(DbContextOptions<EventSystemDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }

    public DbSet<EventEntity> Events { get; set; }

    public DbSet<RegistrationEntity> Registrations { get; set; }

    public DbSet<TicketEntity> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<UserEntity>()
                    .HasKey(entity => entity.Id);
        modelBuilder.Entity<UserEntity>()
                    .Property(entity => entity.Id)
                    .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserEntity>()
                    .Property(u => u.Email)
                    .IsRequired();

        // Event entity configuration
        modelBuilder.Entity<EventEntity>()
                    .HasKey(entity => entity.Id);
        modelBuilder.Entity<EventEntity>()
                    .Property(entity => entity.Id)
                    .ValueGeneratedOnAdd();

        // Registration entity configuration
        modelBuilder.Entity<RegistrationEntity>()
                    .HasKey(r => new { r.UserId, r.EventId });

        modelBuilder.Entity<RegistrationEntity>()
                    .HasOne(r => r.UserEntity)
                    .WithMany(u => u.Registrations)
                    .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<RegistrationEntity>()
                    .HasOne(r => r.EventEntity)
                    .WithMany(e => e.Registrations)
                    .HasForeignKey(r => r.EventId);

        // Ticket entity configuration
        modelBuilder.Entity<TicketEntity>()
                    .HasKey(t => t.Id);

        modelBuilder.Entity<TicketEntity>()
                    .Property(u => u.Status)
                    .IsRequired();

        modelBuilder.Entity<TicketEntity>()
                    .HasOne(t => t.User)
                    .WithMany(u => u.Tickets)
                    .HasForeignKey(t => t.UserId);

        modelBuilder.Entity<TicketEntity>()
                    .HasOne(t => t.Event)
                    .WithMany(e => e.Tickets)
                    .HasForeignKey(t => t.EventId);
    }
}