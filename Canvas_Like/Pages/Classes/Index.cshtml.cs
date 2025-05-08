using DataAccess;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utility;

namespace Canvas_Like.Pages.Classes
{
	public struct ClassViewModel
	{
		public int? ClassId;
		public string? Department;
		public int? CourseNumber;
		public string? Building;
		public string? RoomNumber;
		public string? Days;
		public string? MeetingTime;
	}

	public class IndexModel : PageModel
	{
		private readonly IUnitOfWork _unitOfWork;
		public List<ClassViewModel> ClassViewModels;
		public IEnumerable<ClassDetailsDto> ClassDetails { get; set; }
		private readonly ApplicationDbContext _dbContext;

		public IndexModel(ApplicationDbContext dbContext)
		{
			_unitOfWork = new UnitOfWork(dbContext);
			_dbContext = dbContext;
			ClassViewModels = new List<ClassViewModel>();
		}

		public async Task OnGetAsync()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var query = from c in _dbContext.Classes
						join d in _dbContext.Departments on c.DepartmentId equals d.DepartmentId
						join e in _dbContext.Events on c.CalendarId equals e.CalendarId
						join r in _dbContext.RecurringRules on e.RecurringRuleId equals r.RecurringRuleId
						where c.InstructorId == userId
						select new ClassDetailsDto
						{
							ClassId = c.ClassId,
							Acronym = d.Acronym,
							CourseNumber = c.CourseNumber,
							Building = c.Building,
							RoomNumber = c.RoomNumber,
							WeekdayBitMap = r.WeekdayBitMap,
							Start = e.Start,
							End = e.End
						};
			ClassDetails = await query.ToListAsync();
			foreach (var objClass in ClassDetails)
			{
				ClassViewModel objClassViewModel = new ClassViewModel();
				WeekDayBitMapping weekString = new WeekDayBitMapping(objClass.WeekdayBitMap);
				objClassViewModel.ClassId = objClass.ClassId;
				objClassViewModel.Department = objClass.Acronym;
				objClassViewModel.CourseNumber = objClass.CourseNumber;
				objClassViewModel.Building = objClass.Building;
				objClassViewModel.RoomNumber = objClass.RoomNumber;
				objClassViewModel.Days = weekString.WeekDayString();
				objClassViewModel.MeetingTime = objClass.Start.ToString("hh:mm tt") + " - " + objClass.End.ToString("hh:mm tt");
				ClassViewModels.Add(objClassViewModel);
			}
		}
	}

	public class ClassDetailsDto
	{
		public int ClassId { get; set; }
		public string Acronym { get; set; }
		public int CourseNumber { get; set; }
		public string Building { get; set; }
		public string RoomNumber { get; set; }
		public byte? WeekdayBitMap { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
	}
}
