
namespace DO;

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
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Volunteer() : this(0, "", "", "", default(Role), default(Distance), false) { }
}




  
  
