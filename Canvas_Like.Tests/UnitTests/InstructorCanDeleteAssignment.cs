using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Components.Forms;
using Canvas_Like.Pages.Assignments;
using Canvas_Like.Pages.Classes;
using Microsoft.AspNetCore.Hosting;
using Canvas_Like.Pages.Assignments.Submissions;
using IndexModel = Canvas_Like.Pages.Assignments.IndexModel;

namespace Canvas_Like.Tests.UnitTests
{
    [TestClass]
    public class InstructorDeleteAssignmentsTest
    {
        private ApplicationDbContext _context;
        private UnitOfWork _unitOfWork;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private IndexModel _pageModel;

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

            // Mock UserManager to return a user ID
            _mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user123");

            // Mock IWebHostEnvironment
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockWebHostEnvironment.Setup(e => e.WebRootPath).Returns("wwwroot");

            // Create the PageModel instance
            _pageModel = new IndexModel(_unitOfWork, _mockWebHostEnvironment.Object);

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
                { "action", "DeleteAssignment" }
            });

            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
        }

        [TestMethod]
        public async Task InstructorCanDeleteAssignment()
        {
            // Arrange: Add a class and assignment to the context
            var testClass = new Class
            {
                ClassId = 1,
                Title = "Test Class",
                Building = "Building A",
                RoomNumber = "101",
                InstructorId = "instructor123",
                CalendarId = 1,
            };
            _context.Classes.Add(testClass);

            // When deleting an assignment the ToDo needs to be cascade deleted as well.
            var testToDo = new ToDo
            {
                ToDoId = 1,
                CalendarId = 1,
                Completed = false,
                DueDate = DateTime.Now.AddDays(7),
                Title = "Test ToDo",
                Description = "This is a test ToDo",
            };
            _context.ToDos.Add(testToDo);

            var testAssignment = new Assignment
            {
                AssignmentId = 1,
                ClassId = testClass.ClassId,
                Title = "Test Assignment",
                Description = "Test Description",
                DueDateTime = DateTime.Now.AddDays(7),
                SubmissionType = "Text",
                Points = 100,
                Published = true,
                ToDoId = testToDo.ToDoId,
            };
            _context.Assignments.Add(testAssignment);

            await _context.SaveChangesAsync();

            // Act: Delete the assignment
            var result = await _pageModel.OnPostDeleteAsync(testAssignment.AssignmentId);

            // Assert: Verify that the assignment and ToDo is deleted
            var deletedAssignment = await _context.Assignments.FindAsync(testAssignment.AssignmentId);
            var deletedToDo = await _context.ToDos.FindAsync(testToDo.ToDoId);
            Assert.IsNull(deletedAssignment, "The assignment should be deleted.");
            Assert.IsNull(deletedToDo, "The ToDo should be deleted.");

        }

        [TestCleanupAttribute]
        public void Cleanup()
        {
            // Ensure the in-memory database is deleted after each test
            _context.Database.EnsureDeleted();
        }
    }
}