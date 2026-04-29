using Microsoft.EntityFrameworkCore;

public class SqlDbContext : DbContext
{
    public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options) { }

    public DbSet<TestItem> TestItems { get; set; }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }
    public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
    public DbSet<PropertyAgent> PropertyAgents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User -> Agent (one-to-one)
        modelBuilder.Entity<UserProfile>()
            .HasOne(u => u.Agent)
            .WithOne(a => a.UserProfile)
            .HasForeignKey<Agent>(a => a.Id);

        // PropertyAgent composite primary key
        modelBuilder.Entity<PropertyAgent>()
            .HasKey(pa => new { pa.PropertyId, pa.AgentId });

        modelBuilder.Entity<PropertyAgent>()
            .HasOne(pa => pa.Property)
            .WithMany(p => p.PropertyAgents)
            .HasForeignKey(pa => pa.PropertyId);

        modelBuilder.Entity<PropertyAgent>()
            .HasOne(pa => pa.Agent)
            .WithMany()
            .HasForeignKey(pa => pa.AgentId);
    }
}