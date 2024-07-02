namespace Backend.Models
{
    public class AddblogDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
    }
}
