using System;


namespace DO;

/// <summary>
/// Enum definition for the user's role
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
    Aerial,       // aerial distance
    Walking,      // Walking distance
    Driving       // Travel distance
}

/// <summary>
/// Enum definition for the type of the call
/// </summary>
public enum CallType
{
    Puncture,
    Cables,
    LockedCar   
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

