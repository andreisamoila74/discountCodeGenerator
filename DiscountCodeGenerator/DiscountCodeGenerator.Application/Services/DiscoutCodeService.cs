using DiscountCodeGenerator.Domain.Entities;
using DiscountCodeGenerator.Domain.Repositories;

namespace DiscountCodeGenerator.Application.Services
{
    public class DiscountCodeService
    {
        private readonly IDiscountCodeRepository _repository;

        public DiscountCodeService(IDiscountCodeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DiscountCode>> GenerateDiscountCodesAsync(int count, int length)
        {
            return await _repository.GenerateDiscountCodes(count, length);
        }

        public async Task<bool> UseDiscountCodeAsync(string code)
        {
            return await _repository.UseDiscountCode(code);
        }
    }
}
