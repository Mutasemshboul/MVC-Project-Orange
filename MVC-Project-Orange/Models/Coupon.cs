using System.ComponentModel.DataAnnotations;

namespace MVC_Project_Orange.Models
{
    public class Coupon
    {
        [Key]
        public int CouponID { get; set; }

        [Required, MaxLength(20)]
        public string Code { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string Status { get; set; }
    }
}
