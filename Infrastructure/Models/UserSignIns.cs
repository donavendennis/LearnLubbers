using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class UserSignIns
    {
        [Key]
        public int SignInId { get; set; }  // Primary key for UserSignIns

        [Required]
        public string UserId { get; set; }  // Foreign key for the ApplicationUser

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Required]
        public DateTime SignInTime { get; set; }  // The time when the user signed in
    }
}
