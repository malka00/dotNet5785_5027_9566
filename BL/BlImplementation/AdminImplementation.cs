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

    /// <summary>
    /// Initializes the risk range time
    /// </summary>
    /// <param name="time"></param>
    public void setMaxRange(TimeSpan time)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
       
        AdminManager.MaxRange= time;
        VolunteerManager.Observers.NotifyListUpdated();
        CallManager.Observers.NotifyListUpdated();
    }

    /// <summary>
    /// A function to advance the clock according to the selected time unit
    /// </summary>
    /// <param name="unit"></param>
    public void ForwardClock(BO.TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.UpdateClock(unit switch
        {
            BO.TimeUnit.MINUTE => AdminManager.Now.AddMinutes(1),
            BO.TimeUnit.HOUR => AdminManager.Now.AddHours(1),
            BO.TimeUnit.DAY => AdminManager.Now.AddDays(1),
            BO.TimeUnit.MONTH => AdminManager.Now.AddMonths(1),
            BO.TimeUnit.YEAR => AdminManager.Now.AddYears(1),
            _ => DateTime.MinValue
        });

        VolunteerManager.Observers.NotifyListUpdated();
        CallManager.Observers.NotifyListUpdated();
    }

    /// <summary>
    /// Receives an interval and starts the simulator according to it
    /// </summary>
    /// <param name="interval"></param>
    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }

    /// <summary>
    /// Stops the simulator
    /// </summary>
    public void StopSimulator()
        => AdminManager.Stop(); //stage 7

    /// <summary>
    /// Returns the current time to system clock bins
    /// </summary>
    /// <returns> DateTime </returns>
    public DateTime GetClock() => AdminManager.Now;

    /// <summary>
    /// Returns a risk range
    /// </summary>
    /// <returns> TimeSpan </returns>
    public TimeSpan GetMaxRange()
    {
        return AdminManager.MaxRange; 
    }

    /// <summary>
    /// System initialization
    /// </summary>
    public void initialization()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7
    }

    /// <summary>
    /// system reset
    /// </summary>
    public void Reset()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7
    }
}
