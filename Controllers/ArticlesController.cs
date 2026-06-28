using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReactApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ArticlesController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public class ArticleInfo
        {
            public string? Content { get; set; }
        }

        private async Task<List<ArticleItem>> GetArticlesAsync()
        {
            try
            {
                var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var ArticlesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "articles", "articles.json");
                var ArticleFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "articles");

                var articlesJson = await System.IO.File.ReadAllTextAsync(ArticlesFilePath);
                var articles = JsonSerializer.Deserialize<List<ArticleItem>>(articlesJson, options);
                var images = await System.IO.File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "imagesList.json"));
                var imagesList = JsonSerializer.Deserialize<List<ImageItem>>(images, options);
                var i = 0;
                foreach (var article in articles)
                {
                    var imagePath = Path.Combine(_environment.ContentRootPath, "Assets", "images", imagesList[i++].FileName);
                    if (System.IO.File.Exists(imagePath))
                    {
                        var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
                        article.FeaturedImage = Convert.ToBase64String(imageBytes);
                    }
                }
                return articles ?? new List<ArticleItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading articles: {ex.Message}");
                return new List<ArticleItem>();
            }
        }


        // GET: api/<ArticlesController>
        [HttpGet]
        public async Task<List<ArticleItem>> Get()
        {
            try
            {
                // read the articles from the JSON file and return them as a list of ArticleItem objects
                var articles = await GetArticlesAsync();
                return articles;
            }
            catch (Exception ex)
            {
                // handle exceptions (e.g., log the error, return an empty list, etc.)
                Console.WriteLine($"Error reading articles: {ex.Message}");
                return new List<ArticleItem>();
            }
        }

        // GET api/<ArticlesController>/5
        [HttpGet("{id}")]
        public async Task<ContentResult> Get(int id)
        {
            // get article by id from the JSON file and return it as a string
            var articles = await GetArticlesAsync();
            var article = articles?.FirstOrDefault(a => a.Id == id);
            if (!string.IsNullOrWhiteSpace(article.Content))
            {
                var ArticlesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "articles", "articles.json");
                var ArticleFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "articles");

                var contentFilePath = Path.Combine(ArticleFilePath, article.Content);
                var content = System.IO.File.ReadAllText(contentFilePath, Encoding.Unicode);
                //var contentBase64 = Convert.ToBase64(Encoding.Unicode.GetBytes(content));                
                return Content(content, "text/html", Encoding.Unicode);
            }

            return Content("", "text/html", Encoding.Unicode);
        }

        // POST api/<ArticlesController>
        [HttpPost]
        public async Task Post([FromBody] ArticleItem value)
        {
            // post new articele to the JSON file
            var articles = await GetArticlesAsync();
            value.Id = articles.Count > 0 ? articles.Max(a => a.Id) + 1 : 1; // assign a new ID
            articles.Add(value);
            var ArticlesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "articles", "articles.json");
            await System.IO.File.WriteAllTextAsync(ArticlesFilePath, JsonSerializer.Serialize(articles));
        }

        // PUT api/<ArticlesController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] ArticleItem value)
        {
            // update article by id in the JSON file
            var articles = await GetArticlesAsync();
            var articleIndex = articles.FindIndex(a => a.Id == id);
            if (articleIndex != -1)
            {
                articles[articleIndex] = value; // update the article
                var ArticlesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "articles", "articles.json");
                await System.IO.File.WriteAllTextAsync(ArticlesFilePath, JsonSerializer.Serialize(articles));
            }
            else
            {
                // article not found, you might want to handle this case (e.g., return a 404 response)
                // add new article if it doesn't exist
                value.Id = id; // assign the provided ID
                articles.Add(value);
                var ArticlesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "articles", "articles.json");
                await System.IO.File.WriteAllTextAsync(ArticlesFilePath, JsonSerializer.Serialize(articles));
            }
        }

        // DELETE api/<ArticlesController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            // delete article by id from the JSON file
            var articles = await GetArticlesAsync();
            var articleIndex = articles.FindIndex(a => a.Id == id);
            if (articleIndex != -1)
            {
                articles.RemoveAt(articleIndex); // remove the article
                var ArticlesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "articles", "articles.json");
                await System.IO.File.WriteAllTextAsync(ArticlesFilePath, JsonSerializer.Serialize(articles));
            }
            else
            {
                // article not found, you might want to handle this case (e.g., return a 404 response)
            }
        }
    }
}
