namespace BlApi;

/// <summary>
/// Definition of the admin methods
/// </summary>
public interface IAdmin
{
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
    TimeSpan GetMaxRange();
    void Definition(TimeSpan time);
    void Reset();
    void initialization();
}
