using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
	public class Event
	{
		[Key]
		public int EventId { get; set; }

		[Required]
		[Display(Name = "Calendar")]
		public int CalendarId { get; set; }

		[Required]
		public DateTime Start { get; set; }
		[Required]
		public DateTime End { get; set; }
		[Required, StringLength(100)]
		public string? Title { get; set; }
		[Required, StringLength(1000)]
		public string? Description { get; set; }
		public String? Location { get; set; }

		[Required]
		[Display(Name = "Creator")]
		public string? CreatorId { get; set; }

		[Display(Name = "Recurring Rule")]
		public int? RecurringRuleId { get; set; }


		[Required]
		public bool CanDelete { get; set; }

		[ForeignKey("CalendarId")]
		public Calendar? Calendar { get; set; }

		[ForeignKey("CreatorId")]
		public ApplicationUser? Creator { get; set; }

		[ForeignKey("RecurringRuleId")]
		public RecurringRule? RecurringRule { get; set; }
	}
}
