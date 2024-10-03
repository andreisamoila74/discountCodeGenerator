using DiscountCodeGenerator.Application.Services;
using DiscountCodeGenerator.Domain.Entities;
using DiscountCodeGenerator.Domain.Repositories;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DiscountCodeGenerator.Tests.UnitTests
{
    public class DiscountCodeServiceTests
    {
        [Fact]
        public async Task GenerateDiscountCodesAsync_ShouldReturnGeneratedCodes_WhenCalled()
        {
            var mockRepository = new Mock<IDiscountCodeRepository>();
            var discountCodeService = new DiscountCodeService(mockRepository.Object);

            mockRepository.Setup(repo => repo.GenerateDiscountCodes(5, 8))
                          .ReturnsAsync(new List<DiscountCode>
                          {
                              new DiscountCode { Code = "CODE1" },
                              new DiscountCode { Code = "CODE2" },
                              new DiscountCode { Code = "CODE3" },
                              new DiscountCode { Code = "CODE4" },
                              new DiscountCode { Code = "CODE5" }
                          });

            var generatedCodes = await discountCodeService.GenerateDiscountCodesAsync(5, 8);

            Assert.Equal(5, generatedCodes.Count); 
            mockRepository.Verify(repo => repo.GenerateDiscountCodes(5, 8), Times.Once);
        }

        [Fact]
        public async Task UseDiscountCodeAsync_ShouldReturnTrue_WhenCodeIsValid()
        {
            var mockRepository = new Mock<IDiscountCodeRepository>();

            mockRepository.Setup(repo => repo.UseDiscountCode("VALIDCODE"))
                          .ReturnsAsync(true);

            var discountCodeService = new DiscountCodeService(mockRepository.Object);

            var result = await discountCodeService.UseDiscountCodeAsync("VALIDCODE");

            Assert.True(result);
            mockRepository.Verify(repo => repo.UseDiscountCode("VALIDCODE"), Times.Once); 
        }

        [Fact]
        public async Task UseDiscountCodeAsync_ShouldReturnFalse_WhenCodeIsInvalid()
        {
            var mockRepository = new Mock<IDiscountCodeRepository>();

            mockRepository.Setup(repo => repo.UseDiscountCode("INVALIDCODE"))
                          .ReturnsAsync(false); 

            var discountCodeService = new DiscountCodeService(mockRepository.Object);

            var result = await discountCodeService.UseDiscountCodeAsync("INVALIDCODE");

            Assert.False(result);
            mockRepository.Verify(repo => repo.UseDiscountCode("INVALIDCODE"), Times.Once);
        }
    }
}
