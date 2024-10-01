using DiscountCodeGenerator.Application.Services;

namespace DiscountCodeGenerator.Server
{
    public class TcpHandler
    {
        private readonly DiscountCodeService _discountCodeService;

        public TcpHandler(DiscountCodeService discountCodeService)
        {
            _discountCodeService = discountCodeService;
        }

        public void StartListening()
        {
            // TCP logic...
            // Process client requests using _discountCodeService.GenerateDiscountCodesAsync and UseDiscountCodeAsync
        }
    }
}
