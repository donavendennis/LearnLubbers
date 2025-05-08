using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Moq;
using DataAccess;
using Infrastructure.Models;
using Canvas_Like.Pages.Registration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Canvas_Like.Tests.UnitTests
{
  [TestClass]
  public class StudentEnrollmentTest
  {
    private ApplicationDbContext _context;
    private UnitOfWork _unitOfWork;
    private Mock<UserManager<IdentityUser>> _mockUserManager;
    private IndexPageModel _pageModel;
    private StudentDataService _studentDataService;

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

            _mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("user123");

            // Mock IHttpContextAccessor and Session
            var mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));
            mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                .Returns(false);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext)
                .Returns(new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, "user123")
                    })),
                    Session = mockSession.Object
                });

            // Initialize StudentDataService
            _studentDataService = new StudentDataService(_unitOfWork, mockHttpContextAccessor.Object);

            // Create the PageModel instance
            _pageModel = new IndexPageModel(_unitOfWork, _mockUserManager.Object, _studentDataService);

            // Set up HttpContext for the PageModel
            _pageModel.PageContext = new PageContext
            {
                HttpContext = mockHttpContextAccessor.Object.HttpContext
            };

            // Ensure required entities are present in the database
            var student = new IdentityUser
            {
                Id = "user123",
                UserName = "testuser"
            };
            _context.Users.Add(student);

            var calendarRole = new CalendarRole
            {
                CalendarRoleId = 1,
                Role = "Viewer"
            };
            _context.CalendarRoles.Add(calendarRole);

            _context.SaveChanges();
        }


        [TestMethod]
    public async Task StudentCanEnrollIntoClass()
    {
      // Arrange: Add a class to the context
      var testClass = new Class
      {
        ClassId = 1,
        Title = "Test Class",
        Building = "Building A",
        RoomNumber = "101",
        InstructorId = "instructor123",
        CalendarId = 100
      };
      _context.Classes.Add(testClass);
      await _context.SaveChangesAsync();

      // Act: Attempt to register the student for the class
      IActionResult result = _pageModel.OnPostRegister(1);

      // Assert: Check that the student registration was added
      var registration = _context.StudentRegistrations.FirstOrDefault(r => r.ClassId == 1 && r.StudentId == "user123");
      Assert.IsNotNull(registration, "Student registration should have been added to the database.");
      Assert.IsInstanceOfType(result, typeof(RedirectToPageResult), "Result should be a redirect after successful registration.");
    }

    [TestCleanup]
    public void Cleanup()
    {
      // Ensure the in-memory database is deleted after each test
      _context.Database.EnsureDeleted();
    }

  }
}
