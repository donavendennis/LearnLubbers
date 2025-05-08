using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DataAccess;
using Infrastructure.Models;
using Canvas_Like.Pages.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq.Expressions;

namespace Canvas_Like.Tests.UnitTests
{
    [TestClass]
    public class InstructorDeleteClassTest
    {
        private ApplicationDbContext _context = null!;
        private Mock<IUnitOfWork> _mockUnitOfWork = null!;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment = null!;
        private DeleteModel _pageModel = null!;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor = null!;
        private UnitOfWork _realUnitOfWork = null!;

        [TestInitialize]
        public void SetUp()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            // Initialize the real UnitOfWork
            _realUnitOfWork = new UnitOfWork(_context);

            // Mock IUnitOfWork
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            // Seed the database using the real UnitOfWork
            SeedTestData(_realUnitOfWork);

            // Redirect mocked methods to real UnitOfWork
            _mockUnitOfWork.Setup(u => u.Class).Returns(_realUnitOfWork.Class);
            _mockUnitOfWork.Setup(u => u.Calendar).Returns(_realUnitOfWork.Calendar);
            _mockUnitOfWork.Setup(u => u.CalendarUserRole).Returns(_realUnitOfWork.CalendarUserRole);
            _mockUnitOfWork.Setup(u => u.ApplicationUser).Returns(_realUnitOfWork.ApplicationUser);
            _mockUnitOfWork.Setup(u => u.CalendarRole).Returns(_realUnitOfWork.CalendarRole);

            // Mock other dependencies
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "instructor123") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var mockHttpContext = new DefaultHttpContext { User = claimsPrincipal };

            // Mock session
            var mockSession = new Mock<ISession>();
            byte[] sessionData = null;

            mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out sessionData)).Returns(false);
            mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                       .Callback<string, byte[]>((key, value) => sessionData = value);
            mockHttpContext.Session = mockSession.Object;

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            // Initialize DeleteModel
            var instructorDataService = new InstructorDataService(_realUnitOfWork, _mockHttpContextAccessor.Object);
            _pageModel = new DeleteModel(_realUnitOfWork, _mockWebHostEnvironment.Object, instructorDataService);
            _pageModel.PageContext = new PageContext { HttpContext = mockHttpContext };
        }


        private void SeedTestData(UnitOfWork unitOfWork)
        {
            // Seed ApplicationUser
            var user = new ApplicationUser
            {
                Id = "instructor123",
                UserName = "testuser",
                Email = "testuser@example.com",
                FirstName = "Test", // Required field
                LastName = "User",  // Required field
                DateOfBirth = new DateTime(1985, 1, 1), // Required field
                LocationId = null, // Optional field
                ProfilePictureUrl = null // Optional field
            };
            unitOfWork.ApplicationUser.Add(user);

            // Seed CalendarRole
            var calendarRole = new CalendarRole
            {
                CalendarRoleId = 1,
                Role = "Owner" // Ensures this matches the validation logic
            };
            unitOfWork.CalendarRole.Add(calendarRole);

            // Seed Calendar
            var calendar = new Calendar
            {
                CalendarId = 100,
                CalendarName = "Test Calendar", // Required field
                Color = "#111111" // Required valid hex color
            };
            unitOfWork.Calendar.Add(calendar);

            // Seed Class
            var class_ = new Class
            {
                ClassId = 1,
                Title = "Test Class", // Required field
                DepartmentId = 1, // Use a valid DepartmentId
                Building = "Main", // Required field
                RoomNumber = "101", // Required field
                CalendarId = calendar.CalendarId, // Ensure Calendar exists
                InstructorId = user.Id, // Ensure Instructor exists
                CourseNumber = 101, // Default value set if not provided
                CreditHours = 3, // Default value set if not provided
                Capacity = 30, // Default value set if not provided
                StartDate = DateTime.Now.Date, // Required field
                EndDate = DateTime.Now.AddMonths(3).Date, // Required field
            };
            unitOfWork.Class.Add(class_);

            // Seed CalendarAccess
            var calendarAccess = new CalendarAccess
            {
                CalendarId = calendar.CalendarId, // Ensure Calendar exists
                ApplicationUserId = user.Id, // Ensure User exists
                CalendarRoleId = calendarRole.CalendarRoleId // Ensure Role exists
            };
            unitOfWork.CalendarUserRole.Add(calendarAccess);

            // Save changes
            _context.SaveChanges();
        }


        [TestMethod]
        public void InstructorCanDeleteClass()
        {
            // Arrange
            var originalClassCount = _context.Classes.Count();
            var classIdToDelete = 1;

            // Act
            var result = _pageModel.OnPost(classIdToDelete);
            var newClassCount = _context.Classes.Count();

            // Assert
            Assert.AreEqual(originalClassCount - 1, newClassCount, "Class count should decrease by 1.");
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult), "Result should be a redirect.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted(); // Reset the database after each test
        }
    }
}
