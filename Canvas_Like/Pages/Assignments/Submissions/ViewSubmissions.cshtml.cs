using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace Canvas_Like.Pages.Assignments.Submissions
{
    public class ViewSubmissionsModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public IEnumerable<AssignmentSubmission> Submissions { get; set; }
        public Dictionary<int, ApplicationUser> objUser { get; set; }
        public int AssignmentId { get; set; }
        public string AssignmentName {  get; set; }
        public float Points { get; set; }
        public int ClassId { get; set; }

        public class GradeDistribution
        {
            public float GradeRangeStart { get; set; } // Start of the grade range
            public float GradeRangeEnd { get; set; }   // End of the grade range
            public int Count { get; set; }             // Number of students in that range
        }

        public ViewSubmissionsModel(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            Submissions = new List<AssignmentSubmission>();
            objUser = new Dictionary<int, ApplicationUser>();
        }

        public async Task OnGetAsync(int? assignmentId)
        {
            if (assignmentId.HasValue)
            {

                AssignmentId = assignmentId.Value;
                // Fetch the assignment information to get assignment name and maximum points
                var assignObj = await _unitOfWork.Assignment.GetAsync(a => a.AssignmentId == AssignmentId);
                if (assignObj != null)
                {
                    AssignmentName = assignObj.Title;
                    Points = assignObj.Points;
                    ClassId = assignObj.ClassId;
                    Submissions = await _unitOfWork.AssignmentSubmission.GetAllAsync(s => s.AssignmentId == AssignmentId);

                    foreach (var submission in Submissions)
                    {
                        var user = await _unitOfWork.ApplicationUser.GetAsync(c => c.Id == submission.StudentId);
                        objUser[submission.AssignmentSubmissionId] = user;
                    }
                }
            }
            else
            {
                Submissions = Enumerable.Empty<AssignmentSubmission>();
            }
        }

            public IEnumerable<string> UserFullNames
            {
                get
                {
                    return Submissions.Select(submission => objUser.TryGetValue(submission.AssignmentSubmissionId, out var applicationUser)
                        ? $"{applicationUser.FirstName} {applicationUser.LastName}"
                        : "Unknown User");
                }
            }

        public IEnumerable<GradeDistribution> GetGradeDistribution()
        {
            var grades = Submissions
                .Where(s => s.Grade.HasValue)
                .Select(s => (float)s.Grade.Value)
                .ToList();

            int bucketSize = (int)(Points / 10); // Adjust bucket size dynamically based on max points
            if (bucketSize == 0) bucketSize = 1; // Ensure bucket size is at least 1

            if (!grades.Any())
            {
                // Return an empty list if there are no grades
                return new List<GradeDistribution>();
            }

            float maxGrade = grades.Max(); // Get the maximum grade received
            float topRange = Math.Min(maxGrade, Points); // Cap the range at the maximum grade

            var gradeDistribution = Enumerable.Range(0, (int)(topRange / bucketSize) + 1)
                .Select(i => new GradeDistribution
                {
                    GradeRangeStart = i * bucketSize, // Start from 0
                    GradeRangeEnd = Math.Min((i + 1) * bucketSize - 1, topRange), // Adjust end of range to the max grade
                    Count = grades.Count(g => g >= i * bucketSize && g <= (i + 1) * bucketSize - 1) // Inclusive counting
                })
                .ToList();

            return gradeDistribution;
        }






    }
}
// Submissions = await _unitOfWork.AssignmentSubmission.GetAllAsync(s => s.AssignmentId == AssignmentId, includeProperties: "Student");
//@foreach(var objSubmit in Model.Submissions){
//    var user = Model.objUser.TryGetValue(objSubmit.AssignmentSubmissionId, out var applicationUser)
//        ? $"{applicationUser.FirstName} {applicationUser.LastName}"
//        : "Unknown User";