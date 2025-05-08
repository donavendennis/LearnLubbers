using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Moq;
using DataAccess;
using Infrastructure.Models;
using Canvas_Like.Pages.Payment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Linq;
using DinkToPdf.Contracts;

namespace Canvas_Like.Tests.UnitTests
{
    [TestClass]
    public class StudentPaymentTest
    {
        private ApplicationDbContext _context;
        private UnitOfWork _unitOfWork;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private IndexModel _pageModel;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IConverter> _mockConverter;

        private IConfiguration Configuration => _mockConfiguration.Object;

        [TestInitialize]
        public void Setup()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _unitOfWork = new UnitOfWork(_context);
            _mockConverter = new Mock<IConverter>();

            // Mock UserManager
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null
            );

            // Mock UserManager to return a user ID
            _mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("user123");

            // Mock IConfiguration
            _mockConfiguration = new Mock<IConfiguration>();
            // Set up the Stripe keys
            _mockConfiguration.SetupGet(x => x["Stripe:SecretKey"]).Returns("private_test_key");
            _mockConfiguration.SetupGet(x => x["Stripe:PublishableKey"]).Returns("public_test_key");

            // Create the PageModel instance
            _pageModel = new IndexModel(_unitOfWork, _mockUserManager.Object, _mockConfiguration.Object, _mockConverter.Object);

            // Set up HttpContext for the PageModel
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user123")
                }))
            };
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
        }

        [TestMethod]
        public async Task StudentCanMakeTuitionPayment()
        {
            // Arrange: Add a student user
            var studentUser = new IdentityUser
            {
                Id = "user123",
                UserName = "teststudent@example.com",
                Email = "teststudent@example.com"
            };
            _context.Users.Add(studentUser);
            _context.SaveChanges();

            // Arrange: Add classes
            var testClass1 = new Class
            {
                ClassId = 1,
                Title = "Math 101",
                CreditHours = 3,
                Building = "Building A",
                RoomNumber = "101",
                InstructorId = "instructor123",
                CalendarId = 100
            };

            var testClass2 = new Class
            {
                ClassId = 2,
                Title = "English 101",
                CreditHours = 4,
                Building = "Building B",
                RoomNumber = "202",
                InstructorId = "instructor456",
                CalendarId = 200
            };

            _context.Classes.AddRange(testClass1, testClass2);
            _context.SaveChanges();

            // Enroll the student in classes
            var studentRegistration1 = new StudentRegistration
            {
                ClassId = 1,
                StudentId = "user123"
            };

            var studentRegistration2 = new StudentRegistration
            {
                ClassId = 2,
                StudentId = "user123"
            };

            _context.StudentRegistrations.AddRange(studentRegistration1, studentRegistration2);
            _context.SaveChanges();

            // Act: Call OnPostPayAsync
            var paymentRequest = new IndexModel.PaymentRequest
            {
                Amount = 200M,
                Token = "tok_visa"
            };

            var result = await _pageModel.OnPostPayAsync(paymentRequest);

            // Assert: Check that a PaymentTransaction was added
            var paymentTransaction = _context.PaymentTransactions.FirstOrDefault(pt => pt.StudentId == "user123" && pt.Amount == 200M);

            Assert.IsNotNull(paymentTransaction, "PaymentTransaction should have been added to the database.");
            Assert.AreEqual(200M, paymentTransaction.Amount, "Payment amount should be 200.");
            Assert.IsNotNull(paymentTransaction.TransactionDate, "TransactionDate should not be null.");

            // Assert: Check that the success message is set
            Assert.IsFalse(string.IsNullOrEmpty(_pageModel.Success), "Success message should be set.");
            Assert.IsTrue(_pageModel.Success.Contains("Payment of $200"), "Success message should indicate the payment amount.");

            // Assert: Check that the remaining tuition cost is calculated correctly
            // Total tuition: (3 + 4) * $100 = $700
            // Remaining balance: $700 - $200 = $500
            Assert.AreEqual(500M, _pageModel.tuitionCost, "Remaining tuition cost should be $500.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Ensure the in-memory database is deleted after each test
            _context.Database.EnsureDeleted();
        }
    }
}
