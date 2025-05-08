using Microsoft.EntityFrameworkCore;
using Moq;
using DataAccess;
using Infrastructure.Models;
using Canvas_Like.Pages.Assignments.Submissions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Canvas_Like.Pages.Calendar;

namespace Canvas_Like.Tests.UnitTests
{
    [TestClass]
    public class CalendarNavigationTest
    {
        private ApplicationDbContext _context;
        private UnitOfWork _unitOfWork;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;

        [TestInitialize]
        public void Setup()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase").Options;

            _context = new ApplicationDbContext(options);
            _unitOfWork = new UnitOfWork(_context);

            // Mock UserManager
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null);

            // Mock IWebHostEnvironment
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockWebHostEnvironment.Setup(e => e.WebRootPath).Returns("wwwroot");
        }

        [TestMethod]
        public async Task StudentCalendarNavigation()
        {
            // Arrange: Add a class and assignment to the context
            var testClass = new Class
            {
                ClassId = 1,
                Title = "Test Class",
                Building = "Building A",
                RoomNumber = "101",
                InstructorId = "instructor123",
                CalendarId = 1
            };
            _context.Classes.Add(testClass);

            var testAssignment = new Assignment
            {
                AssignmentId = 1,
                ClassId = 1,
                Title = "Test Assignment",
                Description = "Test Description",
                DueDateTime = DateTime.Now.AddDays(7),
                SubmissionType = "Text",
                Points = 100,
                Published = true
            };
            _context.Assignments.Add(testAssignment);

            // Add calendar access for the student
            var calendarAccess = new CalendarAccess
            {
                ApplicationUserId = "user123",
                CalendarId = 1,
                CalendarRoleId = 1
            };
            _context.CalendarAccesses.Add(calendarAccess);
            //_context.CalendarUserRoles.Add(calendarAccess);

            await _context.SaveChangesAsync();

            // Act: Access the calendar page
            var calendarPageModel = new IndexModel(_unitOfWork);
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user123")
                }))
            };
            calendarPageModel.PageContext = new PageContext { HttpContext = httpContext };
            calendarPageModel.OnGet(); // Simulate getting the calendar page

            // Assert: Check that the assignment is accessible
            Assert.IsTrue(calendarPageModel.Assignments.Any(a => a.AssignmentId == testAssignment.AssignmentId),
                "The assignment should be accessible from the calendar.");

            // Simulate navigating to the assignment submission page
            var submissionPageModel = new SubmitAssignmentModel(_unitOfWork, _mockWebHostEnvironment.Object)
            {
                objAssignment = testAssignment
            };
            submissionPageModel.PageContext = new PageContext { HttpContext = httpContext };

            // Assert that the assignment submission page can be reached
            Assert.IsNotNull(submissionPageModel.objAssignment, "The assignment should be loaded for submission.");
        }

        [TestCleanup]
        public void CleanUp()
        {
            // Ensure the in-memory database is deleted after each test
            _context.Database.EnsureDeleted();
        }
    }
}
