using System.Security.Policy;
using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Utility;

namespace Canvas_Like.Pages.Assignments
{
	public class UpsertModel : PageModel
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly UnitOfWork _unitOfWork;

		[BindProperty] public Assignment objAssignment { get; set; }
		[BindProperty] public List<AssignmentAttachment> objAttachments { get; set; }
		[BindProperty] public ToDo objToDo { get; set; }
		[BindProperty] public IEnumerable<IFormFile>? UploadFiles { get; set; }

		public UpsertModel(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
			objAssignment = new Assignment();
			objAttachments = new List<AssignmentAttachment>();
			objToDo = new ToDo();
			UploadFiles = new List<IFormFile>();
		}

		public async Task<IActionResult> OnGet(int ClassId, int? AssignmentId)
		{
			if (AssignmentId.HasValue)
			{
				objAssignment = _unitOfWork.Assignment.GetById(AssignmentId.Value);
				if (objAssignment == null)
				{
					return NotFound();
				}

				objAttachments = _unitOfWork.AssignmentAttachment
					.GetAll(a => a.AssignmentId == objAssignment.AssignmentId).ToList();
				objToDo = _unitOfWork.ToDo.Get(t => t.ToDoId == objAssignment.ToDoId);
			}
			else
			{
				int? calendarId = _unitOfWork.Class.GetById(ClassId).CalendarId;
				objToDo.CalendarId = calendarId;
				objToDo.Completed = false;
				objToDo.DueDate = DateOnly.FromDateTime(DateTime.Today).ToDateTime(TimeOnly.Parse("11:59:00 PM"));
				objToDo.Title = " ";
				objToDo.Description = " ";
				_unitOfWork.ToDo.Add(objToDo);
				objAssignment.ClassId = ClassId;
				objAssignment.ToDoId = objToDo.ToDoId;
				objAssignment.DueDateTime = objToDo.DueDate;
				objAssignment.Title = " ";
				objAssignment.Description = " ";
				objAssignment.SubmissionType = SubmissionTypes.Txt;
				objAssignment.Points = 100;
				objAssignment.Published = false;
				_unitOfWork.Assignment.Add(objAssignment);
			}
			await _unitOfWork.CommitAsync();
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{

			var action = Request.Form["action"];
			if (action == "Cancel")
			{
				Cancel();
				return RedirectToPage("./Index", new { classId = objAssignment.ClassId });
			}

			// Update ToDo
			objToDo = _unitOfWork.ToDo.GetById(objAssignment.ToDoId);
			objToDo.DueDate = objAssignment.DueDateTime;
			objToDo.Title = objAssignment.Title;
			objToDo.Description = objAssignment.Description;
			_unitOfWork.ToDo.Update(objToDo);

			_unitOfWork.Assignment.Update(objAssignment);

			if (action == "Publish")
			{
				Publish();
				return RedirectToPage("./Index", new { classId = objAssignment.ClassId });
			}
			
			if (!string.IsNullOrEmpty(Request.Form["deleteId"]))
			{
				var attachmentId = int.Parse(Request.Form["deleteId"]);
				Delete(attachmentId);
				return RedirectToPage(new { _unitOfWork.Assignment.GetById(objAssignment.AssignmentId).ClassId, objAssignment.AssignmentId });
			}
			Upload();
			return RedirectToPage(new { _unitOfWork.Assignment.GetById(objAssignment.AssignmentId).ClassId, objAssignment.AssignmentId });
		}

		private void Publish()
		{

			if (!objAssignment.Published) objAssignment.Published = true;
			_unitOfWork.Assignment.Update(objAssignment);

			// Permanently Deleted Attachments
			string webRootPath = _webHostEnvironment.WebRootPath;
			objAttachments = _unitOfWork.AssignmentAttachment.GetAll(a => a.AssignmentId == objAssignment.AssignmentId).ToList();
			foreach (var attachment in objAttachments)
			{
				if (!attachment.Keep)
				{
					var imagePath = Path.Combine(webRootPath, attachment.FileUrl.TrimStart('\\'));
					if (System.IO.File.Exists(imagePath))
					{
						System.IO.File.Delete(imagePath);
					}
					_unitOfWork.AssignmentAttachment.Delete(attachment);
				}
				else if (!attachment.KeepPerminant)
				{
					attachment.KeepPerminant = true;
					_unitOfWork.AssignmentAttachment.Update(attachment);
				}
			}

			// upload new AssignmentAttachments
			var files = HttpContext.Request.Form.Files;
			foreach (var newFile in files)
			{
				if (newFile.Length > 0)
				{
					string fileName = Guid.NewGuid().ToString();
					var uploads = Path.Combine(webRootPath, @"attachments\");
					var extension = Path.GetExtension(newFile.FileName);
					var fullPath = Path.Combine(uploads, fileName + extension);
					using var fileStream = System.IO.File.Create(fullPath);
					newFile.CopyTo(fileStream);
					AssignmentAttachment attachment = new AssignmentAttachment
					{
						AssignmentId = objAssignment.AssignmentId,
						FileName = newFile.FileName,
						FileUrl = @"\attachments\" + fileName + extension,
						Keep = true,
						KeepPerminant = true
					};
					_unitOfWork.AssignmentAttachment.Add(attachment);
				}
			}
			_unitOfWork.CommitAsync();
		}

		private void Upload()
		{
			string webRootPath = _webHostEnvironment.WebRootPath;
			foreach (var newFile in UploadFiles)
			{
				if (newFile.Length > 0)
				{
					string fileName = Guid.NewGuid().ToString();
					var uploads = Path.Combine(webRootPath, @"attachments\");
					var extension = Path.GetExtension(newFile.FileName);
					var fullPath = Path.Combine(uploads, fileName + extension);
					using var fileStream = System.IO.File.Create(fullPath);
					newFile.CopyTo(fileStream);
					AssignmentAttachment attachment = new AssignmentAttachment
					{
						AssignmentId = objAssignment.AssignmentId,
						FileName = newFile.FileName,
						FileUrl = @"\attachments\" + fileName + extension,
						Keep = true,
						KeepPerminant = false
					};
					_unitOfWork.AssignmentAttachment.Add(attachment);
				}
			}
			_unitOfWork.CommitAsync();
		}

		private void Delete(int id)
		{
			AssignmentAttachment attachment = _unitOfWork.AssignmentAttachment.GetById(id);
			attachment.Keep = false;
			_unitOfWork.AssignmentAttachment.Update(attachment);
			_unitOfWork.CommitAsync();
		}

		private void Cancel()
		{
			objAssignment = _unitOfWork.Assignment.GetById(objAssignment.AssignmentId);
			objToDo = _unitOfWork.ToDo.GetById(objAssignment.ToDoId);
			objAttachments = _unitOfWork.AssignmentAttachment.GetAll(a => a.AssignmentId == objAssignment.AssignmentId).ToList();

			string webRootPath = _webHostEnvironment.WebRootPath;
			if (objAssignment.Published)
			{
				foreach (var attachment in objAttachments)
				{
					if (!attachment.KeepPerminant)
					{
						var imagePath = Path.Combine(webRootPath, attachment.FileUrl.TrimStart('\\'));
						if (System.IO.File.Exists(imagePath))
						{
							System.IO.File.Delete(imagePath);
						}
						_unitOfWork.AssignmentAttachment.Delete(attachment);
					}
					else if (attachment.KeepPerminant && !attachment.Keep)
					{
						attachment.Keep = true;
						_unitOfWork.AssignmentAttachment.Update(attachment);
					}
				}
			}
			else
			{
				foreach (var attachment in objAttachments)
				{
					var imagePath = Path.Combine(webRootPath, attachment.FileUrl.TrimStart('\\'));
					if (System.IO.File.Exists(imagePath))
					{
						System.IO.File.Delete(imagePath);
					}
					_unitOfWork.AssignmentAttachment.Delete(attachment);
				}
				_unitOfWork.Assignment.Delete(objAssignment);
				_unitOfWork.ToDo.Delete(objToDo);
			}
			_unitOfWork.CommitAsync();
		}
	}
}
