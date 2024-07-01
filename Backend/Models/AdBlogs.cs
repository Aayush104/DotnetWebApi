using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class AdBlogs
    {
        public int Id { get; set; }

        [Required]
        public String Title { get; set; } 

        [Required]
        public String Description { get; set; }

        [Required]
        public String Image { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        // [ForeignKey("User")]
        public string UserId { get; set; }
        public Registration User { get; set; }



     
    }
}
