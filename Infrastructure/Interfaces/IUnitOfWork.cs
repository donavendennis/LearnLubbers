using Infrastructure.Models;

namespace Infrastructure.Interfaces
{
  public interface IUnitOfWork
  {
    //ADD Models/Tables here as you create them so UnitOfWork will have access
    public IGenericRepository<ApplicationUser> ApplicationUser { get; }
    public IGenericRepository<Calendar> Calendar { get; }
    public IGenericRepository<Location> Location { get; }
    public IGenericRepository<Department> Department { get; }
    public IGenericRepository<Class> Class { get; }
    public IGenericRepository<CalendarRole> CalendarRole { get; }
    public IGenericRepository<CalendarAccess> CalendarUserRole { get; }
    public IGenericRepository<RecurringRule> RecurringRule { get; }
    public IGenericRepository<Event> Event { get; }
    public IGenericRepository<ToDo> ToDo { get; }
    public IGenericRepository<StudentRegistration> StudentRegistration { get; }
    public IGenericRepository<Assignment> Assignment { get; }
    public IGenericRepository<AssignmentAttachment> AssignmentAttachment { get; }
    public IGenericRepository<PaymentTransaction> PaymentTransaction { get; }
    public IGenericRepository<AssignmentSubmission> AssignmentSubmission { get; }
    public IGenericRepository<UserSignIns> UserSignIns { get; }


    //To Be Deleted
    public IGenericRepository<CalendarEvent> CalendarEvent { get; }
    int Commit();

    Task<int> CommitAsync();
  }
}
