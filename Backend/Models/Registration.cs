using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Backend.Models
{
    public class Registration : IdentityUser
    {
        public DateTime CreatedOn{ get; set; }  = DateTime.UtcNow;
        
        public ICollection<AdBlogs> Blogs { get; set; }


       
    }
}
