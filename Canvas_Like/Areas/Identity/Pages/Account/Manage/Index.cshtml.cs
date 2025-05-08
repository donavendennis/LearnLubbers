// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;

namespace Canvas_Like.Areas.Identity.Pages.Account.Manage 
{
	public class IndexModel : PageModel
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly ApplicationDbContext _context;

		public IndexModel(
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager,
			ApplicationDbContext context,
			IWebHostEnvironment webHostEnvironment)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[TempData]
		public string StatusMessage { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
		public InputModel Input { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel
		{

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[Display(Name = "First Name")]
			public string FirstName { get; set; }

			[Required]
			[Display(Name = "Last Name")]
			public string LastName { get; set; }

			[Required]
			[Display(Name = "Date of Birth")]
			[DataType(DataType.Date)]
			public DateTime DateOfBirth { get; set; }

			[Phone]
			[Display(Name = "Phone number")]
			public string PhoneNumber { get; set; }

			[BindProperty]
			[Display(Name = "Profile Picture")]
			public string? ProfilePictureUrl { get; set; }

			public LocationInputModel Location { get; set; } = new LocationInputModel();

			public class LocationInputModel()
			{
				public string Alias { get; set; }
				public string Street1 { get; set; }
				public string? Street2 { get; set; }
				public string City { get; set; }
				public string State { get; set; }
				public string PostalCode { get; set; }
			}
		}

		private async Task LoadAsync(ApplicationUser user)
		{
			var userName = await _userManager.GetUserNameAsync(user);
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);


			Console.WriteLine("Loading Location Data:");
			Console.WriteLine($"Street1: {user.Location?.Street1}, City: {user.Location?.City}");

			Username = userName;

			Input = new InputModel
			{
				PhoneNumber = phoneNumber,
				FirstName = user.FirstName,
				LastName = user.LastName,
				DateOfBirth = user.DateOfBirth,
				ProfilePictureUrl = user.ProfilePictureUrl,
				Location = user.Location != null ? new InputModel.LocationInputModel
				{
					Street1 = user.Location.Street1,
					Street2 = user.Location.Street2,
					City = user.Location.City,
					State = user.Location.State,
					PostalCode = user.Location.PostalCode
				}
				: new InputModel.LocationInputModel()
			};
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.Users
				.OfType<ApplicationUser>()
				.Include(u => u.Location)
				.FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			await LoadAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var user = await _userManager.GetUserAsync(User) as ApplicationUser;
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (!ModelState.IsValid)
			{
				await LoadAsync(user);
				return Page();
			}
			//Profile Picture Upload
			string webRootPath = _webHostEnvironment.WebRootPath;
			var files = HttpContext.Request.Form.Files;
			if (files.Count > 0)
			{
				string fileName = Guid.NewGuid().ToString();
				var uploads = Path.Combine(webRootPath, @"images\profile_pictures");
				var extension = Path.GetExtension(files[0].FileName);
				if (user.ProfilePictureUrl != null)
				{
					var imagePath = Path.Combine(webRootPath, user.ProfilePictureUrl.TrimStart('\\'));
					if (System.IO.File.Exists(imagePath))
					{
						System.IO.File.Delete(imagePath);
					}
				}
				var fullPath = Path.Combine(uploads, fileName + extension);
				using var fileStream = System.IO.File.Create(fullPath);
				files[0].CopyTo(fileStream);
				user.ProfilePictureUrl = @"/images/profile_pictures/" + fileName + extension;

                Console.WriteLine("Full path: " + fullPath);
            }
            else
			{
                
                user.ProfilePictureUrl = Input.ProfilePictureUrl;
            }


			user.FirstName = Input.FirstName;
			user.LastName = Input.LastName;
			user.DateOfBirth = Input.DateOfBirth;


			if (user.LocationId.HasValue)
			{
				// User already has a location, so update the existing location
				var existingLocation = await _context.Locations.FindAsync(user.LocationId.Value);
				if (existingLocation != null)
				{
					existingLocation.Street1 = Input.Location.Street1;
					existingLocation.Street2 = Input.Location.Street2;
					existingLocation.City = Input.Location.City;
					existingLocation.State = Input.Location.State;
					existingLocation.PostalCode = Input.Location.PostalCode;
					existingLocation.Alias = "Home"; // Hardcoded alias

					_context.Update(existingLocation); // Update the existing location
				}
			}
			else
			{
				// User doesn't have a location, so create a new one
				var newLocation = new Location
				{
					Street1 = Input.Location.Street1,
					Street2 = Input.Location.Street2,
					City = Input.Location.City,
					State = Input.Location.State,
					PostalCode = Input.Location.PostalCode,
					Alias = "Home" // Hardcoded alias
				};

				_context.Locations.Add(newLocation); // Add the new location to the database
				await _context.SaveChangesAsync(); // Save to generate LocationId

				// Assign the newly created location to the user
				user.LocationId = newLocation.LocationID;
			}

			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			if (Input.PhoneNumber != phoneNumber)
			{
				var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
				if (!setPhoneResult.Succeeded)
				{
					StatusMessage = "Unexpected error when trying to set phone number.";
					return RedirectToPage();
				}
			}

			await _userManager.UpdateAsync(user);
			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated";
			return RedirectToPage();
		}
	}
}