using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Payment
    {
        [Key]
        public int ID { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public string UserId { get; set; }

        public int BlogId { get; set; }

        // Navigation Properties
        public  Registration User { get; set; }
        public  AdBlogs Blog { get; set; }
    }
}
