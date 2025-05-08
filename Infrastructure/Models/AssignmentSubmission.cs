using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
  public class AssignmentSubmission
  {
    [Key] public int AssignmentSubmissionId { get; set; }

    [Required] public int? AssignmentId { get; set; }
    [ForeignKey(nameof(AssignmentId))] public Assignment? Assignment { get; set; }

    [Required] public string? StudentId { get; set; }
    [ForeignKey(nameof(StudentId))] public ApplicationUser? Student { get; set; }

    [Required] public bool Submitted { get; set; } = false;

    public string? Submission { get; set; }

    public int? Grade { get; set; } = null;

    [Required] public DateTime? SubmissionDateTime { get; set; }

    public DateTime? GradedOn { get; set; } = null;
  }
}
