using Infrastructure.Interfaces;
using Infrastructure.Models;
using System.Transactions;

namespace DataAccess
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly ApplicationDbContext _dbContext;  //dependency injection of Data Source
    private IGenericRepository<ApplicationUser> _ApplicationUser;
    private IGenericRepository<Class> _Class;
    private IGenericRepository<Department> _Department;
    private IGenericRepository<Location> _Location;
    private IGenericRepository<Calendar> _Calendar;
    private IGenericRepository<CalendarRole> _CalendarRole;
    private IGenericRepository<CalendarAccess> _CalendarUserRole;
    private IGenericRepository<Event> _Event;
    private IGenericRepository<RecurringRule> _RecurringRule;
    private IGenericRepository<ToDo> _ToDo;
    private IGenericRepository<StudentRegistration> _StudentRegistration;
    private IGenericRepository<Assignment> _Assignment;
    private IGenericRepository<AssignmentAttachment> _AssignmentAttachment;
    private IGenericRepository<PaymentTransaction> _PaymentTransaction;
    private IGenericRepository<AssignmentSubmission> _AssignmentSubmission;
    private IGenericRepository<UserSignIns> _UserSignIns;

    private IGenericRepository<CalendarEvent> _CalendarEvent;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public IGenericRepository<ApplicationUser> ApplicationUser
    {
      get
      {
        if (_ApplicationUser == null)
        {
          _ApplicationUser = new GenericRepository<ApplicationUser>(_dbContext);
        }
        return _ApplicationUser;
      }
    }

    public IGenericRepository<CalendarEvent> CalendarEvent
    {
      get
      {
        if (_CalendarEvent == null)
        {
          _CalendarEvent = new GenericRepository<CalendarEvent>(_dbContext);
        }
        return _CalendarEvent;
      }
    }

    public IGenericRepository<Class> Class
    {
      get
      {
        if (_Class == null)
        {
          _Class = new GenericRepository<Class>(_dbContext);
        }
        return _Class;
      }
    }

    public IGenericRepository<Department> Department
    {
      get
      {
        if (_Department == null)
        {
          _Department = new GenericRepository<Department>(_dbContext);
        }
        return _Department;
      }
    }

    public IGenericRepository<Location> Location
    {
      get
      {
        if (_Location == null)
        {
          _Location = new GenericRepository<Location>(_dbContext);
        }
        return _Location;
      }
    }

    public IGenericRepository<Calendar> Calendar
    {
      get
      {
        if (_Calendar == null)
        {
          _Calendar = new GenericRepository<Calendar>(_dbContext);
        }
        return _Calendar;
      }
    }

    public IGenericRepository<CalendarRole> CalendarRole
    {
      get
      {
        if (_CalendarRole == null)
        {
          _CalendarRole = new GenericRepository<CalendarRole>(_dbContext);
        }
        return _CalendarRole;
      }
    }

    public IGenericRepository<CalendarAccess> CalendarUserRole
    {
      get
      {
        if (_CalendarUserRole == null)
        {
          _CalendarUserRole = new GenericRepository<CalendarAccess>(_dbContext);
        }
        return _CalendarUserRole;
      }
    }

    public IGenericRepository<Event> Event
    {
      get
      {
        if (_Event == null)
        {
          _Event = new GenericRepository<Event>(_dbContext);
        }
        return _Event;
      }
    }

    public IGenericRepository<RecurringRule> RecurringRule
    {
      get
      {
        if (_RecurringRule == null)
        {
          _RecurringRule = new GenericRepository<RecurringRule>(_dbContext);
        }
        return _RecurringRule;
      }
    }

    public IGenericRepository<ToDo> ToDo
    {
      get
      {
        if (_ToDo == null)
        {
          _ToDo = new GenericRepository<ToDo>(_dbContext);
        }
        return _ToDo;
      }
    }

    public IGenericRepository<StudentRegistration> StudentRegistration
    {
      get
      {
        if (_StudentRegistration == null)
        {
          _StudentRegistration = new GenericRepository<StudentRegistration>(_dbContext);
        }
        return _StudentRegistration;
      }
    }

    public IGenericRepository<Assignment> Assignment
    {
      get
      {
        if (_Assignment == null)
        {
          _Assignment = new GenericRepository<Assignment>(_dbContext);
        }
        return _Assignment;
      }
    }

    public IGenericRepository<AssignmentAttachment> AssignmentAttachment
    {
      get
      {
        if (_AssignmentAttachment == null)
        {
          _AssignmentAttachment = new GenericRepository<AssignmentAttachment>(_dbContext);
        }
        return _AssignmentAttachment;
      }
    }

    public IGenericRepository<PaymentTransaction> PaymentTransaction
    {
      get
      {
        if (_PaymentTransaction == null)
        {
          _PaymentTransaction = new GenericRepository<PaymentTransaction>(_dbContext);
        }
        return _PaymentTransaction;
      }
    }

    public IGenericRepository<AssignmentSubmission> AssignmentSubmission
    {
      get
      {
        if (_AssignmentSubmission == null)
        {
          _AssignmentSubmission = new GenericRepository<AssignmentSubmission>(_dbContext);
        }
        return _AssignmentSubmission;
      }
    }

    public IGenericRepository<UserSignIns> UserSignIns
    {
      get
      {
        if (_UserSignIns == null)
        {
          _UserSignIns = new GenericRepository<UserSignIns>(_dbContext);
        }
        return _UserSignIns;
      }
    }

    public int Commit()
    {
      return _dbContext.SaveChanges();
    }

    public async Task<int> CommitAsync()
    {
      return await _dbContext.SaveChangesAsync();
    }

    //additional method added for garbage disposal

    public void Dispose()
    {
      _dbContext.Dispose();
    }
  }
}
