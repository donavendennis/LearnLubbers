using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Utility;

namespace Canvas_Like.Pages.Classes
{
  public class UpsertModel : PageModel
  {
    #region Dependency Injection
    private readonly UnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    [BindProperty]
    public Class objClass { get; set; }
    [BindProperty]
    public Infrastructure.Models.Calendar objCalendar { get; set; }
    [BindProperty]
    public Event objEvent { get; set; }
    [BindProperty]
    public RecurringRule objRecurringRulE { get; set; }
    [BindProperty]
    public CalendarAccess objCalendarAccess { get; set; }
    [BindProperty]
    public IEnumerable<SelectListItem> DepartmentList { get; set; }
    [BindProperty]
    public List<string> DaysMet { get; set; }
    [BindProperty]
    public TimeOnly objTimeStart { get; set; }
    [BindProperty]
    public TimeOnly objTimeEnd { get; set; }
    [BindProperty]
    public DateOnly objDateStart { get; set; }
    [BindProperty]
    public DateOnly objDateEnd { get; set; }

    private string? InstructorId;

    private readonly InstructorDataService _instructorDataService;


    public UpsertModel(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, InstructorDataService instructorDataService)
    {
      _unitOfWork = unitOfWork;
      _webHostEnvironment = webHostEnvironment;
      _instructorDataService = instructorDataService;
      objClass = new Class();
      objCalendar = new Infrastructure.Models.Calendar();
      objEvent = new Event();
      objRecurringRulE = new RecurringRule();
      objCalendarAccess = new CalendarAccess();
      DepartmentList = new List<SelectListItem>();
      DaysMet = new List<string>();
      objTimeStart = new TimeOnly();
      objTimeEnd = new TimeOnly();
      objDateStart = new DateOnly();
      objDateEnd = new DateOnly();
    }
    #endregion

    public IActionResult OnGet(int? id)
    {

      var claimsIdentity = User.Identity as ClaimsIdentity;
      InstructorId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      //declare variables for manipulation
      objClass = new Class();
      objCalendar = new Infrastructure.Models.Calendar();
      objEvent = new Event();
      objRecurringRulE = new RecurringRule();
      objCalendarAccess = new CalendarAccess();
      //populate the department list
      DepartmentList = _unitOfWork.Department.GetAll()
          .Select(d => new SelectListItem
          {
            Value = d.DepartmentId.ToString(),
            Text = d.Acronym
          });
      objDateStart = DateOnly.FromDateTime(DateTime.Today);
      objDateEnd = DateOnly.FromDateTime(DateTime.Today);
      objTimeStart = TimeOnly.Parse("12:00:00 PM");
      objTimeEnd = TimeOnly.Parse("12:00:00 PM");

      //if class id is null, create a new class object and return the page
      if (id == null || id == 0)
      {
        return Page();
      }

      //if class id is not null, get the class object from the database
      if (id != 0)
      {
        objClass = _unitOfWork.Class.GetById(id);
        objDateStart = DateOnly.FromDateTime(objClass.StartDate);
        objDateEnd = DateOnly.FromDateTime(objClass.EndDate);
        objTimeStart = TimeOnly.FromDateTime(objClass.StartDate);
        objTimeEnd = TimeOnly.FromDateTime(objClass.EndDate);
      }
      //retrieve the calendar object from the database
      if (objClass.CalendarId != 0)
      {
        objCalendar = _unitOfWork.Calendar.GetById(objClass.CalendarId);
      }
      //retrieve the event object from the database
      if (objCalendar.CalendarId != 0)
      {
        objEvent = _unitOfWork.Event.Get(u => u.CalendarId == objCalendar.CalendarId);
      }
      //retrieve the recurring rule object from the database
      if (objEvent.RecurringRuleId != 0)
      {
        objRecurringRulE = _unitOfWork.RecurringRule.Get(u => u.RecurringRuleId == objEvent.RecurringRuleId);
        // //declare date and time objects
        WeekDayBitMapping dayMapping = new WeekDayBitMapping(objRecurringRulE.WeekdayBitMap);
        DaysMet = dayMapping.WeekDayString().Split(", ").ToList();
      }
      //retrieve the calendar access object from the database
      if (objCalendar.CalendarId != 0)
      {
        objCalendarAccess = _unitOfWork.CalendarUserRole.Get(u => u.CalendarId == objCalendar.CalendarId && u.ApplicationUserId == InstructorId);
      }

      //any retrieved objects are null, return not found
      if (objClass == null || objCalendar == null || objEvent == null || objRecurringRulE == null || objCalendarAccess == null)
      {
        return NotFound();
      }

      // Log the state of objClass before returning the page
      Console.WriteLine($"Before return Page(): objClass.ClassId = {objClass.ClassId}");

      return Page();
    }

