using System.ComponentModel.DataAnnotations;

namespace CLDV_POE.Models
{
    public class CustomerSql
    {
        [Key]
        public string CustomerId { get; set; }
        public string Customer_Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}


