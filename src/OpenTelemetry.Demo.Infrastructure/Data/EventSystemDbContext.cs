namespace OpenTelemetry.Demo.Infrastructure.Data;

public class EventSystemDbContext(DbContextOptions<EventSystemDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<EventEntity> Events { get; set; }
    public DbSet<RegistrationEntity> Registrations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<UserEntity>()
            .HasKey("_id");
        modelBuilder.Entity<UserEntity>()
            .Property("_id")
            .ValueGeneratedOnAdd(); // Ensures ID is auto-increment

        modelBuilder.Entity<UserEntity>()
            .Property(u => u.Email)
            .IsRequired();

        // Event entity configuration
        modelBuilder.Entity<EventEntity>()
            .HasKey("_id");
        modelBuilder.Entity<EventEntity>()
            .Property("_id")
            .ValueGeneratedOnAdd(); // Ensures ID is auto-increment

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

        // modelBuilder.Metadata.FindNavigation(nameof(UserEntity.Registrations))
        //     .SetPropertyAccessMode(PropertyAccessMode.Field);
        //
        // modelBuilder.Metadata.FindNavigation(nameof(EventEntity.Registrations))
        //     .SetPropertyAccessMode(PropertyAccessMode.Field);

        // If Registration had its own primary key
        // modelBuilder.Entity<Registration>()
        //     .Property(r => r.SomeId)
        //     .ValueGeneratedOnAdd();  // If you have a separate primary key for Registration
    }
}