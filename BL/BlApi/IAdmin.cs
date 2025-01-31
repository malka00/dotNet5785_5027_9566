namespace BlApi;

/// <summary>
/// Definition of the admin methods
/// </summary>
public interface IAdmin
{
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
    TimeSpan GetMaxRange();
    void setMaxRange(TimeSpan time);
    void Reset();
    void initialization();

    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5

    void StartSimulator(int interval); //stage 7
    void StopSimulator(); //stage 7
}
