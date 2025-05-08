using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Canvas_Like.Pages.Assignments
{
	public class IndexModel : PageModel
	{
		private readonly UnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

		[BindProperty]
		public IEnumerable<Assignment> AssignmentDetails { get; set; } = new List<Assignment>();

		[BindProperty]
		public int ClassId { get; set; }

		public string ClassName { get; set; }
		public string FinalGrade { get; set; }
		public int FinalScore { get; set; }
		public double ClassAverage { get; set; }

		public IndexModel(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task OnGetAsync(int? classId)
		{
			if (classId.HasValue)
			{
				ClassId = classId.Value;

				// Fetch the class information to get the class name
				var classObj = await _unitOfWork.Class.GetAsync(c => c.ClassId == classId.Value);
				if (classObj != null)
				{
					ClassName = classObj.Title;  // Set the class title dynamically
				}

				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				AssignmentDetails = await _unitOfWork.Assignment.GetAllAsync(a => a.ClassId == classId.Value);
				var submissions = await _unitOfWork.AssignmentSubmission.GetAllAsync(s => s.StudentId == userId && s.Assignment.ClassId == classId.Value);

				// Calculate the final grade for the student
				FinalGrade = CalculateFinalGrade(submissions, AssignmentDetails);

				// Calculate the class average
				ClassAverage = await CalculateClassAverage(classId.Value);
			}
			else
			{
				AssignmentDetails = Enumerable.Empty<Assignment>();
				FinalGrade = "N/A";
			}
		}

		public async Task<IActionResult> OnPostDeleteAsync(int id)
		{
			Assignment assignment = _unitOfWork.Assignment.GetById(id);
			ToDo toDo = _unitOfWork.ToDo.GetById(assignment.ToDoId);
			List<AssignmentAttachment> attachments = _unitOfWork.AssignmentAttachment.GetAll(a => a.AssignmentId == id).ToList();

			string webRootPath = _webHostEnvironment.WebRootPath;

			foreach (var attachment in attachments)
			{
				var imagePath = Path.Combine(webRootPath, attachment.FileUrl.TrimStart('\\'));
				if (System.IO.File.Exists(imagePath))
				{
					System.IO.File.Delete(imagePath);
				}

				_unitOfWork.AssignmentAttachment.Delete(attachment);
			}

			_unitOfWork.Assignment.Delete(assignment);
			_unitOfWork.ToDo.Delete(toDo);
			return RedirectToPage(new { classId = assignment.ClassId });
		}

		public string CalculateFinalGrade(IEnumerable<AssignmentSubmission> submissions, IEnumerable<Assignment> assignments)
		{
			// Calculate total points earned by the student
			float totalPointsEarned = submissions.Sum(s => s.Grade ?? 0);

			// Calculate total points possible for assignments
			float totalPointsPossible = assignments.Sum(a => a.Points);

			if (totalPointsPossible == 0)
			{
				FinalScore = 0;
				return "N/A";  // No assignments available
			}

			float percentage = (totalPointsEarned / totalPointsPossible) * 100;
			FinalScore = (int)Math.Round(percentage);

			// Apply the grading scale
			if (percentage >= 94) return "A";
			else if (percentage >= 90) return "A-";
			else if (percentage >= 87) return "B+";
			else if (percentage >= 84) return "B";
			else if (percentage >= 80) return "B-";
			else if (percentage >= 77) return "C+";
			else if (percentage >= 74) return "C";
			else if (percentage >= 70) return "C-";
			else if (percentage >= 67) return "D+";
			else if (percentage >= 64) return "D";
			else if (percentage >= 60) return "D-";
			else return "E";
		}

		private async Task<double> CalculateClassAverage(int classId)
		{
			var assignments = await _unitOfWork.Assignment.GetAllAsync(a => a.ClassId == classId);
			var allSubmissions = await _unitOfWork.AssignmentSubmission.GetAllAsync(s => s.Assignment.ClassId == classId);

			double totalPointsEarned = allSubmissions.Sum(s => s.Grade ?? 0);
			float totalPointsPossible = assignments.Sum(a => a.Points);

			if (totalPointsPossible == 0)
			{
				return 0;
			}

			return (totalPointsEarned / totalPointsPossible) * 100;
		}
	}
}

