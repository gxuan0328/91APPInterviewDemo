using System;
using System.ComponentModel.DataAnnotations;

namespace ManagementCenter.Models
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public int Price { get; set; }
        public DateTime CreateTime { get; set; } 
    }
}