using DiscountCodeGenerator.Domain.Entities;
using DiscountCodeGenerator.Domain.Repositories;
using DiscountCodeGenerator.Infrastructure.Data;
using DiscountCodeGenerator.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DiscountCodeGenerator.Tests.IntegrationTests
{
    public class DiscountCodeRepositoryTests
    {
        private DiscountCodeDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DiscountCodeDbContext>()
                .UseInMemoryDatabase(databaseName: "DiscountCodeTestDb")
                .Options;

            return new DiscountCodeDbContext(options);
        }

        [Fact]
        public async Task GenerateDiscountCodes_ShouldAddCodesToDatabase()
        {
            var context = GetInMemoryDbContext();
            var repository = new DiscountCodeRepository(context);

            var generatedCodes = await repository.GenerateDiscountCodes(5, 8);

            Assert.Equal(5, generatedCodes.Count);
            foreach (var code in generatedCodes)
            {
                Assert.NotNull(await context.DiscountCodes.FirstOrDefaultAsync(c => c.Code == code.Code));
            }
        }

        [Fact]
        public async Task UseDiscountCode_ShouldMarkCodeAsUsed()
        {
            var context = GetInMemoryDbContext();
            var repository = new DiscountCodeRepository(context);

            var discountCode = new DiscountCode { Code = "VALIDCODE", IsUsed = false };
            await context.DiscountCodes.AddAsync(discountCode);
            await context.SaveChangesAsync();

            var result = await repository.UseDiscountCode("VALIDCODE");

            Assert.True(result); 
            var usedCode = await context.DiscountCodes
                                        .Where(c => c.Code == "VALIDCODE")
                                        .FirstOrDefaultAsync();
            Assert.True(usedCode.IsUsed);
        }

        [Fact]
        public async Task UseDiscountCode_ShouldReturnFalse_WhenCodeDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var repository = new DiscountCodeRepository(context);

            var result = await repository.UseDiscountCode("NONEXISTENTCODE");

            Assert.False(result);
        }

        [Fact]
        public async Task CodeExistsAsync_ShouldReturnTrue_WhenCodeExists()
        {
            var context = GetInMemoryDbContext();
            var repository = new DiscountCodeRepository(context);

            var discountCode = new DiscountCode { Code = "EXISTINGCODE", IsUsed = false };
            await context.DiscountCodes.AddAsync(discountCode);
            await context.SaveChangesAsync();

            var result = await repository.CodeExistsAsync("EXISTINGCODE");

            Assert.True(result);
        }

        [Fact]
        public async Task CodeExistsAsync_ShouldReturnFalse_WhenCodeDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var repository = new DiscountCodeRepository(context);

            var result = await repository.CodeExistsAsync("NONEXISTENTCODE");

            Assert.False(result);
        }
    }
}
