using Infrastructure.Interfaces;
using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Canvas_Like.Pages
{
  [Authorize]
  public class IndexModel : PageModel
  {
    private readonly ILogger<IndexModel> _logger;
    private readonly IUnitOfWork _unitOfWork;
    // public List<Assignment> objAssignments;
    public List<ToDo> objToDos;
    public List<Class> objClasses;

    public List<int> AssignmentIds;
    //public List<System.Drawing.Image> objSystemImages;
    public IndexModel(ILogger<IndexModel> logger, UnitOfWork unitOfWork)//, IUnitOfWork unitOfWork)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      objToDos = new List<ToDo>();
      objClasses = new List<Class>();
      AssignmentIds = new List<int>();
    }

        public void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // Check if session already has the data
                string classesSessionKey = User.IsInRole("Instructor") ? "InstructorClasses" : "UserClasses";
                string toDosSessionKey = "UserToDos";

                if (User.IsInRole("Instructor"))
                {
                    // Always refresh session data for instructors to ensure consistency
                    objClasses = HttpContext.Session.GetString(classesSessionKey) == null
                        ? _unitOfWork.Class.GetAll().Where(c => c.InstructorId == userId).ToList()
                        : JsonConvert.DeserializeObject<List<Class>>(HttpContext.Session.GetString(classesSessionKey));

                    // Store the result in session
                    HttpContext.Session.SetString(classesSessionKey, JsonConvert.SerializeObject(objClasses, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        Formatting = Formatting.Indented
                    }));
                }
                else if (User.IsInRole("Student"))
                {
                    // Check if session already has the data for students
                    if (string.IsNullOrEmpty(HttpContext.Session.GetString(classesSessionKey)) ||
                        string.IsNullOrEmpty(HttpContext.Session.GetString(toDosSessionKey)))
                    {
                        // Get registered classes for the student
                        List<int> registeredClassIds = _unitOfWork.StudentRegistration.GetAll()
                            .Where(rC => rC.StudentId == userId)
                            .Select(rC => rC.ClassId)
                            .ToList();

                        objClasses = _unitOfWork.Class.GetAll()
                            .Where(c => registeredClassIds.Contains(c.ClassId)).ToList();

                        // Get To-Dos for assignments related to the classes
                        var assignmentsWithToDos = _unitOfWork.Assignment
                            .GetAll(predicate: assignment => registeredClassIds.Contains(assignment.ClassId)
                                                              && assignment.ToDo != null
                                                              && assignment.ToDo.DueDate > DateTime.Now,
                                                              includes: "ToDo")
                            .OrderBy(assignment => assignment.ToDo.DueDate)
                            .Take(5)
                            .ToList();

                        objToDos = assignmentsWithToDos.Select(assignment => assignment.ToDo).ToList();
                        AssignmentIds = assignmentsWithToDos.Select(assignment => assignment.AssignmentId).ToList();

                        // Store the results in session
                        HttpContext.Session.SetString(classesSessionKey, System.Text.Json.JsonSerializer.Serialize(objClasses, new JsonSerializerOptions
                        {
                            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                            WriteIndented = true
                        }));

                        HttpContext.Session.SetString(toDosSessionKey, System.Text.Json.JsonSerializer.Serialize(objToDos, new JsonSerializerOptions
                        {
                            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                            WriteIndented = true
                        }));
                    }
                    else
                    {
                        // Use session data for classes and To-Dos
                        objClasses = JsonConvert.DeserializeObject<List<Class>>(HttpContext.Session.GetString(classesSessionKey));
                        objToDos = JsonConvert.DeserializeObject<List<ToDo>>(HttpContext.Session.GetString(toDosSessionKey));

                        // Recalculate AssignmentIds
                        var assignments = _unitOfWork.Assignment.GetAll(
                            a => objToDos.Select(t => t.ToDoId).Contains(a.ToDoId)
                        ).ToList();

                        AssignmentIds = assignments.Select(a => a.AssignmentId).ToList();
                    }
                }
            }
        }

    }
}