    public IActionResult OnPost()
    {//if create/update
      //var sameName = _unitOfWork.Class.GetAll().Where((c) => c.Title == objClass.Title).Any();

      //if (sameName)
      //{
      //  ModelState.AddModelError("objClass.Title", "There is already a class with this name");
      //  return Page();
      //}

      var claimsIdentity = User.Identity as ClaimsIdentity;
      InstructorId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (objClass.ClassId == 0)
      {
        //create a new calendar object
        objCalendar.CalendarName = objClass.Title;
        objCalendar.Color = "#00FF00";
        _unitOfWork.Calendar.Add(objCalendar);

        //Create new Class object
        objClass.Calendar = objCalendar;
        objClass.CalendarId = objCalendar.CalendarId;
        objClass.StartDate = objDateStart.ToDateTime(objTimeStart);
        objClass.EndDate = objDateEnd.ToDateTime(objTimeEnd);
        objClass.InstructorId = InstructorId;
        objClass.Department = _unitOfWork.Department.GetById(objClass.DepartmentId);
        _unitOfWork.Class.Add(objClass);

        //Create new Recurring Rule object
        objRecurringRulE.WeekdayBitMap = WeekDayBitMapping.GetWeekdayBitMap(DaysMet);
        objRecurringRulE.Skip = 1;
        objRecurringRulE.Frequency = FrequencyConstants.Weekly;
        objRecurringRulE.EndDate = objDateEnd.ToDateTime(objTimeEnd);
        _unitOfWork.RecurringRule.Add(objRecurringRulE);

        //Create new Event object
        objEvent.CalendarId = objCalendar.CalendarId;
        objEvent.Start = objDateStart.ToDateTime(objTimeStart);
        objEvent.End = objDateEnd.ToDateTime(objTimeEnd);
        Department dep = _unitOfWork.Department.GetById(objClass.DepartmentId);
        if(dep == null) dep = new Department{DepartmentId = 1, Acronym = "CS"};
        objEvent.Title = "Class " + dep + " " + objClass.CourseNumber;
        objEvent.Description = objClass.Title;
        objEvent.Location = objClass.Building + " " + objClass.RoomNumber;
        objEvent.CreatorId = InstructorId;
        objEvent.RecurringRuleId = objRecurringRulE.RecurringRuleId;
        objEvent.CanDelete = false;
        _unitOfWork.Event.Add(objEvent);

        //Create new Calendar Access object
        objCalendarAccess.ApplicationUserId = InstructorId;
        objCalendarAccess.CalendarId = objCalendar.CalendarId;
        CalendarRole objCalendarRole = _unitOfWork.CalendarRole.Get(r => r.Role == CalendarRoleConstants.Owner);
        if(objCalendarRole == null) objCalendarRole = new CalendarRole { CalendarRoleId = 1, Role = CalendarRoleConstants.Owner };
        objCalendarAccess.CalendarRoleId = objCalendarRole.CalendarRoleId;
        _unitOfWork.CalendarUserRole.Add(objCalendarAccess);
      }
      else //if update
      {
        //Update Calendar Object
        objCalendar.CalendarName = objClass.Title;
        objCalendar.Color = "#00FF00";
        _unitOfWork.Calendar.Update(objCalendar);

        //Update Class Object
        objClass.StartDate = objDateStart.ToDateTime(objTimeStart);
        objClass.EndDate = objDateEnd.ToDateTime(objTimeEnd);
        _unitOfWork.Class.Update(objClass);

        //update recurring rule object
        objRecurringRulE.WeekdayBitMap = WeekDayBitMapping.GetWeekdayBitMap(DaysMet);
        objRecurringRulE.EndDate = objDateEnd.ToDateTime(objTimeEnd);
        _unitOfWork.RecurringRule.Update(objRecurringRulE);

        //update event object
        objEvent.Start = objDateStart.ToDateTime(objTimeStart);
        objEvent.End = objDateEnd.ToDateTime(objTimeEnd);
        objEvent.Title = "Class " + _unitOfWork.Department.GetById(objClass.DepartmentId).Acronym + " " + objClass.CourseNumber;
        objEvent.Description = objClass.Title;
        objEvent.Location = objClass.Building + " " + objClass.RoomNumber;
        _unitOfWork.Event.Update(objEvent);
      }
            //commit the changes to the database
            //_unitOfWork.Commit();

      _instructorDataService.RefreshInstructorData(InstructorId);
      return RedirectToPage("./Index");
    }

  }
}
