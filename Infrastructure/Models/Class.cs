using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
  public class Class
  {
    [Key]
    public int ClassId { get; set; }


    [Required]
    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [Required]
    [Display(Name = "Instructor")]
    public string? InstructorId { get; set; }

    [Required, StringLength(10)]
    public String? Building { get; set; }
    [Required, StringLength(10)]
    public String? RoomNumber { get; set; }

    [Required]
    [Display(Name = "Calendar")]
    public int? CalendarId { get; set; }

    private int _coursenNumber;
    [Required, DisplayName("Course Number")]
    public int CourseNumber
    {
      get => _coursenNumber == 0 ? 1000 : _coursenNumber;
      set => _coursenNumber = value;
    }

    [Required, StringLength(100)]
    public string? Title { get; set; }

    private int _creditHours;

    [Required, Range(0, 20)]
    public int CreditHours
    {
      get => _creditHours == 0 ? 3 : _creditHours;
      set => _creditHours = value;
    }

    private int _capacity;

    [Required, Range(0, 100)]
    public int Capacity
    {
      get => _capacity == 0 ? 30 : _capacity;
      set => _capacity = value;
    }

    [Required, DisplayName("Start Date"), DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required, DisplayName("End Date"), DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [ForeignKey("DepartmentId")]
    public Department? Department { get; set; }

    [ForeignKey("CalendarId")]
    public Calendar? Calendar { get; set; }

    [ForeignKey("InstructorId")]
    public ApplicationUser? Instructor { get; set; }

    // Navigation property for the registrations (one-to-many)
    public ICollection<StudentRegistration> Registrations { get; set; }
  }
}
