namespace Infrastructure.Models
{
	public class CalendarAccess
	{
		public ApplicationUser? ApplicationUser { get; set; }
		public string ApplicationUserId { get; set; }
		public Calendar? Calendar { get; set; }
		public int CalendarId { get; set; }
		public CalendarRole? CalendarRole { get; set; }
		public int CalendarRoleId { get; set; }
	}
}
