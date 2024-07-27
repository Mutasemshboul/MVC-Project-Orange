using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVC_Project_Orange.Models
{
    public class Testimonial
    {
        [Key]
        public int TestimonialId { get; set; }

        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; }

        public bool IsDeleted { get; set; }
    }
}
