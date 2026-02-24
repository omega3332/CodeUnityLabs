using CodeUnityLabs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeUnityLabs.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Your existing tables
        public DbSet<User> Users { get; set; }
        public DbSet<Authorizations> Authorizations { get; set; }
        public DbSet<WaitingList> WaitingList { get; set; }
        public DbSet<Rezervation> Reservations { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<UserType> UserTypes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // MUST call this for Identity

            // Seed UserTypes
            modelBuilder.Entity<UserType>().HasData(
                new UserType { User_Type_Id = 1, Type_Name = "Admin" },
                new UserType { User_Type_Id = 2, Type_Name = "Staff" },
                new UserType { User_Type_Id = 3, Type_Name = "Student" }
            );

            // Your existing constraints & relationships
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Authorizations>()
                .HasOne(a => a.User)
                .WithMany(u => u.Authorizations)
                .HasForeignKey(a => a.User_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rezervation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rezervations)
                .HasForeignKey(r => r.User_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WaitingList>()
                .HasOne(w => w.User)
                .WithMany(u => u.WaitingLists)
                .HasForeignKey(w => w.User_Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}