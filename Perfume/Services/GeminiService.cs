using System.Text;
using System.Text.Json;

namespace Perfume.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        private const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        public GeminiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["GeminiApiKey"]!;
        }

        public async Task<string> AskAsync(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    system_instruction = new
                    {
                        parts = new[]
                        {
                    new { text = "Sen uzman ve zarif bir parfümörsün (koku danışmanı)." +
                                 "Kullanıcılara parfüm koleksiyonları, koku notaları (odunsu, çiçeksi, baharatlı vb.) " +
                                 "ve tarzlarına uygun parfümler hakkında profesyonel, şık ve kısa cevaplar ver." +
                                 "Sadece parfüm, kozmetik ve koku dünyası hakkında konuş. " +
                                 "İlgisiz (matematik, yazılım, tarih vb.) soruları kibarca reddet."
                        }
                    }
                    },
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = $"{BASE_URL}?key={_apiKey}";

                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);

                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? "Yanıt alınamadı.";
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                return "Çok fazla istek gönderildi. Lütfen biraz bekleyin.";
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return "API anahtarı geçersiz. Lütfen kontrol edin.";
            }
            catch (Exception ex)
            {
                return $"Hata oluştu: {ex.Message}";
            }
        }
    }
}
