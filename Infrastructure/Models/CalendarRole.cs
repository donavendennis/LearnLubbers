using System.ComponentModel.DataAnnotations;
using Utility;

namespace Infrastructure.Models
{
	public class CalendarRole
	{
		[Key]
		public int CalendarRoleId { get; set; }

		private string _role;
		[Required, StringLength(20)]
		public string Role
		{
			get => _role; set
			{
				if (!CalendarRoleConstants.IsValidCalendarRole(value))
				{
					throw new ArgumentException("Invalid Calendar Role");
				}
				_role = value;
			}
		}
	}
}
