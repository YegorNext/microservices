using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ActionResourceMetric> Metrics { get; set; }
        public DbSet<UserAction> UserActions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAction>()
                .HasOne(u => u.ActionResourceMetric)
                .WithOne(m => m.UserAction)
                .HasForeignKey<ActionResourceMetric>(m => m.ActionId)
                .HasPrincipalKey<UserAction>(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
