using Microsoft.EntityFrameworkCore;

namespace ClinicService.Data.Context
{
    public class ClinicServiceDbContext : DbContext
    {
        public ClinicServiceDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Consultation>()
                .HasOne(c => c.Pet)
                .WithMany(p => p.Consultations)
                .HasForeignKey(p => p.PetId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Pet> Pets { get; set; }

        public DbSet<Consultation> Consultations { get; set; }
    }
}
