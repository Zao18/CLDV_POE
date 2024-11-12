using System.ComponentModel.DataAnnotations;

namespace CLDV_POE.Models
{
    public class ProductSql
    {
        [Key]
        public string ProductId { get; set; }
        public string Product_Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Stock_Level { get; set; }
        public string ImageUrl { get; set; }
    }
}

