

using BlApi;
using BO;
using Helpers;


namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Definition(TimeSpan time)
    {
     
      //  DalApi.IConfig.ConfigImplementation.config.RiskRange = time;

    }

    public void ForwardClock(BO.TimeUnit unit) => ClockManager.UpdateClock(unit switch
    {
        BO.TimeUnit.MINUTE => ClockManager.Now.AddMinutes(1),
        BO.TimeUnit.HOUR => ClockManager.Now.AddHours(1),
        BO.TimeUnit.DAY => ClockManager.Now.AddDays(1),
        BO.TimeUnit.MONTH => ClockManager.Now.AddMonths(1),
        BO.TimeUnit.YEAR => ClockManager.Now.AddYears(1),
        _ => DateTime.MinValue
    });
  
   
    public DateTime GetClock() => _dal.Config.Clock; //stage 4 //;
   

    public TimeSpan GetMaxRange()
    {
       return _dal.Config.RiskRange;
    }

    public void initialization()
    {
        DalTest.Initialization.Do(); //stage 4
        ClockManager.UpdateClock(ClockManager.Now);  //stage 4 - needed since we want the label on Pl to be updated
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}
