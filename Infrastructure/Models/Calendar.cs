using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace Infrastructure.Models
{
	public class Calendar
	{
		[Key]
		public int CalendarId { get; set; }
		[Required, StringLength(100)]
		public string? CalendarName { get; set; }

		private string? _color;
		public string? Color
		{
			get => _color;
			set
			{
				if (!IsValidHexColor(value))
				{
					throw new ArgumentException("Invalid hex color code.");
				}
				_color = value;
			}
		}

		private bool IsValidHexColor(string hex)
		{
			// Regex to check for valid hex color codes
			return Regex.IsMatch(hex, "^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$");
		}
	}
}
