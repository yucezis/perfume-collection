using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Perfume.Data; // Kendi DbContext'inin olduğu yer
using System.Linq;
using System.Data.Common;

namespace Perfume.Tests.IntegrationTests
{
    public class CustomWebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // 1. NÜKLEER SEÇENEK: İçinde "DbContextOptions" geçen TÜM ayarları bul ve yok et!
                var dbOptions = services.Where(d => d.ServiceType.Name.Contains("DbContextOptions")).ToList();
                foreach (var option in dbOptions)
                {
                    services.Remove(option);
                }

                // 2. Gizli kalan fiziksel SQL bağlantılarını da süpür
                var dbConnections = services.Where(d => d.ServiceType == typeof(DbConnection)).ToList();
                foreach (var conn in dbConnections)
                {
                    services.Remove(conn);
                }

                // 3. Artık içerisi tertemiz! InMemory veritabanımızı gönül rahatlığıyla kurabiliriz.
                services.AddDbContext<PerfumeDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            });

            builder.UseEnvironment("Testing");
        }
    }
}