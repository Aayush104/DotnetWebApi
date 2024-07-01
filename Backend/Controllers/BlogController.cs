using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<BlogController> _logger;

        public BlogController(AppDbContext dbContext, IWebHostEnvironment environment, ILogger<BlogController> logger, IConfiguration configurat)
        {
            _dbContext = dbContext;
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("AddBlog")]
        public IActionResult Create([FromForm] AddblogDTO addBlogDto)

        {

            var user = HttpContext.User.FindFirst("Id");
            var userId = user?.Value;

            // Log the user ID
            _logger.LogInformation("User ID fpr add blog {userId}  yess", userId);


            if (addBlogDto.Image == null)
            {
                return BadRequest("Need an image file");
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(addBlogDto.Image.FileName);

            string imageFullPath = Path.Combine(_environment.WebRootPath, "Images", newFileName);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                addBlogDto.Image.CopyTo(stream);
            }

            string imageUrl = $"{Request.Scheme}://{Request.Host}/Images/{newFileName}";

            AdBlogs adBlogs = new AdBlogs
            {
                Title = addBlogDto.Title,
                Description = addBlogDto.Description,
                Image = imageUrl,
                UserId = userId
            };

            _dbContext.Blog.Add(adBlogs);
            _dbContext.SaveChanges();

            return Ok("Blog created successfully");
        }

        [HttpGet("GetBlog")]

        public IActionResult GetBlog()
        {
            var user = HttpContext.User.FindFirst("Id");
            var userId = user?.Value;

            // Log the user ID
            _logger.LogInformation("User ID: from this {userId}  yess", userId);

            if (user == null)
            {
                return NotFound("No User Found");
            }
            
            var blogs = _dbContext.Blog.Where(blog => blog.UserId != userId).ToList();

            if (blogs == null || blogs.Count == 0)
            {
                return NotFound("No blogs found");
            }

            var getBlogs = blogs.Select(blog => new getBlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                Image = blog.Image,

            }).ToList();


            return Ok(getBlogs);
        }

        [HttpGet("Description/{id}")]
        public IActionResult Description(int id)
        {

            var blog = _dbContext.Blog.FirstOrDefault(b => b.Id == id);

            if (blog == null)
            {
                return NotFound("No blogs found");
            }

            var blogDto = new getBlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                Image = blog.Image,
            };

            return Ok(blogDto);
        }
    }


}
