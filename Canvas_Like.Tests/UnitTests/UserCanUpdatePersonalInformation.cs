using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Canvas_Like.Areas.Identity.Pages.Account.Manage;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

namespace Canvas_Like.Tests.UnitTests
{
    [TestClass]
    public class UserUpdatePersonalInformationTest
    {
        private Mock<UserManager<IdentityUser>> _mockUserManager = null!;
        private Mock<SignInManager<IdentityUser>> _mockSignInManager = null!;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment = null!;
        private ApplicationDbContext _context = null!;
        private IndexModel _pageModel = null!;

        public UserUpdatePersonalInformationTest()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<IdentityUser>>();
            var userValidators = new List<IUserValidator<IdentityUser>> { new Mock<IUserValidator<IdentityUser>>().Object };
            var passwordValidators = new List<IPasswordValidator<IdentityUser>> { new Mock<IPasswordValidator<IdentityUser>>().Object };
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<IdentityUser>>>();

            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                store.Object, options.Object, passwordHasher.Object, userValidators, passwordValidators, keyNormalizer.Object, errors.Object, services.Object, logger.Object);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            var loggerSignIn = new Mock<ILogger<SignInManager<IdentityUser>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<IdentityUser>>();

            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
                _mockUserManager.Object, contextAccessor.Object, userPrincipalFactory.Object, options.Object, loggerSignIn.Object, schemes.Object, confirmation.Object);

            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(dbContextOptions);

            _pageModel = new IndexModel(_mockUserManager.Object, _mockSignInManager.Object, _context, _mockWebHostEnvironment.Object);
        }

        [TestInitialize]
        public void SetUp()
        {
            var user = new ApplicationUser
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "1234567890",
                ProfilePictureUrl = "/images/profile_pictures/test.jpg"
            };
            _mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(IdentityResult.Success);

            // Mock HttpContext
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            var mockForm = new Mock<IFormCollection>();
            var mockFiles = new Mock<IFormFileCollection>();

            mockForm.Setup(f => f.Files).Returns(mockFiles.Object);
            mockRequest.Setup(r => r.Form).Returns(mockForm.Object);
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

            _pageModel.PageContext.HttpContext = mockHttpContext.Object;
        }

        [TestMethod]
        public async Task OnPostAsync_UpdatePersonalInformation_Success()
        {
            // Arrange
            var user = new ApplicationUser { Id = "testuser", PhoneNumber = "1234567890" };
            _mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.GetPhoneNumberAsync(user)).ReturnsAsync("1234567890");
            _mockUserManager.Setup(u => u.SetPhoneNumberAsync(user, "0987654321")).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            _pageModel.Input = new IndexModel.InputModel
            {
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = DateTime.Now,
                PhoneNumber = "0987654321",
                Location = new IndexModel.InputModel.LocationInputModel
                {
                    Street1 = "123 Main St",
                    City = "Test City",
                    State = "TS",
                    PostalCode = "12345"
                }
            };

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            _mockUserManager.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _mockUserManager.Reset();
            _mockSignInManager.Reset();
            _mockWebHostEnvironment.Reset();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
