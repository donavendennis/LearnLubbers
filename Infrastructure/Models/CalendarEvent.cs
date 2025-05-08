using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
	/// <summary>
	/// This class is going to be replaced by Calendar, Event, RecurringRule, and ToDo
	/// !!!!!!Do not use for any future implementation!!!!!!
	/// </summary>
	public class CalendarEvent
	{
		[Key]
		public int Id { get; set; }

		[Required, StringLength(200)]
		public string? Title { get; set; }
		[StringLength(300)]
		public string? Description { get; set; }

		[Required]
		public DateTime StartDate { get; set; }
		[Required]
		public DateTime EndDate { get; set; }


		public ApplicationUser? ApplicationUser { get; set; }
		[ForeignKey(nameof(ApplicationUser))] public string? UserId { get; set; }

	}
}
