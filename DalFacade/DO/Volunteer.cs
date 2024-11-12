

using System.Data;

// Module Volunteer.cs
namespace DO;

/// <summary>
/// Volunteer Entity represents a volunteer with all its props
/// </summary>
/// <param name="Id">Personal unique ID of the volunteer (as in national id card)</param>
/// <param name="FullName">Private Name of the volunteer</param>
/// <param name="PhoneNumber">volunteer for a standard cell phone. 10 digits only. Starts with the number 0</param>
/// <param name="Email">Represents a valid email address</param>
/// <param name="Password">At the beginning, an initial password will be given by the manager and then the volunteer will be able to update it.</param>
/// <param name="FullAddress">Complete address of the volunteer</param>
/// <param name="Latitude">a number indicating how far a point on the Earth is south or north of the equator</param>
/// <param name="Longitude">A number indicating how far a point on Earth is east or west of the equator</param>
/// <param name="Role Job ">Represents a position - volunteer/manager</param>
/// <param name="Active">Whether the volunteer is active or not</param>
/// <param name="MaxReading">Each volunteer will define through the display the maximum distance for receiving a call</param>
/// <param name="Distance TypeDistance">Aerial distance, walking distance, driving distance</param>
public record class Volunteer
{
    public int Id { get; set; }                  // מזהה ייחודי של המתנדב
    public string FullName { get; set; }         // שם מלא (שם פרטי ושם משפחה)
    public string PhoneNumber { get; set; }      // מספר טלפון
    public string Email { get; set; }            // כתובת דוא"ל
    public string? Password { get; set; }        // סיסמה (אם רלוונטי)
    public string? FullAddress { get; set; }     // כתובת מלאה
    public double? Latitude { get; set; }        // קו רוחב
    public double? Longitude { get; set; }       // קו אורך
    public Role Job { get; set; }                // תפקיד (מתנדב או מנהל)
    public bool Active { get; set; }             // האם המשתמש פעיל
    public double? MaxReading { get; set; }      // מרחק מקסימלי לפעילות
    public Distance TypeDistance { get; set; }   // סוג המרחק (אווירי, הליכה, נסיעה)

    /// <summary>
    /// Default constructor Volunteer
    /// </summary>
    public Volunteer() : this(){ }


    // פונקציה לעדכון מיקום המתנדב
    public void UpdateLocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}
