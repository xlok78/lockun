using KhaoThi23.Models;
using Microsoft.EntityFrameworkCore;

namespace KhaoThi23.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Noti> Notis { get; set; }
        public DbSet<PhucKhao> PhucKhaos { get; set; }
        public DbSet<News> News { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasOne(e => e.Employee)
                .WithOne(a => a.Account)
                .HasForeignKey<Employee>(a => a.AccountId);

            modelBuilder.Entity<Account>()
                .HasKey(e => e.AccountId);

            modelBuilder.Entity<PhucKhao>()
            .HasOne(p => p.Employee)
            .WithMany()
            .HasForeignKey(p => p.EmployeeId);

            modelBuilder.Entity<News>()
                .HasKey(c => c.NewsId);

            modelBuilder.Entity<News>()
                .HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId);
//ok cơ bản là ông kiểm xem đúng ko
        }
    }
}
