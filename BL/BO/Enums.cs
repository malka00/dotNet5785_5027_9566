
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
    LockedCar,
    None
}
public enum StatusTreat
{
    Open,
    Treat,
    Close,
    Expired,
    RiskOpen,
    TreatInRisk
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
    SumCalls,
    SumCanceled,
    SumExpired,
    IdCall,
    CType,
}
public enum ECallInList
{
    Id ,
    CallId,
    CType,
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
    FullAddress,
    TimeOpen,
    StartTreat,
    TimeClose,
    TypeEndTreat,
}
public enum EOpenCallInList
{
    Id,
    CType,
    Description,
    FullAddress,
    TimeOpen,
    MaxTimeToClose,
    distanceCallVolunteer,
    
}
public enum TimeUnit
{
    MINUTE,
    HOUR,
    DAY,
    MONTH,
    YEAR
}