using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Utility;

namespace Canvas_Like.Pages.Classes
{
	public class DeleteModel : PageModel
	{
		private readonly UnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		[BindProperty]
		public Class Class { get; set; }
		[BindProperty]
		public TimeOnly StartTime { get; set; }
		[BindProperty]
		public TimeOnly EndTime { get; set; }
		public DateOnly StartDate { get; set; }
		[BindProperty]
		public DateOnly EndDate { get; set; }
		[BindProperty]
		public string DaysMet { get; set; }
		[BindProperty]
		public string DepartmentAcronym { get; set; }
		[BindProperty]
		private IEnumerable<CalendarAccess> calendarAccesses { get; set; }
		[BindProperty]
		private IEnumerable<Event> events { get; set; }
		[BindProperty]
		internal IEnumerable<RecurringRule> recurringRules { get; set; }
		private readonly InstructorDataService _instructorDataService;


		public DeleteModel(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, InstructorDataService instructorDataService)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
			_instructorDataService = instructorDataService;
			Class = new Class();
			StartTime = new TimeOnly();
			EndTime = new TimeOnly();
			StartDate = new DateOnly();
			EndDate = new DateOnly();
			DaysMet = "";
			DepartmentAcronym = "";
			calendarAccesses = new List<CalendarAccess>();
			events = new List<Event>();
			recurringRules = new List<RecurringRule>();
		}


		public IActionResult OnGet(int id)
		{
			Class = _unitOfWork.Class.GetById(id);
			if (Class == null)
			{
				return NotFound();
			}
			StartTime = TimeOnly.FromDateTime(Class.StartDate);
			EndTime = TimeOnly.FromDateTime(Class.EndDate);
			StartDate = DateOnly.FromDateTime(Class.StartDate);
			EndDate = DateOnly.FromDateTime(Class.EndDate);
			int? recurringRuleId = _unitOfWork.Event.Get(e => e.CalendarId == Class.CalendarId).RecurringRuleId;
			WeekDayBitMapping dayMapping = new WeekDayBitMapping(_unitOfWork.RecurringRule.GetById(recurringRuleId).WeekdayBitMap);
			DaysMet = dayMapping.WeekDayString();
			DepartmentAcronym = _unitOfWork.Department.GetById(Class.DepartmentId).Acronym;
			return Page();
		}

		public IActionResult OnPost(int id)
		{
			// Get all objects to delete
			Class = _unitOfWork.Class.GetById(id);
			Infrastructure.Models.Calendar calendar = _unitOfWork.Calendar.GetById(Class.CalendarId);
			calendarAccesses = _unitOfWork.CalendarUserRole.GetAll().Where(u => u.CalendarId == calendar.CalendarId).ToList();
			events = _unitOfWork.Event.GetAll().Where(e => e.CalendarId == calendar.CalendarId).ToList();
			recurringRules = _unitOfWork.RecurringRule.GetAll().Where(r => events.Select(e => e.RecurringRuleId).Contains(r.RecurringRuleId)).ToList();
			List<Assignment> assignments = _unitOfWork.Assignment.GetAll(a => a.ClassId == Class.CalendarId).ToList();
			List<AssignmentAttachment> assignmentAttachments = _unitOfWork.AssignmentAttachment.GetAll(a => assignments.Select(a => a.AssignmentId).Contains(a.AssignmentId)).ToList();
			List<ToDo> toDos = _unitOfWork.ToDo.GetAll(t => assignments.Select(a => a.ToDoId).Contains(t.ToDoId)).ToList();
			//Delete all objects in the database in order to ensure no foreign key constraints aren't violated
			foreach (var assignmentAttachment in assignmentAttachments)
			{
				_unitOfWork.AssignmentAttachment.Delete(assignmentAttachment);
			}
			foreach (var toDo in toDos)
			{
				_unitOfWork.ToDo.Delete(toDo);
			}
			foreach (var assignment in assignments)
			{
				_unitOfWork.Assignment.Delete(assignment);
			}
			foreach (var calendarAccess in calendarAccesses)
			{
				_unitOfWork.CalendarUserRole.Delete(calendarAccess);
			}
			foreach (var eventItem in events)
			{
				_unitOfWork.Event.Delete(eventItem);
			}
			foreach (var recurringRule in recurringRules)
			{
				_unitOfWork.RecurringRule.Delete(recurringRule);
			}
			_unitOfWork.Class.Delete(Class);
			_unitOfWork.Calendar.Delete(calendar);
            //return to the classes index page
            var claimsIdentity = User.Identity as ClaimsIdentity;
            string instructorId = claimsIdentity?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            _instructorDataService.RefreshInstructorData(instructorId);
			return RedirectToPage("./Index");
		}
	}
}
