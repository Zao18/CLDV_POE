using System;
using System.ComponentModel.DataAnnotations;

namespace CLDV_POE.Models
{
    public class OrderSql
    {
        [Key]
        public string? Order_Id { get; set; }
        public string? Customer_Id { get; set; }
        public string? Product_Id { get; set; }
        public DateTime Order_Date { get; set; }
    }
}

