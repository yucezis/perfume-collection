using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Perfume.Models; 
using Perfume.Services;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace Perfume.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly TokenService _sut;

        public TokenServiceTests()
        {
            _configMock = new Mock<IConfiguration>();

            _configMock.Setup(x => x["Jwt:Key"]).Returns("BuCokGizliVeEnAz32KarakterUzunlugundaBirTestAnahtaridir!!");
            _configMock.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            _configMock.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");

            _sut = new TokenService(_configMock.Object);
        }

        [Fact]
        public void CreateToken_ValidUserAndRoles_ReturnsJwtString()
        {
            var user = new AppUser { Id = "test-id-123", Email = "doga@test.com", Name = "Doğa" };
            var roles = new List<string> { "Admin", "User" };

            var token = _sut.CreateToken(user, roles);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();

            token.Split('.').Length.Should().Be(3);
        }

        [Fact]
        public void CreateToken_ValidUser_ContainsCorrectClaims()
        {
            var user = new AppUser { Id = "test-id-123", Email = "doga@test.com", Name = "Doğa" };
            var roles = new List<string> { "Admin" };

            var tokenString = _sut.CreateToken(user, roles);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);

            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "doga@test.com");
            jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Admin");
            jwtToken.Issuer.Should().Be("TestIssuer"); 
            jwtToken.Audiences.Should().Contain("TestAudience");
        }
    }
}
