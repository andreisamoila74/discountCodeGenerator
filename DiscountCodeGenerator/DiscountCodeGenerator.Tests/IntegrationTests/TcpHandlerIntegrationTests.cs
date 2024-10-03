using DiscountCodeGenerator.Application.Services;
using DiscountCodeGenerator.Client;
using DiscountCodeGenerator.Domain.Repositories;
using DiscountCodeGenerator.Infrastructure.Data;
using DiscountCodeGenerator.Infrastructure.Repositories;
using DiscountCodeGenerator.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;
using System.Text.Json;

namespace DiscountCodeGenerator.Tests.IntegrationTests
{
    public class TcpHandlerIntegrationTests
    {
        private const int Port = 5000;

        private DiscountCodeDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DiscountCodeDbContext>()
                .UseInMemoryDatabase(databaseName: "DiscountCodeDb")
                .Options;

            return new DiscountCodeDbContext(options);
        }

        [Fact]
        public async Task TcpHandler_ShouldGenerateDiscountCodes_WhenGenerateRequestIsSent()
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<DiscountCodeDbContext>(options => options.UseInMemoryDatabase("DiscountCodeDb"))
                .AddScoped<IDiscountCodeRepository, DiscountCodeRepository>()
                .AddScoped<DiscountCodeService>()
                .BuildServiceProvider();

            var discountCodeService = serviceProvider.GetService<DiscountCodeService>();
            var tcpHandler = new TcpHandler(discountCodeService);

            var generateRequest = new GenerateRequest { Count = 5, Length = 8 };
            var requestData = JsonSerializer.Serialize(generateRequest);

            // Start server
            var listenerTask = tcpHandler.StartListeningAsync();

            using (var client = new TcpClient("localhost", 5000))
            using (var networkStream = client.GetStream())
            using (var writer = new StreamWriter(networkStream) { AutoFlush = true })
            using (var reader = new StreamReader(networkStream))
            {
                await writer.WriteLineAsync(requestData);

                var responseData = await reader.ReadLineAsync();
                var response = JsonSerializer.Deserialize<GenerateResponse>(responseData);

                Assert.True(response.Result);
            }

            // Wait for server to finish handling the request
            await Task.Delay(500); // Small delay to allow the server to finish processing
        }

        [Fact]
        public async Task TcpHandler_ShouldUseDiscountCode_WhenUseCodeRequestIsSent()
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<DiscountCodeDbContext>(options => options.UseInMemoryDatabase("DiscountCodeDb"))
                .AddScoped<IDiscountCodeRepository, DiscountCodeRepository>()
                .AddScoped<DiscountCodeService>()
                .BuildServiceProvider();

            var discountCodeService = serviceProvider.GetService<DiscountCodeService>();
            var tcpHandler = new TcpHandler(discountCodeService);

            // Generate a discount code so it can be used
            var generateRequest = new GenerateRequest { Count = 1, Length = 8 };
            var generatedCodes = await discountCodeService.GenerateDiscountCodesAsync(1, 8);
            var generatedCode = generatedCodes.First();  // Assuming you get at least one code

            var useCodeRequest = new UseCodeRequest { Code = generatedCode.Code };  // Use the generated code
            var requestData = JsonSerializer.Serialize(useCodeRequest);

            // Start server
            var listenerTask = tcpHandler.StartListeningAsync();

            using (var client = new TcpClient("localhost", 5000))
            using (var networkStream = client.GetStream())
            using (var writer = new StreamWriter(networkStream) { AutoFlush = true })
            using (var reader = new StreamReader(networkStream))
            {
                await writer.WriteLineAsync(requestData);

                var responseData = await reader.ReadLineAsync();
                var response = JsonSerializer.Deserialize<UseCodeResponse>(responseData);

                Assert.True(response.Success);
            }

            await Task.Delay(500);
        }

        [Fact]
        public async Task TcpHandler_ShouldReturnError_WhenInvalidRequestIsSent()
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<DiscountCodeDbContext>(options => options.UseInMemoryDatabase("DiscountCodeDb"))
                .AddScoped<IDiscountCodeRepository, DiscountCodeRepository>()
                .AddScoped<DiscountCodeService>()
                .BuildServiceProvider();

            var discountCodeService = serviceProvider.GetService<DiscountCodeService>();
            var tcpHandler = new TcpHandler(discountCodeService);

            var invalidRequest = "{ \"Invalid\": \"data\" }";

            var listenerTask = tcpHandler.StartListeningAsync();

            using (var client = new TcpClient("localhost", 5000))
            using (var networkStream = client.GetStream())
            using (var writer = new StreamWriter(networkStream) { AutoFlush = true })
            using (var reader = new StreamReader(networkStream))
            {
                await writer.WriteLineAsync(invalidRequest);

                var responseData = await reader.ReadLineAsync();
                var response = JsonSerializer.Deserialize<UseCodeResponse>(responseData);

                Assert.False(response.Success);
            }

            await Task.Delay(500);
        }
    }
}
