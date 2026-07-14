using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace AspCoreForReactApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ImagesController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ImageItem>>> Get()
        {
            var response = await LoadImagesAsync();
            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpGet("{editorialId}")]
        public async Task<ActionResult<IReadOnlyList<ImageItem>>> GetByEditorialId(string editorialId)
        {
            var response = await LoadImagesAsync();
            if (response is null)
            {
                return NotFound();
            }

            var decodedId = Uri.UnescapeDataString(editorialId);
            var filtered = response
                .Where(image => string.Equals(image.Url, decodedId, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            return Ok(filtered);
        }       

        private async Task<ImageItem[]?> LoadImagesAsync()
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "Assets", "imagesList.json");

            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var images = JsonSerializer.Deserialize<List<ImageItem>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (images is null)
            {
                return Array.Empty<ImageItem>();
            }

            foreach (var image in images)
            {
                var imagePath = Path.Combine(_environment.ContentRootPath, "Assets", "images", image.FileName);
                if (System.IO.File.Exists(imagePath))
                {
                    var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
                    image.Content = Convert.ToBase64String(imageBytes);
                }
            }

            return images
                .Select(image => new ImageItem
                {
                    Title = image.Title,
                    FileName = $"/assets/images/{image.FileName}",
                    Content = "data:image/png;base64, " + image.Content,
                    Url = image.Url
                })
                .ToArray();
        }
    }
}