using Microsoft.EntityFrameworkCore;
using DiscountCodeGenerator.Infrastructure.Data;
using DiscountCodeGenerator.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using DiscountCodeGenerator.Infrastructure.Repositories;
using DiscountCodeGenerator.Application.Services;

namespace DiscountCodeGenerator.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<DiscountCodeDbContext>(options =>
                    options.UseSqlite("Data Source=../../../../DiscountCodeGenerator.Infrastructure/Data/db.sqlite"))
                .AddScoped<IDiscountCodeRepository, DiscountCodeRepository>()
                .AddScoped<DiscountCodeService>()
                .BuildServiceProvider();


            var discountCodeService = serviceProvider.GetService<DiscountCodeService>();

            if (discountCodeService == null)
            {
                Console.WriteLine("Failed to retrieve DiscountCodeService from the service provider.");
                return;
            }

            var tcpHandler = new TcpHandler(discountCodeService);
            await tcpHandler.StartListeningAsync();
        }
    }
}