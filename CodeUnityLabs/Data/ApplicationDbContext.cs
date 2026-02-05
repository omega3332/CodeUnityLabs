using CodeUnityLabs.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeUnityLabs.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }


            
        public DbSet<User> Users { get; set; }
        public DbSet<Authorizations> Authorizations { get; set; }
        public DbSet<WaitingList> WaitingList { get; set; }
        public DbSet<Rezervation> Reservations { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<UserType> UserTypes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserType>().ToTable("UserType"); 
        }

        
    }
}
