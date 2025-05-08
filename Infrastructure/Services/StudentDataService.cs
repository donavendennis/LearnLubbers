using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

public class StudentDataService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StudentDataService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public void RefreshStudentData(string userId)
    {
        var context = _httpContextAccessor.HttpContext;
        var registeredClassIds = _unitOfWork.StudentRegistration.GetAll()
            .Where(rC => rC.StudentId == userId)
            .Select(rC => rC.ClassId)
            .ToList();

        var objClasses = _unitOfWork.Class.GetAll()
            .Where(c => registeredClassIds.Contains(c.ClassId)).ToList();

        var assignmentsWithToDos = _unitOfWork.Assignment
            .GetAll(predicate: assignment => registeredClassIds.Contains(assignment.ClassId)
                                              && assignment.ToDo != null
                                              && assignment.ToDo.DueDate > DateTime.Now,
                                              includes: "ToDo")
            .OrderBy(assignment => assignment.ToDo.DueDate)
            .Take(5)
            .ToList();

        var objToDos = assignmentsWithToDos.Select(assignment => assignment.ToDo).ToList();

        // Store the data in session
        context.Session.SetString("UserClasses", JsonSerializer.Serialize(objClasses, new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        }));

        context.Session.SetString("UserToDos", JsonSerializer.Serialize(objToDos, new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        }));
    }
}

