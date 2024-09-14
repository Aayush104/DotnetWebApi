using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Backend.Models
{
    public class Registration : IdentityUser
    {
        public DateTime CreatedOn{ get; set; }  = DateTime.UtcNow;
        public string? Otp { get; set; } 

        public string validity { get; set; }
        
        public ICollection<AdBlogs> Blogs { get; set; }
        public ICollection<Payment> BlogSubscriptions { get; set; }


       
    }
}
