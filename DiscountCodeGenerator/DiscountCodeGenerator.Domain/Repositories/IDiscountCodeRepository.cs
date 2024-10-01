using DiscountCodeGenerator.Domain.Entities;

namespace DiscountCodeGenerator.Domain.Repositories
{
    public interface IDiscountCodeRepository
    {
        Task<List<DiscountCode>> GenerateDiscountCodes(int count, int length);
        Task<bool> UseDiscountCode(string code);
        Task<bool> CodeExistsAsync(string code);
    }
}
