
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
/// <param name="Job">Represents a position - volunteer/manager</param>
/// <param name="Active">Whether the volunteer is active or not</param>
/// <param name="MaxReading">Each volunteer will define through the display the maximum distance for receiving a call</param>
/// <param name="TypeDistance">Aerial distance, walking distance, driving distance</param>
public record Volunteer
(
     int Id,
     string FullName,
     string PhoneNumber,
     string Email,
     Distance TypeDistance,
     Role Job,
     bool Active,
     string? Password = null,
     string? FullAddress = null,
     double? Latitude = null,
     double? Longitude = null,
     double? MaxReading = null
   )


/// <summary>
/// Default constructor Volunteer
/// </summary>



{
    public Volunteer() : this(0, "", "", "", default(Distance), default(Role), false) { }
};




  
  
