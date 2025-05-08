using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
	public class StudentRegistration
	{
		[Key]
		public int RegistrationId { get; set; }

		// Foreign key for ApplicationUser (Student)
		[Required]
		public string StudentId { get; set; }
		[ForeignKey(nameof(StudentId))]
		public ApplicationUser Student { get; set; }

		// Foreign key for Class
		[Required]
		public int ClassId { get; set; }
		[ForeignKey(nameof(ClassId))]
		public Class Class { get; set; }

		public DateTime RegistrationDate { get; set; }
	}
}
