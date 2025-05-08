using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models
{
	public class Department
	{
		[Key]
		public int DepartmentId { get; set; }

		[Required, StringLength(100), DisplayName("Department")]
		public string? Name { get; set; }

		[Required, StringLength(4), DisplayName("Department Code")]
		public string? Acronym { get; set; }
	}
}
