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
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace Canvas_Like.Tests.UnitTests
{
    [TestClass]
    public class InstructorCreateClassTest
    {
        private Mock<IUnitOfWork> _mockUnitOfWork = null!;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment = null!;
        private UpsertModel _pageModel = null!;
        private Mock<InstructorDataService> _mockInstructorDataService = null!;

        public InstructorCreateClassTest()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create a real ApplicationDbContext and UnitOfWork
            var realDbContext = new ApplicationDbContext(options);
            var realUnitOfWork = new UnitOfWork(realDbContext);

            // Initialize Mock Objects
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // Set up the HttpContext for IHttpContextAccessor
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "InstructorId") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockHttpContext = new DefaultHttpContext { User = claimsPrincipal };
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            // Create a real InstructorDataService using the mock IUnitOfWork and IHttpContextAccessor
            var instructorDataService = new InstructorDataService(realUnitOfWork, mockHttpContextAccessor.Object);

            // Create UpsertModel with Real UnitOfWork and Real InstructorDataService
            _pageModel = new UpsertModel(
                realUnitOfWork, // Real UnitOfWork
                _mockWebHostEnvironment.Object,
                instructorDataService // Real InstructorDataService with mocked IHttpContextAccessor
            );
        }



        [TestInitialize]
        public void SetUp()
        {
            // Set up HttpContext with Claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "InstructorId") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockHttpContext = new DefaultHttpContext { User = claimsPrincipal };

            // Set up a mock session
            var mockSession = new Mock<ISession>();
            byte[] data = null;

            mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out data)).Returns(false);
            mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>())).Callback<string, byte[]>((key, value) => data = value);
            mockHttpContext.Session = mockSession.Object;

            // Configure IHttpContextAccessor
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            // Reinitialize dependencies
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var realDbContext = new ApplicationDbContext(options);
            var realUnitOfWork = new UnitOfWork(realDbContext);
            var instructorDataService = new InstructorDataService(realUnitOfWork, mockHttpContextAccessor.Object);

            // Initialize the UpsertModel
            _pageModel = new UpsertModel(
                realUnitOfWork,
                _mockWebHostEnvironment.Object,
                instructorDataService
            );

            _pageModel.PageContext = new PageContext { HttpContext = mockHttpContext };

            // Initialize Bind Properties for UpsertModel
            _pageModel.objClass = new Class
            {
                DepartmentId = 1,
                CourseNumber = 101,
                Title = "Introduction to Computer Science",
                Building = "Main Hall",
                RoomNumber = "101",
                CreditHours = 3
            };
            _pageModel.objDateStart = DateOnly.FromDateTime(new DateTime(2023, 1, 1));
            _pageModel.objDateEnd = DateOnly.FromDateTime(new DateTime(2023, 5, 1));
            _pageModel.objTimeStart = new TimeOnly(9, 0);
            _pageModel.objTimeEnd = new TimeOnly(10, 0);
            _pageModel.DaysMet = new List<string> { "Monday", "Wednesday", "Friday" };

            _pageModel.objCalendar = new Calendar();
            _pageModel.objEvent = new Event();
            _pageModel.objRecurringRulE = new RecurringRule();
            _pageModel.objCalendarAccess = new CalendarAccess();
        }


        [TestMethod]
        public void OnPost_CreateNewCourse_Success()
        {
            // Arrange
            var department = new Department { DepartmentId = 1, Acronym = "CS" };
            var calendarRole = new CalendarRole { CalendarRoleId = 1, Role = CalendarRoleConstants.Owner };

            _mockUnitOfWork.Setup(u => u.Department.GetById(It.IsAny<int>())).Returns(department);
            _mockUnitOfWork.Setup(u => u.CalendarRole.Get(
                It.Is<Expression<Func<CalendarRole, bool>>>(expr => expr.Compile()(calendarRole)),
                false,
                null
            )).Returns(calendarRole);

            // Act
            var result = _pageModel.OnPost();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
            Assert.AreNotEqual(_pageModel.objClass.ClassId, 0);
            Assert.AreNotEqual(_pageModel.objCalendar.CalendarId, 0);
            Assert.AreNotEqual(_pageModel.objEvent.EventId, 0);
            Assert.AreNotEqual(_pageModel.objRecurringRulE.RecurringRuleId, 0);
            Assert.AreNotEqual(_pageModel.objCalendarAccess.CalendarRoleId, 0);
            Assert.AreNotEqual(_pageModel.objCalendarAccess.ApplicationUserId, null);
            Assert.AreNotEqual(_pageModel.objCalendarAccess.CalendarId, 0);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureDeleted(); // Reset the database after each test
            }
        }

    }
}
