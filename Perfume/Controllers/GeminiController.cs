using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Perfume.Services;

namespace Perfume.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeminiController : ControllerBase
    {
        private readonly GeminiService _gemini;

        public GeminiController(GeminiService gemini)
        {
            _gemini = gemini;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt boş olamaz.");

            var answer = await _gemini.AskAsync(request.Prompt);
            return Ok(new { answer });
        }

        public record AskRequest(string Prompt);


    }
}
