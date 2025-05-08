using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Moq;
using DataAccess;
using Infrastructure.Models;
using Canvas_Like.Pages.Assignments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Canvas_Like.Tests.UnitTests
{
    [TestClass]
    public class InstructorAssignmentCreationTest
    {
        private ApplicationDbContext _context;
        private UnitOfWork _unitOfWork;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private UpsertModel _pageModel;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _unitOfWork = new UnitOfWork(_context);

            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockWebHostEnvironment.Setup(e => e.WebRootPath).Returns("wwwroot");

            _pageModel = new UpsertModel(_unitOfWork, _mockWebHostEnvironment.Object);
        }

        [TestMethod]
        public async Task Test_CreateAssignment()
        {
            var testCalendar = new Calendar
            {
                CalendarId = 1,
                CalendarName = "Test Calendar"

            };

            _context.Calendars.Add(testCalendar);

            var testClass = new Class
            {
                ClassId = 1,
                Title = "Test Class",
                Building = "Building A",
                RoomNumber = "101",
                InstructorId = "instructor123",
                CalendarId = testCalendar.CalendarId
            };
            _context.Classes.Add(testClass);

            await _context.SaveChangesAsync();

            await _pageModel.OnGet(testClass.ClassId, null);

            // Set the necessary properties for the assignment
            _pageModel.objAssignment.Title = "New Assignment";
            _pageModel.objAssignment.Description = "Assignment Description";
            _pageModel.objAssignment.DueDateTime = DateTime.Now.AddDays(7);
            _pageModel.objAssignment.Points = 100;
            _pageModel.objAssignment.SubmissionType = SubmissionTypes.Txt;

            // Simulate form action "Publish"
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "action", "Publish" }
            });

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = formCollection;

            // Set PageContext
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            var result = await _pageModel.OnPostAsync();

            // Assert: Check that the assignment was saved and published
            var createdAssignment = _context.Assignments.FirstOrDefault(a => a.Title == "New Assignment");

            Assert.IsNotNull(createdAssignment, "Assignment should have been created and saved to the database.");
            Assert.IsTrue(createdAssignment.Published, "Assignment should be published.");
            Assert.AreEqual(testClass.ClassId, createdAssignment.ClassId, "Assignment should be associated with the correct class.");
            Assert.AreEqual("Assignment Description", createdAssignment.Description, "Assignment description should match.");
            Assert.AreEqual(100, createdAssignment.Points, "Assignment points should match.");
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult), "Result should be a redirect after successful creation.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
        }
    }
}
