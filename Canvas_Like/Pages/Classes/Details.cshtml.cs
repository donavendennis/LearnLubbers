using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace Canvas_Like.Pages.Classes
{
	public class DetailsModel : PageModel
	{
		private readonly ApplicationDbContext _dbContext;

		public DetailsModel(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Class Class { get; set; }

		public async Task<IActionResult> OnGetAsync(int id)
		{
			Class = await _dbContext.Classes
				.Include(c => c.Department)
				.FirstOrDefaultAsync(m => m.ClassId == id);

			if (Class == null)
			{
				return NotFound();
			}

			ViewData["CalendarId"] = Class.CalendarId;

			var classEvent = await _dbContext.Events
				.Include(e => e.RecurringRule)
				.FirstOrDefaultAsync(e => e.CalendarId == Class.CalendarId);

			if (classEvent != null)
			{
				ViewData["MeetingTime"] = classEvent.Start.ToString("hh:mm tt") + " - " + classEvent.End.ToString("hh:mm tt");
				ViewData["Days"] = new WeekDayBitMapping(classEvent.RecurringRule.WeekdayBitMap).WeekDayString();
			}

			return Page();
		}

	}
}
