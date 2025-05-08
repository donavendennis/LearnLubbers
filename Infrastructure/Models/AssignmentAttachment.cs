using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
	public class AssignmentAttachment
	{
		[Key]
		public int AssignmentAttachmentId { get; set; }

		[Required]
		public int AssignmentId { get; set; }
		[ForeignKey("AssignmentId")]
		public Assignment Assignment { get; set; }

		[Required]
		public string? FileName { get; set; }

		[Required]
		public string? FileUrl { get; set; }

		[Required]
		public bool Keep { get; set; } = true;

		[Required]
		public bool KeepPerminant { get; set; } = false;
	}
}
