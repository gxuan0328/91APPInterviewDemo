using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ManagementCenter.Models
{
    public class OrderOverviewDto
    {
        public int Id { get; set; }
        public string Buyer { get; set; }
        public int Total { get; set; }
        public string PaymentType { get; set; }
        public string Address { get; set; }
        public DateTime CreateTime { get; set; }
        public int OrderStatusType_Id { get; set; }
        public string OrderStatusType { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }
    }

    public class OrderDetailDto
    {
        public int Id { get; set; }
        public string Buyer { get; set; }
        public string Seller { get; set; }
        public List<Product> Products { get; set; }
        public string PaymentType { get; set; }
        public string Address { get; set; }
        public DateTime CreateTime { get; set; }
        public string OrderStatusType { get; set; }
        public int Total { get; set; }
        public int Cost { get; set; }
    }

    public class PutOrderDto
    {
        [Required]
        public int[] Ids { get; set; }
    }
}