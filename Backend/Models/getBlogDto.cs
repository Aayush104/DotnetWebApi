﻿using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class getBlogDto
    {
        public int Id { get; set; }
        public string Title { get; set; }


        public string Description { get; set; }

        public string BlogStatus { get; set; }  
        public decimal Amount { get; set; }
        public string Image { get; set; }
        public string loginId {get; set;}

        public string encId { get; set;}    

    }
}
