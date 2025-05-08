using Microsoft.EntityFrameworkCore;
using DataAccess;
using Infrastructure.Models;
using Canvas_Like.Pages.Assignments.Grading;
using Microsoft.AspNetCore.Mvc;

namespace Canvas_Like.Tests.UnitTests
{
  [TestClass]
  public class InstructorGradingAssignmentTest
  {
    private ApplicationDbContext _context;
    private UnitOfWork _unitOfWork;
    private IndexModel _pageModel;

    [TestInitialize]
    public void Setup()
    {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: "TestDatabase")
          .Options;

      _context = new ApplicationDbContext(options);
      _unitOfWork = new UnitOfWork(_context);

      _pageModel = new IndexModel(_unitOfWork);
    }

    [TestMethod]
    public async Task InstructorCanGradeAssignment()
    {
      var assignmentSubmission1 = new AssignmentSubmission
      {
        AssignmentSubmissionId = 1,
        AssignmentId = 1,
        StudentId = "student123",
        Submitted = true,
        SubmissionDateTime = new System.DateTime()
      };
      _context.AssignmentSubmissions.Add(assignmentSubmission1);
      await _context.SaveChangesAsync();

      _pageModel.assignmentSubmission = assignmentSubmission1;

      var prevGrade = _pageModel.assignmentSubmission.Grade;
      _pageModel.assignmentSubmission.Grade = 50;
      IActionResult result = _pageModel.OnPost();

      var assignmentSubmission = _context.AssignmentSubmissions.FirstOrDefault(
          a => a.AssignmentSubmissionId == 1);

      Assert.IsNotNull(assignmentSubmission,
          "Assignment Submission should have been added to the database.");
      Assert.AreNotEqual(prevGrade, assignmentSubmission.Grade,
          "Assignment grade should have changed");
      Assert.AreEqual(50, assignmentSubmission.Grade,
          "Assignment grade should be 50");
    }

    [TestCleanup]
    public void Cleanup()
    {
      // Ensure the in-memory database is deleted after each test
      _context.Database.EnsureDeleted();
    }

  }
}
