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
    }
}