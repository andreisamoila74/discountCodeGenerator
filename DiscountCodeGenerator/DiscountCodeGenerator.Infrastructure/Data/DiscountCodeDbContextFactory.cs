using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DiscountCodeGenerator.Infrastructure.Data
{
    public class DiscountCodeDbContextFactory : IDesignTimeDbContextFactory<DiscountCodeDbContext>
    {
        public DiscountCodeDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DiscountCodeDbContext>();
            optionsBuilder.UseSqlite("Data Source=Data/db.sqlite");

            return new DiscountCodeDbContext(optionsBuilder.Options);
        }
    }
}
