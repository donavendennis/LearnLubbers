using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Canvas_Like.Pages.Calendar
{
	public class IndexModel : PageModel
	{
		private readonly UnitOfWork _unitOfWork;
		public List<CalendarAccess> objCalendarAccesses;
		public List<Infrastructure.Models.Calendar> objectCalendars;
		public List<Infrastructure.Models.Event> objectEvents;
		public List<Infrastructure.Models.Event> recurringObjectEvents;
		public List<Infrastructure.Models.RecurringRule> recurringRules;
        public List<Assignment> Assignments { get; set; }

        public IndexModel(UnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			objCalendarAccesses = new List<CalendarAccess>();
			objectCalendars = new List<Infrastructure.Models.Calendar>();
			objectEvents = new List<Infrastructure.Models.Event>();
			recurringObjectEvents = new List<Infrastructure.Models.Event>();
			recurringRules = new List<Infrastructure.Models.RecurringRule>();
            Assignments = new List<Assignment>();

        }

        public void OnGet()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			objCalendarAccesses = _unitOfWork.CalendarUserRole.GetAll().Where(a => a.ApplicationUserId == userId).ToList();
			List<int> calendarIds = objCalendarAccesses.Select(
				(c) => c.CalendarId).ToList();
			objectCalendars = _unitOfWork.Calendar.GetAll().Where(c => calendarIds.Contains(c.CalendarId)).ToList();

			objectEvents = _unitOfWork.Event.GetAll().Where(
				(e) => calendarIds.Contains(e.CalendarId) && e.RecurringRuleId == null).ToList();
			recurringObjectEvents = _unitOfWork.Event.GetAll().Where(
				(e) => calendarIds.Contains(e.CalendarId) && e.RecurringRuleId != null).ToList();
			List<int?> recurringRuleIds = recurringObjectEvents.Select((r) => r.RecurringRuleId).ToList();
			recurringRules = _unitOfWork.RecurringRule.GetAll().Where(
				(r) => recurringRuleIds.Contains(r.RecurringRuleId)).ToList();
            Assignments = _unitOfWork.Assignment.GetAll().ToList();
        }
    }
}
