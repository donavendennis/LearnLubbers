using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Models;
using DataAccess;

namespace Canvas_Like.Pages.Assignments.Grading
{
  public class IndexModel : PageModel
  {
    private readonly UnitOfWork _unitOfWork;
    [BindProperty]
    public AssignmentSubmission? assignmentSubmission { get; set; }
    public Assignment? assignment;
    private int? id;

    public List<string>? FileUrl;
    public List<string>? FileName;

    [BindProperty] public int? score { get; set; }

    public IndexModel(UnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    public void OnGet(int? submissionId)
    {
      id = submissionId;
      assignmentSubmission = _unitOfWork.AssignmentSubmission
        .GetById(submissionId);
      assignment = _unitOfWork.Assignment
        .GetById(assignmentSubmission.AssignmentId);

      FileDecoder();
    }

    public IActionResult OnPost()
    {
      if (assignmentSubmission != null)
      {
        AssignmentSubmission submission = _unitOfWork.AssignmentSubmission
          .GetById(assignmentSubmission.AssignmentSubmissionId);

        submission.Grade = assignmentSubmission.Grade;
        submission.GradedOn = DateTime.UtcNow;

        _unitOfWork.AssignmentSubmission.Update(submission);
        _unitOfWork.Commit();
      }

      return RedirectToPage();
    }


    private void FileDecoder()
    {
      Console.WriteLine(assignmentSubmission.Submission);
      if (assignmentSubmission.Submission == null) return;
      FileUrl = new List<string>();
      FileName = new List<string>();
      List<string> tempList = assignmentSubmission.Submission.Split(';').ToList();
      tempList.RemoveAt(tempList.Count - 1);
      foreach (var file in tempList)
      {
        string[] temp = file.Split(',');
        FileName.Add(temp[0]);
        FileUrl.Add(temp[1]);
      }
    }
  }
}
