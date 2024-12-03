
namespace BO;
public enum Role
{
    Volunteer,
    Boss
}

// Enum definition for the distance type
public enum Distance
{
    Aerial,      // aerial distance
    Walking,    // Walking distance
    Driving      // Travel distance
}
public enum CallType
{
    Puncture,
    Cables,
    LockedCar
}
public enum StatusTreat
{
    Open,
    Treat,
    Close,
    RiskOpen
}
public enum TypeEnd
{

    Treated,
    SelfCancel,
    ManagerCancel,
    ExpiredCancel
}
public enum EVolunteerInList
{
    Id,
    FullName,
    Active,
    SunCalls,
    Sumcanceled,
    SunExpired,
    IdCall,
    Ctype,
}
public enum ECallInList
{
    Id ,
    CallId,
    CTypy,
    TimeOpened,
    TimeLeft,
    LastVolunteer,
    TotalTime,
    Status,
    SumAssignment,
}
public enum EClosedCallInList
{
    Id,
    CType,
    FullAdress,
    TimeOpen,
    StertTreet,
    TimeClose,
    TypeEndTreat,
}
public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}