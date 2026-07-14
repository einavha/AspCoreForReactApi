using Microsoft.AspNetCore.Mvc;

namespace AspCoreForReactApi.Controllers
{
    [ApiController]
    [Route("wp-content")]
    public class WpContentController : ControllerBase
    {
        private const string DefaultWordPressBaseUrl = "https://hebrewwordpresss-e8gxb2fjbmhwcsar.israelcentral-01.azurewebsites.net";

        private readonly HttpClient _httpClient;
        private readonly string _wordPressBaseUrl;

        public WpContentController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _wordPressBaseUrl = configuration["WordPress:BaseUrl"] ?? DefaultWordPressBaseUrl;
        }

        [HttpGet("{**path}")]
        public async Task<IActionResult> Get(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.Contains(".."))
            {
                return BadRequest();
            }

            var requestUrl = $"{_wordPressBaseUrl.TrimEnd('/')}/wp-content/{EscapePath(path)}{Request.QueryString}";

            try
            {
                using var response = await _httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode);
                }

                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                var content = await response.Content.ReadAsByteArrayAsync();

                return File(content, contentType);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error reading WordPress content: {ex.Message}");
                return StatusCode(StatusCodes.Status502BadGateway);
            }
        }

        private static string EscapePath(string path)
        {
            return string.Join('/', path.Split('/').Select(Uri.EscapeDataString));
        }
    }
}
