using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Canvas_Like.Pages.Registration
{
  public class ClassDataView
  {
    public int ClassId { get; set; }
    public string Department { get; set; }
    public int DepartmentId { get; set; }
    public string Title { get; set; }
    public int CourseNo { get; set; }
    public bool IsRegistered { get; set; }
  }

  public class IndexPageModel : PageModel
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly UnitOfWork _unitOfWork;
    private readonly StudentDataService _studentDataService;
    public List<ClassDataView> objClasses;
    public List<Department> departmentList;

        public IndexPageModel(UnitOfWork unitOfWork, UserManager<IdentityUser> userManager, StudentDataService studentDataService)
    {
      _userManager = userManager;
      _unitOfWork = unitOfWork;
      _studentDataService = studentDataService;
      objClasses = new List<ClassDataView>();
      departmentList = new List<Department>();
    }

        public void OnGet()
    {
      string userId = _userManager.GetUserId(User);
      List<Class> allClasses = _unitOfWork.Class.GetAll().ToList();
      List<int> allRegistrationIds = _unitOfWork.StudentRegistration
        .GetAll().Where(r => r.StudentId == userId)
        .Select(r => r.ClassId).ToList();

      List<Class> registeredClasses = allClasses
        .Where(c => allRegistrationIds.Contains(c.ClassId))
        .ToList();

      objClasses = new List<ClassDataView>(
        allClasses.Select(c => new ClassDataView
        {
          ClassId = c.ClassId,
          Department = _unitOfWork.Department.GetById(c.DepartmentId).Name,
          CourseNo = c.CourseNumber,
          DepartmentId = c.DepartmentId,
          Title = c.Title,
          IsRegistered = registeredClasses.Contains(c)
        })
      );

      objClasses.Sort((x, y) => x.IsRegistered.CompareTo(y.IsRegistered));


      departmentList = _unitOfWork.Department.GetAll().ToList();
    }

    public IActionResult OnPostRegister(int classId)
    {
      var userId = _userManager.GetUserId(User);
      Class classRegistered = _unitOfWork.Class.GetById(classId);

      if (_unitOfWork.StudentRegistration
          .Get(sr => sr.StudentId == userId && sr.ClassId == classId) == null)
      {
        var registration = new StudentRegistration
        {
          StudentId = userId,
          ClassId = classId,
          RegistrationDate = DateTime.Now
        };
        _unitOfWork.StudentRegistration.Add(registration);

        if (classRegistered.CalendarId != null)
        {
          CalendarRole viewRole = _unitOfWork.CalendarRole.GetAll()
            .Where(r => r.Role == "Viewer").FirstOrDefault();

          CalendarAccess calendarAccess = new CalendarAccess
          {
            ApplicationUserId = userId,
            CalendarId = (int)classRegistered.CalendarId,
            CalendarRoleId = viewRole.CalendarRoleId
          };
          _unitOfWork.CalendarUserRole.Add(calendarAccess);
        }
      }
            // Call the service to refresh student data
            _studentDataService.RefreshStudentData(userId);
            return RedirectToPage();
    }

    public IActionResult OnPostDrop(int classId)
    {
      Console.WriteLine("shit");
      var userId = _userManager.GetUserId(User);
      Class classDropped = _unitOfWork.Class.GetById(classId);

      var registrations = _unitOfWork.StudentRegistration.GetAll()
          .Where(sr => sr.StudentId == userId && sr.ClassId == classId).ToList();

      if (registrations != null)
      {
        foreach (var reg in registrations)
        {
          _unitOfWork.StudentRegistration.Delete(reg);
        }
      }

      if (classDropped.CalendarId != null)
      {
        CalendarRole viewRole = _unitOfWork.CalendarRole.GetAll()
          .Where(r => r.Role == "Viewer").FirstOrDefault();

        List<CalendarAccess> calendarAccess = _unitOfWork.CalendarUserRole.GetAll()
          .Where(a => a.CalendarRoleId == viewRole.CalendarRoleId &&
              a.CalendarId == classDropped.CalendarId).ToList();

        foreach (var access in calendarAccess)
        {
          _unitOfWork.CalendarUserRole.Delete(access);
        }
      }
            // Call the service to refresh student data
            _studentDataService.RefreshStudentData(userId);
            return RedirectToPage();
            return RedirectToPage();
    }
  }
}
