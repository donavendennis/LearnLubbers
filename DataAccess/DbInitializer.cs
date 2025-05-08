using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Utility;

//using Utility;

namespace DataAccess.DbInitializer
{
  public class DbInitializer : IDbInitializer
  {
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;


    public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
      _db = db;
      _userManager = userManager;
      _roleManager = roleManager;
    }

    public void Initialize()
    {
      _db.Database.EnsureCreated();

      //migrations if they are not applied
      try
      {
        if (_db.Database.GetPendingMigrations().Any())
        {
          _db.Database.Migrate();
        }
      }
      catch (Exception)
      {

      }

      if (!_db.ApplicationUsers.Any()) InitUsers();
      if (!_db.CalendarEvents.Any()) InitCalendarEvents();
      if (!_db.CalendarRoles.Any()) InitCalendarRoles();
      if (!_db.Calendars.Any()) InitCalendar();
      if (!_db.RecurringRules.Any()) InitRecurringRule();
      if (!_db.Events.Any()) InitEvents();
      if (!_db.Departments.Any()) InitDepartment();
      if (!_db.Classes.Any()) InitClass();
      if (!_db.ToDos.Any()) InitToDo();
      if (!_db.Assignments.Any()) InitAssignment();
      if (!_db.AssignmentAttachments.Any()) InitAssignmentAttachment();
      if (!_db.AssignmentSubmissions.Any()) InitAssignmentSubmission();

    }

    public void InitUsers()
    {

      //create roles if they are not created
      //SD is a “Static Details” class we will create in Utility to hold constant strings for Roles

      _roleManager.CreateAsync(new IdentityRole(SD.InstructorRole)).GetAwaiter().GetResult();
      _roleManager.CreateAsync(new IdentityRole(SD.StudentRole)).GetAwaiter().GetResult();

      //Create at least one "Super Admin" or “Admin”.  Repeat the process for other users you want to seed

      _userManager.CreateAsync(new ApplicationUser
      {
        UserName = "donavendennis@mail.weber.edu",
        Email = "donavendennis@mail.weber.edu",
        FirstName = "Donaven",
        LastName = "Dennis",
        EmailConfirmed = true,
        DateOfBirth = new DateTime(1996, 01, 08)
      }, "Admin123*").GetAwaiter().GetResult();

      ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "donavendennis@mail.weber.edu");

      _userManager.AddToRoleAsync(user, SD.InstructorRole).GetAwaiter().GetResult();

      _userManager.CreateAsync(new ApplicationUser
      {
        UserName = "test@test.com",
        Email = "test@test.com",
        FirstName = "Bryson",
        LastName = "Test",
        EmailConfirmed = true,
        DateOfBirth = new DateTime(2001, 01, 08)
      }, "Admin123*").GetAwaiter().GetResult();

