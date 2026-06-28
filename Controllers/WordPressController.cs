using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace ReactApp1.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordPressController : ControllerBase
    {
        private const string DefaultWordPressBaseUrl = "https://hebrewwordpresss-e8gxb2fjbmhwcsar.israelcentral-01.azurewebsites.net";

        private readonly HttpClient _httpClient;
        private readonly string _wordPressBaseUrl;

        public WordPressController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _wordPressBaseUrl = configuration["WordPress:BaseUrl"] ?? DefaultWordPressBaseUrl;
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<List<Editorial>>> GetList()
        {
            try
            {
                var posts = await GetWordPressPosts();
                var list = posts.Select(ToEditorial).ToList();

                return Ok(list);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error reading WordPress posts: {ex.Message}");
                return StatusCode(StatusCodes.Status502BadGateway, new List<Editorial>());
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing WordPress posts: {ex.Message}");
                return StatusCode(StatusCodes.Status502BadGateway, new List<Editorial>());
            }
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<Editorial>> Get(string slug)
        {
            var posts = await GetWordPressPosts(slug);
            var post = posts.Select(ToEditorial).FirstOrDefault();

            if (post is null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpGet("/wp-content/{**path}")]
        public async Task<IActionResult> GetImage(string path)
        {
            var requestUrl = $"{_wordPressBaseUrl.TrimEnd('/')}/wp-content/{path}{Request.QueryString}";

            try
            {
                using var response = await _httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode);
                }

                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                var image = await response.Content.ReadAsByteArrayAsync();

                return File(image, contentType);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error reading WordPress image: {ex.Message}");
                return StatusCode(StatusCodes.Status502BadGateway);
            }
        }

        private async Task<List<JsonElement>> GetWordPressPosts(string? slug = null)
        {
            var query = "rest_route=/wp/v2/posts&per_page=100&_embed=1";
            if (!string.IsNullOrWhiteSpace(slug))
            {
                query += $"&slug={Uri.EscapeDataString(slug)}";
            }

            var requestUrl = $"{_wordPressBaseUrl.TrimEnd('/')}/index.php?{query}";
            using var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);

            return document.RootElement.EnumerateArray().Select(item => item.Clone()).ToList();
        }

        private static Editorial ToEditorial(JsonElement post)
        {
            var slug = GetString(post, "slug") ?? "";
            var title = StripHtml(GetNestedRendered(post, "title"));
            var description = StripHtml(GetNestedRendered(post, "excerpt"));
            var content = GetNestedRendered(post, "content");
            var mainImage = GetFeaturedImage(post);

            return new Editorial
            {
                Folder = slug,
                Slug = slug,
                Id = $"wp-post-{GetInt(post, "id")}",
                Title = title,
                Description = description,
                Content = content,
                Files = string.IsNullOrWhiteSpace(mainImage) ? Array.Empty<string>() : new[] { mainImage },
                MainImage = mainImage
            };
        }

        private static string? GetFeaturedImage(JsonElement post)
        {
            if (!post.TryGetProperty("_embedded", out var embedded) ||
                !embedded.TryGetProperty("wp:featuredmedia", out var media) ||
                media.ValueKind != JsonValueKind.Array ||
                media.GetArrayLength() == 0)
            {
                return null;
            }

            var firstMedia = media[0];
            return GetString(firstMedia, "source_url");
        }

        private static string? GetNestedRendered(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property) ||
                !property.TryGetProperty("rendered", out var rendered) ||
                rendered.ValueKind != JsonValueKind.String)
            {
                return null;
            }

            return rendered.GetString();
        }

        private static string? GetString(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property) ||
                property.ValueKind != JsonValueKind.String)
            {
                return null;
            }

            return property.GetString();
        }

        private static int GetInt(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property) ||
                property.ValueKind != JsonValueKind.Number ||
                !property.TryGetInt32(out var value))
            {
                return 0;
            }

            return value;
        }

        private static string? StripHtml(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var withoutTags = Regex.Replace(value, "<.*?>", string.Empty);
            return WebUtility.HtmlDecode(withoutTags).Trim();
        }
    }
}
