using System;


namespace DO;


public enum Role
{
    Volunteer,   // מתנדב
    Boss         // מנהל
}

// הגדרת Enum עבור סוג המרחק
public enum Distance
{
    Aerial,      // מרחק אווירי
    Walking,     // מרחק הליכה
    Driving      // מרחק נסיעה
}
public enum CallType
{
    Puncture,
    Cables,
    LockedCar   
}
public enum TypeEnd
{

    Treated,//טופל
   // NotYetTreet//לא טופל
    SelfCancel,       // ביטול עצמי
    ManagerCancel,    // ביטול מנהל
    ExpiredCancel     // ביטול פג תוקף
}

