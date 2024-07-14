using Backend.Models;
using Backend.Security;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
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
        private readonly IDataProtector _protector;

        public BlogController(AppDbContext dbContext, IWebHostEnvironment environment, ILogger<BlogController> logger, DataSecurityProvider security, IDataProtectionProvider provider)
        {
            _dbContext = dbContext;
            _environment = environment;
            _logger = logger;
            _protector = provider.CreateProtector(security.Datakey);
        }

        [HttpPost("AddBlog")]
        public IActionResult Create([FromForm] AddblogDTO addBlogDto)
        {
            var user = HttpContext.User.FindFirst("Id");
            var userId = user?.Value;

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
        [AllowAnonymous]
        public IActionResult GetBlog()
        {
            var user = HttpContext.User.FindFirst("Id");
            var userId = user?.Value;

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
                encId = _protector.Protect(blog.Id.ToString())
            }).ToList();

            return Ok(getBlogs);
        }

        [HttpGet("Description/{id}")]
        [AllowAnonymous]
        public IActionResult Description(string id)
        {

            int blogId = Convert.ToInt32(_protector.Unprotect(id));

            var blog = _dbContext.Blog.FirstOrDefault(b => b.Id == blogId);

            if (blog == null)
            {
                _logger.LogInformation("No blog found with ID {id}", id);
                return NotFound("No blogs found");
            }

            _logger.LogInformation("Blog found with ID {id}", id);

            var blogDto = new getBlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                Image = blog.Image,
                loginId = blog.UserId,
                encId = _protector.Protect(blog.Id.ToString())

            };

            return Ok(blogDto);
        }

        [HttpGet("Personal")]
        public IActionResult Personal()
        {
            var user = HttpContext.User.FindFirst("Id");
            var userId = user?.Value;

            _logger.LogInformation("Personal", userId);

            var blogs = _dbContext.Blog.Where(b => b.UserId == userId).ToList();
            if (blogs == null || blogs.Count == 0)
            {
                return NotFound("No Blogs Found");
            }

            var getBlogs = blogs.Select(blog => new getBlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                Image = blog.Image,
                encId = _protector.Protect(blog.Id.ToString())

            }).ToList();

            return Ok(getBlogs);
        }

        [HttpPost("Edit/{id}")]
        [AllowAnonymous]
        public IActionResult Edit(string id, [FromForm] AddblogDTO addBlogDto)

        {

            int blogId = Convert.ToInt32(_protector.Unprotect(id));
            var blog = _dbContext.Blog.Find(blogId);

            if (blog == null)
            {
                return BadRequest("No blog found with the given ID");
            }

            if (addBlogDto.Image != null)
            {
                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(addBlogDto.Image.FileName);

                string imageFullPath = Path.Combine(_environment.WebRootPath, "Images", newFileName);

                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    addBlogDto.Image.CopyTo(stream);
                }

                string imageUrl = $"{Request.Scheme}://{Request.Host}/Images/{newFileName}";

                string oldFilePath = Path.Combine(_environment.WebRootPath, "Images", Path.GetFileName(blog.Image));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                blog.Image = imageUrl;
            }

            blog.Title = addBlogDto.Title;
            blog.Description = addBlogDto.Description;

            _dbContext.Blog.Update(blog);
            _dbContext.SaveChanges();

            return Ok(blog);
        }


        [HttpDelete("Delete/{id}")]
        [AllowAnonymous]

        public IActionResult Delete(string id)
        {
            int blogId = Convert.ToInt32(_protector.Unprotect(id));

            if (blogId == 0)
            {
                return BadRequest("No Id");
            }

            var blog = _dbContext.Blog.Find(blogId);


           
            string imageFullPath = blog.Image;

            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }
            
            _dbContext.Blog.Remove(blog);
            _dbContext.SaveChanges(true);
            return Ok("deleted");

        }




        [HttpPost("Search/{query}")]
        [AllowAnonymous]

        public IActionResult Search(string query)
        {

            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Field is empty");
            }


            var Data = _dbContext.Blog.Where(b => b.Title.Contains(query)).ToList();

            if (!Data.Any())
            {
                return BadRequest("No resuolt found");
            }

            return Ok(Data);

        }
    }



}
