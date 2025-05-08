using System.Security.Claims;
using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Utility;

namespace Canvas_Like.Pages.Assignments.Submissions
{
	public class SubmitAssignmentModel : PageModel
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly UnitOfWork _unitOfWork;

		[BindProperty] public Assignment objAssignment { get; set; }
		[BindProperty] public List<AssignmentAttachment> objAttachments { get; set; }
		[BindProperty] public AssignmentSubmission objSubmission { get; set; }
		[BindProperty] public List<string>? FileUrl { get; set; }
		[BindProperty] public List<string>? FileName { get; set; }
		[BindProperty] public IEnumerable<IFormFile>? UploadFiles { get; set; }
		[BindProperty] public IEnumerable<AssignmentSubmission> Submissions { get; set; }

		public SubmitAssignmentModel(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
			objAssignment = new Assignment();
			objAttachments = new List<AssignmentAttachment>();
			objSubmission = new AssignmentSubmission();
			FileUrl = new List<string>();
			FileName = new List<string>();
			UploadFiles = new List<IFormFile>();
			Submissions = new List<AssignmentSubmission>();
		}

		public async Task<IActionResult> OnGet(int assignmentId)
		{
			string StudentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (StudentId == null)
			{
				Console.Write("Failed to Retrieve Student Info\n");
				return NotFound();
			}

			objAssignment = _unitOfWork.Assignment.GetById(assignmentId);
			if (objAssignment.AssignmentId == null || objAssignment.AssignmentId == 0)
			{
				Console.Write("Failed to Retrieve Assignment Info\n");
				return NotFound();
			}

			Submissions = await _unitOfWork.AssignmentSubmission.GetAllAsync(s => s.AssignmentId == assignmentId);

			// Calculate class average
			double classAverage = Submissions.Count() > 0 ? Submissions.Average(s => s.Grade ?? 0) : 0;

			objSubmission = await _unitOfWork.AssignmentSubmission.GetAsync(
				s => s.AssignmentId == assignmentId && s.StudentId == StudentId);

			if (objSubmission == null)
			{
				objSubmission = new AssignmentSubmission();
				objSubmission.AssignmentId = assignmentId;
				objSubmission.StudentId = StudentId;
				objSubmission.SubmissionDateTime = DateTime.Now;
				_unitOfWork.AssignmentSubmission.Add(objSubmission);
				await _unitOfWork.CommitAsync();
				objSubmission = await _unitOfWork.AssignmentSubmission.GetAsync(
					s => s.AssignmentId == assignmentId && s.StudentId == StudentId);
			}
			else
			{
				if (objAssignment.SubmissionType == SubmissionTypes.File)
				{
					FileDecoder();
				}
			}

			ViewData["ClassAverage"] = classAverage;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var action = Request.Form["action"];
			if (action == "UploadFile")
			{
				FileEncoder();
				return RedirectToPage("/Assignments/Submissions/SubmitAssignment", new { assignmentId = objAssignment.AssignmentId });
			}
			if (!string.IsNullOrEmpty(Request.Form["removeFile"]))
			{
				FileRemover(Request.Form["removeFile"]);
				return RedirectToPage("/Assignments/Submissions/SubmitAssignment", new { assignmentId = objAssignment.AssignmentId });
			}
			if (action == "DeleteDraft")
			{
				DeleteSubmissionDraft();
			}
			else if (action == "SaveDraft")
			{
				SaveSubmissionDraft();
			}
			else if (action == "SubmitAssignment")
			{
				SubmitAssignment();
			}
			return RedirectToPage("/Assignments/Index", new { classId = objAssignment.ClassId });
		}

		//Decodes file Url and Name from the submission when called
		private void FileDecoder()
		{
			if (objSubmission.Submission.IsNullOrEmpty()) return;
			FileUrl = new List<string>();
			FileName = new List<string>();
			List<string> tempList = objSubmission.Submission.Split(';').ToList();
			tempList.RemoveAt(tempList.Count - 1);
			foreach (var file in tempList)
			{
				string[] temp = file.Split(',');
				FileName.Add(temp[0]);
				FileUrl.Add(temp[1]);
			}
		}

		//uploads files to the submission when called and encodes the file into submission
		private void FileEncoder()
		{
			string webRootPath = _webHostEnvironment.WebRootPath;
			foreach (var newFile in UploadFiles)
			{
				if (newFile.Length > 0)
				{
					string fileName = Guid.NewGuid().ToString();
					var uploads = Path.Combine(webRootPath, @"Submissions\");
					var extension = Path.GetExtension(newFile.FileName);
					var fullPath = Path.Combine(uploads, fileName + extension);
					using var fileStream = System.IO.File.Create(fullPath);
					newFile.CopyTo(fileStream);
					objSubmission.Submission += newFile.FileName + "," + @"\Submissions\" + fileName + extension + ";";
				}
			}
			_unitOfWork.AssignmentSubmission.Update(objSubmission);
			_unitOfWork.CommitAsync();
		}

		//removes file from the submission when called
		private void FileRemover(string fileUrl)
		{
			FileDecoder();
			int index = FileUrl.FindIndex(url => url == fileUrl);
			string fileName = FileName[index];
			string webRootPath = _webHostEnvironment.WebRootPath;
			var uploads = Path.Combine(webRootPath, fileUrl.TrimStart('\\'));
			if (System.IO.File.Exists(uploads))
			{
				System.IO.File.Delete(uploads);
			}
			string toRemove = fileName + "," + fileUrl + ";";
			objSubmission.Submission = objSubmission.Submission.Replace(toRemove, "");
			_unitOfWork.AssignmentSubmission.Update(objSubmission);
			_unitOfWork.CommitAsync();
		}

		// Deletes the submission draft
		private void DeleteSubmissionDraft()
		{
			objSubmission = _unitOfWork.AssignmentSubmission.GetById(objSubmission.AssignmentSubmissionId);
			_unitOfWork.AssignmentSubmission.Delete(objSubmission);
			_unitOfWork.CommitAsync();
		}

		// Saves the submission draft for later editing
		private void SaveSubmissionDraft()
		{
			objSubmission.Submitted = false;
			if (objAssignment.SubmissionType == SubmissionTypes.File)
			{
				FileEncoder();
			}
			objSubmission.Grade = null;
			objSubmission.SubmissionDateTime = DateTime.Now;
			_unitOfWork.AssignmentSubmission.Update(objSubmission);
			_unitOfWork.CommitAsync();
		}

		// Submits the assignment
		private void SubmitAssignment()
		{
			objSubmission.Submitted = true;
			if (objAssignment.SubmissionType == SubmissionTypes.File)
			{
				FileEncoder();
			}
			objSubmission.Grade = null;
			objSubmission.SubmissionDateTime = DateTime.Now;
			_unitOfWork.AssignmentSubmission.Update(objSubmission);
			_unitOfWork.CommitAsync();
		}
	}
}