      ApplicationUser user2 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "test@test.com");

      _userManager.AddToRoleAsync(user2, SD.InstructorRole).GetAwaiter().GetResult();

      _userManager.CreateAsync(new ApplicationUser
      {
        UserName = "student@weber.edu",
        Email = "student@weber.edu",
        FirstName = "Student",
        LastName = "Name",
        EmailConfirmed = true,
        DateOfBirth = new DateTime(2001, 01, 08)
      }, "Password123*").GetAwaiter().GetResult();

      ApplicationUser user3 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "student@weber.edu");

      _userManager.AddToRoleAsync(user3, SD.StudentRole).GetAwaiter().GetResult();

      _userManager.CreateAsync(new ApplicationUser
      {
        UserName = "arpitchristi@weber.edu",
        Email = "arpitchristi@weber.edu",
        FirstName = "Arpit",
        LastName = "Christi",
        EmailConfirmed = true,
        DateOfBirth = new DateTime(1981, 06, 08)
      }, "ChrisiIsTheBest2024!").GetAwaiter().GetResult();

      ApplicationUser user4 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "arpitchristi@weber.edu");

      _userManager.AddToRoleAsync(user4, SD.InstructorRole).GetAwaiter().GetResult();

      _db.SaveChanges();
    }

    /// <summary>
    /// to be deleted
    /// </summary>
    public void InitCalendarEvents()
    {
      ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "test@test.com");

      var objectCalendarEvents = new List<CalendarEvent>
                {
                    new CalendarEvent
                    {
                        Title = "Event 1",
                        StartDate = new DateTime(2024, 10, 1, 10, 0, 0),
                        EndDate = new DateTime(2024, 10, 1, 12, 0, 0),
                        Description= "This is event 1",
                        ApplicationUser= user
                    },
                    new CalendarEvent
                    {
                        Title = "Event 2",
                        StartDate = new DateTime(2024, 10, 2, 14, 0, 0),
                        EndDate = new DateTime(2024, 10, 2, 16, 0, 0),
                        Description= "This is event 2",
                        ApplicationUser= user
                    },
                };

      foreach (var c in objectCalendarEvents)
      {
        _db.CalendarEvents.Add(c);
      }

      _db.SaveChanges();
    }

    public void InitRecurringRule()
    {
      var objectRecurringRule = new List<RecurringRule>
            {
                new RecurringRule
                {
                    Skip = 1,
                    EndDate = new DateTime(2024, 12, 12),
                    Frequency = FrequencyConstants.Weekly,
                    WeekdayBitMap = 0x92
                },
            };

      foreach (var c in objectRecurringRule)
      {
        _db.RecurringRules.Add(c);
      }
      _db.SaveChanges();
    }


    public void InitDepartment()
    {
      var objectDepartment = new List<Department>
            {
                new Department
                {
                    Name = DepartmentsOptions.CS,
                    Acronym = DepartmentsAcronyms.CS,
                },
                new Department
                {
                    Name = DepartmentsOptions.Music,
                    Acronym = DepartmentsAcronyms.Music,
                }, new Department
                {
                    Name = DepartmentsOptions.EGE,
                    Acronym = DepartmentsAcronyms.EGE,
                }, new Department
                {
                    Name = DepartmentsOptions.English,
                    Acronym = DepartmentsAcronyms.English,
                }, new Department
                {
                    Name = DepartmentsOptions.Math,
                    Acronym = DepartmentsAcronyms.Math,
                }
            };
      foreach (var c in objectDepartment)
      {
        _db.Departments.Add(c);
      }
      _db.SaveChanges();
    }

    public void InitCalendarRoles()
    {
      List<string> roles = new List<string> { "Owner", "Editor", "Viewer" };

      roles.ForEach((role) =>
      {
        _db.CalendarRoles.Add(
                    new CalendarRole
                {
                  Role = role
                }
                );
      });
      _db.SaveChanges();
    }

    public void InitCalendar()
    {
      ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "arpitchristi@weber.edu");

      var objectCalendar = new List<Calendar>
            {
                new Calendar
                {
                    CalendarName = "CS3750",
                    Color = "#00FF00"
                },
                new Calendar
                {
                    CalendarName = "Arpit's Calendar",
                    Color = "#0000FF"
                }
            };
      foreach (var c in objectCalendar)
      {
        _db.Calendars.Add(c);
      }

      _db.SaveChanges();

      Calendar calendar0 = _db.Calendars.FirstOrDefault(
          u => u.CalendarName == "Arpit's Calendar"
      );
      CalendarRole ownerRole = _db.CalendarRoles.FirstOrDefault(
          r => r.Role == "Owner"
      );

      Calendar calendar1 = _db.Calendars.FirstOrDefault(
          u => u.CalendarName == "CS3750"
      );


      if (user == null || calendar0 == null || ownerRole == null)
      {
        Console.WriteLine("Unable to create Calendar Access");
        return;
      }

      if (user == null || calendar1 == null || ownerRole == null)
      {
        Console.WriteLine("Unable to create Calendar Access");
        return;
      }

      _db.CalendarAccesses.Add(
          new CalendarAccess
          {
            ApplicationUserId = user.Id,
            CalendarId = calendar0.CalendarId,
            CalendarRoleId = ownerRole.CalendarRoleId
          }
      );

      _db.CalendarAccesses.Add(
          new CalendarAccess
          {
            ApplicationUserId = user.Id,
            CalendarId = calendar1.CalendarId,
            CalendarRoleId = ownerRole.CalendarRoleId
          }
      );

      _db.SaveChanges();
    }

    public void InitEvents()
    {
      Calendar calendar0 = _db.Calendars.FirstOrDefault(c => c.CalendarName == "CS3750");
      RecurringRule recurringRule0 = _db.RecurringRules.FirstOrDefault(c => c.Skip == 1);
      ApplicationUser user0 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "arpitchristi@weber.edu");
      var objectEvent = new List<Event>
            {
                new Event
                {
                    CalendarId = calendar0.CalendarId,
                    Start = new DateTime(2024, 8, 30, 9,00,00),
                    End = new DateTime(2024, 8, 30, 11,30,00),
                    Title = "Class CS3750",
                    Description = "Software Engineering 2",
                    Location = "CAE 142",
                    CreatorId = user0.Id,
                    CanDelete = false,
                    RecurringRuleId =recurringRule0.RecurringRuleId
                },
            };
      foreach (var c in objectEvent)
      {
        _db.Events.Add(c);
      }
      _db.SaveChanges();


      _db.RecurringRules.Add(new RecurringRule
      {
        Skip = 1,
        EndDate = DateTime.Parse("2024-09-19 14:30:00"),
        Frequency = "Weekly",
        WeekdayBitMap = 0b01010100
      });


      _db.SaveChanges();

      RecurringRule recurring = _db.RecurringRules.FirstOrDefault();
      ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(
          u => u.Email == "arpitchristi@weber.edu");
      Calendar calendar = _db.Calendars.FirstOrDefault(
          u => u.CalendarName == "Arpit's Calendar");


      if (calendar == null || user == null || recurring == null)
      {
        Console.WriteLine("Unable to create Calendar Events");
        return;
      }

      List<Event> events = new List<Event>()
            {
                new Event
                      {
                      CalendarId = calendar.CalendarId,
                      Start = DateTime.Parse("2024-09-19 14:30:00"),
                      End = DateTime.Parse("2024-09-19 17:30:00"),
                      Title = "Tea Party",
                      Description = "A fun party with my friends to tell the my secrets",
                      Location = "West Park, Fun City",
                      CreatorId = user.Id,
                      CanDelete = true
                },
                new Event
                {
                      CalendarId = calendar.CalendarId,
                      Start = DateTime.Parse("2024-09-11 12:30:00"),
                      End = DateTime.Parse("2024-09-13 11:30:00"),
                      Title = "Camping Trip",
                      Description = "A trip to Causey to get my fishing on",
                      Location = "Causey Resevoir, UT",
                      CreatorId = user.Id,
                      CanDelete = false
                },
                new Event
                {
                      CalendarId = calendar.CalendarId,
                      Start = DateTime.Parse("2024-09-10 14:30:00"),
                      End = DateTime.Parse("2024-09-10 17:30:00"),
                      Title = "Grocery Shopping",
                      Description = "Time to go to the store and restock!",
                      Location = "West Park, Fun City",
                      CreatorId = user.Id,
                      CanDelete = true,
                      RecurringRuleId = recurring.RecurringRuleId
                },
            };

      events.ForEach((e) =>
      {
        _db.Add(e);
      });

      foreach (var c in events)
      {
        _db.Events.Add(c);
      }

      _db.SaveChanges();
    }

    public void InitClass()
    {
      ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "arpitchristi@weber.edu");
      Department department = _db.Departments.FirstOrDefault(c => c.Name == DepartmentsOptions.CS);
      Calendar calendar = _db.Calendars.FirstOrDefault(c => c.CalendarName == "CS3750");

      if (user == null || department == null || calendar == null)
      {
        Console.WriteLine("Unable to initialize Class");
      }
      var objectClass = new List<Class>
            {
                new Class
                {
                    DepartmentId = department.DepartmentId,
                    InstructorId = user.Id,
                    Building = "CAE",
                    RoomNumber = "142",
                    CalendarId = calendar.CalendarId,
                    CourseNumber = 3750,
                    Title = "Software Engineering 2",
                    CreditHours = 4
                },
            };
      foreach (Class c in objectClass)
      {
        _db.Classes.Add(c);
      }
      _db.SaveChanges();
    }

    public void InitToDo()
    {
      Calendar calendar = _db.Calendars.FirstOrDefault(ca => ca.CalendarName == "CS3750");

      if (calendar == null)
      {
        Console.WriteLine("Unable to initialize ToDo");
      }
      var objectToDo = new List<ToDo>
            {
                new ToDo
                {
                    CalendarId = calendar.CalendarId,
                    Completed = false,
                    DueDate = DateTime.Parse("2024-10-10 17:30:00"),
                    Title = "Assignment 1",
                    Description = "Test assignment",
                },
            };
      foreach (ToDo td in objectToDo)
      {
        _db.ToDos.Add(td);
      }
      _db.SaveChanges();
    }

    public void InitAssignment()
    {
      Class course = _db.Classes.FirstOrDefault(c => c.Title == "Software Engineering 2");
      ToDo todo = _db.ToDos.FirstOrDefault(t => t.Title == "Assignment 1");

      if (todo == null || course == null)
      {
        Console.WriteLine("Unable to initialize Assignment");
      }
      var objectAssignment = new List<Assignment>
            {
                new Assignment
                {
                    ClassId = course.ClassId,
                    ToDoId = todo.ToDoId,
                    Title = "Assignment 1",
                    DueDateTime = DateTime.Parse("2024-10-10 17:30:00"),
                    Description = "Test assignment",
                    SubmissionType = SubmissionTypes.Txt,
                    Points = 100,
                    Published = true
                }
            };
      foreach (Assignment a in objectAssignment)
      {
        _db.Assignments.Add(a);
      }
      _db.SaveChanges();
    }

    public void InitAssignmentAttachment()
    {
      int assignmentId = _db.Assignments.FirstOrDefault(a => a.Title == "Assignment 1").AssignmentId;


      AssignmentAttachment AsAt = new AssignmentAttachment
      {
        AssignmentId = assignmentId,
        FileName = "Sprint4Notes",
        FileUrl = "/attachments/8b4eab0b-9d59-4733-b140-a16f3fd4d9a5.txt",
        Keep = true,
        KeepPerminant = true
      };

      _db.AssignmentAttachments.Add(AsAt);
      _db.SaveChanges();
    }

    public void InitAssignmentSubmission()
    {
      int? assignmentId = _db.Assignments.FirstOrDefault(a => a.Title == "Assignment 1").AssignmentId;
      string? userId = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "student@weber.edu").Id;
      if (userId == null || assignmentId == null)
      {
        Console.WriteLine("Unable to initialize AssignmentSubmission");
      }

      AssignmentSubmission submission = new AssignmentSubmission
      {
        AssignmentId = (int)assignmentId,
        StudentId = userId,
        Submitted = false,
        Submission = "This is a test submission",
        Grade = null,
        SubmissionDateTime = DateTime.Now
      };

      _db.AssignmentSubmissions.Add(submission);
      _db.SaveChanges();
    }
  }
}

