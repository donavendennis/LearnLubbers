using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
  public class ApplicationUser : IdentityUser
  {
    [Required][DisplayName("First Name")] public string? FirstName { get; set; }

    [Required][DisplayName("Last Name")] public string? LastName { get; set; }

    [Required][DisplayName("Date Of Birth")] public DateTime DateOfBirth { get; set; }
    public string FullName { get { return FirstName + " " + LastName; } }

    public Location? Location { get; set; }
    [ForeignKey(nameof(Location))] public int? LocationId { get; set; }

    // Navigation property for the registrations (one-to-many)
    public ICollection<StudentRegistration> Registrations { get; set; }

    public string? ProfilePictureUrl { get; set; }
  }
}
