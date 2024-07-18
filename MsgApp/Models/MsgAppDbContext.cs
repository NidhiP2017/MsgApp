using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MsgApp.Models
{
    public class MsgAppDbContext : IdentityDbContext<ChatUsers>
    {
        public MsgAppDbContext() { }
        public MsgAppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<ChatUsers> ChatUsers { get; set; }
        public DbSet<Messages> Messages { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MsgAppDb;Trusted_Connection=true");
                optionsBuilder.UseSqlServer("Server=HP;Database=MsgAppDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
