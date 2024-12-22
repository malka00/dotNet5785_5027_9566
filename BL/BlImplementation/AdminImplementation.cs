using BlApi;
using DalTest;
using Helpers;
namespace BlImplementation;


/// <summary>
/// Implementation of the methods of admin
/// </summary>
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;  //stage 4

    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5
    public void Definition(TimeSpan time)
    {
        //_dal.ResetDB();
        //Initialization.Do();
        //ClockManager.UpdateClock(ClockManager.Now);
        AdminManager.MaxRange= time;
    }

    public void ForwardClock(BO.TimeUnit unit) => AdminManager.UpdateClock(unit switch
    {
        BO.TimeUnit.MINUTE => AdminManager.Now.AddMinutes(1),
        BO.TimeUnit.HOUR => AdminManager.Now.AddHours(1),
        BO.TimeUnit.DAY => AdminManager.Now.AddDays(1),
        BO.TimeUnit.MONTH => AdminManager.Now.AddMonths(1),
        BO.TimeUnit.YEAR => AdminManager.Now.AddYears(1),
        _ => DateTime.MinValue
    });
  
    public DateTime GetClock() => AdminManager.Now; 
   
    public TimeSpan GetMaxRange()
    {
       return AdminManager.MaxRange; ;
    }

    public void initialization()
    {
        DalTest.Initialization.Do(); 
        AdminManager.UpdateClock(AdminManager.Now);  
    }

    public void Reset()
    {
        _dal.ResetDB(); 
        AdminManager.UpdateClock(AdminManager.Now);
    }
}
