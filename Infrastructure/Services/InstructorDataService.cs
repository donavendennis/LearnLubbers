using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

public class InstructorDataService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InstructorDataService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public void RefreshInstructorData(string instructorId)
    {
        var context = _httpContextAccessor.HttpContext;

        // Debug log: Start of method
        Console.WriteLine($"Refreshing instructor data for InstructorId: {instructorId}");

        // Fetch all classes taught by the instructor
        var objClasses = _unitOfWork.Class.GetAll()
            .Where(c => c.InstructorId == instructorId)
            .ToList();

        Console.WriteLine($"Classes fetched for InstructorId {instructorId}: {objClasses.Count}");

        // Fetch associated calendars for the instructor's classes
        var objCalendars = objClasses
            .Select(c => _unitOfWork.Calendar.GetById(c.CalendarId))
            .Where(calendar => calendar != null)
            .ToList();

        Console.WriteLine($"Calendars fetched for InstructorId {instructorId}: {objCalendars.Count}");

        // Fetch events tied to the calendars
        var objEvents = objCalendars
            .SelectMany(cal => _unitOfWork.Event.GetAll().Where(e => e.CalendarId == cal.CalendarId))
            .ToList();

        Console.WriteLine($"Events fetched for InstructorId {instructorId}: {objEvents.Count}");

        // Fetch recurring rules for the events
        var objRecurringRules = objEvents
            .Where(e => e.RecurringRuleId != null)
            .Select(e => _unitOfWork.RecurringRule.GetById(e.RecurringRuleId.Value))
            .Where(rule => rule != null)
            .ToList();

        Console.WriteLine($"Recurring Rules fetched for InstructorId {instructorId}: {objRecurringRules.Count}");

        // Store the data in session
        context.Session.SetString("InstructorClasses", JsonSerializer.Serialize(objClasses, new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        }));

        context.Session.SetString("InstructorCalendars", JsonSerializer.Serialize(objCalendars, new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        }));

        context.Session.SetString("InstructorEvents", JsonSerializer.Serialize(objEvents, new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        }));

        context.Session.SetString("InstructorRecurringRules", JsonSerializer.Serialize(objRecurringRules, new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        }));

        Console.WriteLine("Instructor data refreshed and stored in session.");
    }

}
