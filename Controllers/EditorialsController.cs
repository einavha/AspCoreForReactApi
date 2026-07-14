using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace AspCoreForReactApi.Controllers
{
    public class Editorial
    {
        public string? Folder { get; set; }
        public string[]? Files { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? Id { get; set; }
        public string? Slug { get; set; }
        public string? MainImage { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class EditorialsController
    {
        [HttpGet]
        [Route("list")]
        public async Task<List<Editorial>> GetList()
        {
            try
            {
                var ImageFilePathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Editorials");
                var edListFilePath = Path.Combine(ImageFilePathBase, "list.json");
                var edListText = File.OpenRead(edListFilePath);
                //desrilized list
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var list = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Editorial>>(edListText, options);
                foreach (var item in list)
                {
                    item.MainImage = $"/api/editorials/image/{item.Slug}/{item.Files[0]}";
                }
                return list ?? new List<Editorial>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading editorial list: {ex.Message}");
                return new List<Editorial>();
            }
        }


        [HttpGet]
        [Route("{Id}")]
        public async Task<Editorial> Get(string Id)
        {
            var list = await GetList();
            var ed = list.FirstOrDefault(x => x.Slug == Id);
            return ed;
        }

        [HttpGet]
        [Route("image/{Slug}/{Name}")]
        public async Task<IActionResult> Image(string Slug, string Name)
        {
            var ImageFilePathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Editorials");
            var imagePath = Path.Combine(ImageFilePathBase, Slug, Name);
            if (System.IO.File.Exists(imagePath))
            {
                var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);

                return new FileContentResult(imageBytes, "image/png");
                //return File(imageBytes, "image/png");
                //return "data:image/png;base64, " + Convert.ToBase64String(imageBytes);
            }
            return null;
        }
    }
}
