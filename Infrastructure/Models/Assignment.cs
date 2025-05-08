using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
  public class Assignment
  {
    [Key]
    public int AssignmentId { get; set; }

    [Required]
    public int ClassId { get; set; }

    [ForeignKey(nameof(ClassId))]
    public Class? Class { get; set; }

    [Required]
    public int ToDoId { get; set; }

    [ForeignKey(nameof(ToDoId))]
    public ToDo? ToDo { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime DueDateTime { get; set; }

    [Required]
    [StringLength(1000)]
    public string Description { get; set; }

    [Required]
    [StringLength(50)]
    public string? SubmissionType { get; set; }

    [Required]
    [Range(0, 300)]
    public float Points { get; set; }

    [Required]
    public bool Published { get; set; } = false;

    public DateTime DateCreated { get; private set; }
    public Assignment()
    {
      DateCreated = DateTime.UtcNow;
    }
  }
}
