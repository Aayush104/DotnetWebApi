using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Registration : IdentityUser
    {
        public DateTime CreatedOn{ get; set; }  = DateTime.UtcNow;
    }
}
