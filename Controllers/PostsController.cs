using Microsoft.AspNetCore.Mvc;

namespace ReactApp1.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public PostsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet]
        public async Task<List<Post>> List()
        {
            var path = Path.Combine(_environment.ContentRootPath, "Data", "postsList.json");
            var fl = System.IO.File.OpenRead(path);
            var content = await new StreamReader(fl).ReadToEndAsync();
            fl.Close();
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var posts = System.Text.Json.JsonSerializer.Deserialize<List<Post>>(content, options);
            var x = 0;
            var y = 0;
            /*
            foreach (var post in posts ?? new List<Post>())
            {
                post.Top = y;
                post.Left = x;

                if (x + post.Width > screenWidth)
                {
                    x = 0;
                    y += post.Height;
                }
                else
                {
                    x += post.Width;
                }
            }
            */
            return posts ?? new List<Post>();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> Get(int id)
        {
            var posts = await List();
            var post = posts.FirstOrDefault(item => item.Id == id);
            if (post is null)
            {
                return NotFound();
            }

            return Ok(post);
        }        
    }
}