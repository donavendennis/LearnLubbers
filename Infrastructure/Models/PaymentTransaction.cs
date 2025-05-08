using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    // This is just a very basic transaction table, please feel free to alter as
    // seen fit.
    public class PaymentTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        // foreign key generation
        [Required]
        public string StudentId { get; set; }
        [ForeignKey(nameof (StudentId))]
        public ApplicationUser Student { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TransactionDate { get; set; }


    }
}
