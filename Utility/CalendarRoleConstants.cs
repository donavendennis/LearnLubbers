namespace Utility
{
	public class CalendarRoleConstants
	{
		public const string Owner = "Owner";
		public const string Editor = "Editor";
		public const string Viewer = "Viewer";

		public static bool IsValidCalendarRole(string role)
		{
			return role == Owner || role == Editor || role == Viewer;
		}
	}
}
