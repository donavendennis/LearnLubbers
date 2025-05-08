using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Moq;
using DataAccess;
using Infrastructure.Models;
using Canvas_Like.Pages.Assignments.Submissions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Canvas_Like.Tests.UnitTests
{
    [TestClass]
    public class StudentAssignmentSubmissionTest
    {
        private ApplicationDbContext _context;
        private UnitOfWork _unitOfWork;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private SubmitAssignmentModel _pageModel;

        [TestInitialize]
        public void Setup()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _unitOfWork = new UnitOfWork(_context);

            // Mock UserManager
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null
            );

            // Mock UserManager to return a user ID
            _mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("user123");

            // Mock IWebHostEnvironment
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockWebHostEnvironment.Setup(e => e.WebRootPath).Returns("wwwroot");

            // Create the PageModel instance
            _pageModel = new SubmitAssignmentModel(_unitOfWork, _mockWebHostEnvironment.Object);

            // Set up HttpContext for the PageModel
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user123")
                }))
            };
            httpContext.Request.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            httpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "action", "SubmitAssignment" }
            });

            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
        }

        [TestMethod]
        public async Task StudentCanSubmitTextEntryAssignment()
        {
            // Arrange: Add a class and assignment to the context
            var testClass = new Class
            {
                ClassId = 1,
                Title = "Test Class",
                Building = "Building A",
                RoomNumber = "101",
                InstructorId = "instructor123",
                CalendarId = 1 // Add the missing CalendarId to satisfy the requirement
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

            await _context.SaveChangesAsync();

            // Arrange: Create a new AssignmentSubmission entity and add it to the context
            var submission = new AssignmentSubmission
            {
                AssignmentId = testAssignment.AssignmentId,
                StudentId = "user123",
                SubmissionDateTime = DateTime.Now,
                Submitted = false,
                Grade = null
            };
            _context.AssignmentSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            // Act: Submit a text entry for the assignment
            _pageModel.objAssignment = testAssignment;
            _pageModel.objSubmission = submission;
            _pageModel.objSubmission.Submission = "This is my submission text.";

            IActionResult result = await _pageModel.OnPostAsync();

            // Assert: Check that the submission was updated as submitted
            var updatedSubmission = _context.AssignmentSubmissions.FirstOrDefault(s =>
                s.AssignmentId == testAssignment.AssignmentId && s.StudentId == "user123");

            Assert.IsNotNull(updatedSubmission, "Assignment submission should have been added to the database.");
            Assert.IsTrue(updatedSubmission.Submitted, "Submission should have been marked as submitted.");
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult), "Result should be a redirect after successful submission.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Ensure the in-memory database is deleted after each test
            _context.Database.EnsureDeleted();
        }
    }
}

