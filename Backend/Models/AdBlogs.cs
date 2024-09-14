using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class AdBlogs
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } 

        [Required]
        public string Description { get; set; }

        [Required]
        public string Image { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string BlogStatus { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public Registration User { get; set; }
        public ICollection<Payment> BlogSubscriptions { get; set; }




    }
}
