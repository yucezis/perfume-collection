using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Perfume.Tests.IntegrationTests
{
    public class BrandControllerTests : IClassFixture<CustomWebAppFactory>
    {
        private readonly HttpClient _client;

        public BrandControllerTests(CustomWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllBrands_WhenCalled_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/Brand");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}