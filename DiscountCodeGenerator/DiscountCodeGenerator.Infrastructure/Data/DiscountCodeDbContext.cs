using DiscountCodeGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscountCodeGenerator.Infrastructure.Data
{
    public class DiscountCodeDbContext : DbContext
    {
        public DbSet<DiscountCode> DiscountCodes { get; set; }

        public DiscountCodeDbContext(DbContextOptions<DiscountCodeDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscountCode>()
                .HasIndex(dc => dc.Code)
                .IsUnique();
        }
    }
}