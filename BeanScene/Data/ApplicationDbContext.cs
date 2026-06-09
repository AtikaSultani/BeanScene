using BeanScene.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeanScene.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Area> Areas { get; set; }

        public DbSet<Table> Tables { get; set; }

        public DbSet<Sitting> Sittings { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<ReservationTable> ReservationTables { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ReservationTable>()
                .HasKey(rt => new { rt.ReservationId, rt.TableId });
        }
    }
}
