using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class getBlogDto
    {
        public int  Id { get; set; }
        public String Title { get; set; }

 
        public String Description { get; set; }

        
        public String Image { get; set; }
    }
}
