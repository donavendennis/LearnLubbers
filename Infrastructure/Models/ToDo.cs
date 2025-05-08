using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
	public class ToDo
	{
		[Key]
		public int ToDoId { get; set; }
		public Calendar? Calendar { get; set; }
		[Required, ForeignKey(nameof(Calendar))] public int? CalendarId { get; set; }
		[Required]
		public bool? Completed { get; set; }
		[Required]
		public DateTime DueDate { get; set; }
		[Required, StringLength(100)]
		public string? Title { get; set; }
		[Required, StringLength(1000)]
		public string? Description { get; set; }
	}
}
