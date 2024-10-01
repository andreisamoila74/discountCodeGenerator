using DiscountCodeGenerator.Domain.Repositories;
using DiscountCodeGenerator.Domain.Entities;
using DiscountCodeGenerator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscountCodeGenerator.Infrastructure.Repositories
{
    public class DiscountCodeRepository : IDiscountCodeRepository
    {
        private readonly DiscountCodeDbContext _context;

        public DiscountCodeRepository(DiscountCodeDbContext context)
        {
            _context = context;
        }

        public async Task<List<DiscountCode>> GenerateDiscountCodes(int count, int length)
        {
            var codes = new List<DiscountCode>();
            for (int i = 0; i < count; i++)
            {
                string code;
                do
                {
                    code = GenerateRandomCode(length);
                } while (await CodeExistsAsync(code));

                var discountCode = new DiscountCode { Code = code, IsUsed = false };
                codes.Add(discountCode);
                _context.DiscountCodes.Add(discountCode);
            }
            await _context.SaveChangesAsync();
            return codes;
        }

        public async Task<bool> UseDiscountCode(string code)
        {
            var discountCode = await _context.DiscountCodes.SingleOrDefaultAsync(dc => dc.Code == code);
            if (discountCode == null || discountCode.IsUsed)
                return false;

            discountCode.IsUsed = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.DiscountCodes.AnyAsync(dc => dc.Code == code);
        }

        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
