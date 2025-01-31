
namespace BO;

/// <summary>
/// Enum definition for the role of the user
/// </summary>
public enum Role
{
    Volunteer,
    Boss
}

/// <summary>
/// Enum definition for the distance type
/// </summary>
public enum Distance
{
    Aerial,      // aerial distance
    Walking,     // Walking distance
    Driving      // Travel distance
}

/// <summary>
/// Enum definition for the type of the call
/// </summary>
public enum CallType
{
    Puncture,
    Cables,
    LockedCar,
    None
}

/// <summary>
/// Enum definition for the status of the call
/// </summary>
public enum StatusTreat
{
    None,
    Open,
    Treat,
    Closed,
    Expired,
    RiskOpen,
    TreatInRisk
}

/// <summary>
/// Enum definition for the type of the end of the call
/// </summary>
public enum TypeEnd
{
    Treated,
    SelfCancel,
    ManagerCancel,
    ExpiredCancel
}

/// <summary>
/// Enum definition for the property of volunteer in list
/// </summary>
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

/// <summary>
/// Enum definition for the property of call in list
/// </summary>
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

/// <summary>
/// Enum definition for the property of closed call in list
/// </summary>
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

/// <summary>
/// Enum definition for the property of open call in list
/// </summary>
public enum EOpenCallInList
{
    Id,
    CType,
    FullAddress,
    TimeOpen,
    MaxTimeToClose,
    distanceCallVolunteer, 
}

/// <summary>
/// Enum definition for the unit of time
/// </summary>
public enum TimeUnit
{
    MINUTE,
    HOUR,
    DAY,
    MONTH,
    YEAR
}