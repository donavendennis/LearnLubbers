using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models
{
	public class Location
	{
		[Key]
		public int LocationID { get; set; } 

		[StringLength(100)]
		public string Alias { get; set; }

		[StringLength(100)]
		public string? Building { get; set; }
		[StringLength(100)]
		public string? RoomNumber { get; set; }

		[StringLength(200), DisplayName("Street Line 1")]
		public string? Street1 { get; set; }

		[StringLength(200), DisplayName("Street Line 2")]
		public string? Street2 { get; set; }

		[StringLength(100)]
		public string? City { get; set; }

		[StringLength(50)]
		public string? State { get; set; }

		[StringLength(10), DisplayName("")]
		public string? PostalCode { get; set; }

	}
}
