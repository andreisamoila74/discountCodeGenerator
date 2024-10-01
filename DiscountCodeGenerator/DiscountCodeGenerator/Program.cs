using Microsoft.EntityFrameworkCore;
using DiscountCodeGenerator.Infrastructure.Data;
using DiscountCodeGenerator.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using DiscountCodeGenerator.Infrastructure.Repositories;
using DiscountCodeGenerator.Application.Services;
using DiscountCodeGenerator.Server;

namespace DiscountCodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
            .AddDbContext<DiscountCodeDbContext>(options =>
                options.UseSqlite("Data Source=Data/discountcodes.db"))
            .AddScoped<IDiscountCodeRepository, DiscountCodeRepository>()
            .AddScoped<DiscountCodeService>()
            .BuildServiceProvider();

            var tcpHandler = new TcpHandler(serviceProvider.GetService<DiscountCodeService>());
            tcpHandler.StartListening();
        }
    }
}
