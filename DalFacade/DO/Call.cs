


// Module Call.cs
namespace DO;

public record Call
(
        int Id,
        CallType Type,
        string Description,
        string FullAddress,
        double Latitude,
        double Longitude,
        DateTime TimeOpened,
        DateTime? MaxTimeToClose = null
  );
/// <summary>
/// Course Entity
/// </summary>
/// <param name="Id">Unique readable identifier</param>
/// <param name="CallType Type">the type of reading</param>
/// <param name="Description">Verbal description of the reading</param>
/// <param name="FullAddress">Full address of the call</param>
/// <param name="Latitude">Latitude of reading location</param>
/// <param name="Longitude">Longitude of reading location</param>
/// <param name="TimeOpened">Reading opening time</param>
/// <param name="Credits">Maximum time to finish reading (if any)</param>

public class Call
{
    /// <summary>
    /// Default constructor for Call with default values
    /// </summary>
    public Call() : this(0, default(CallType), "", "", 0, 0, DateTime.MinValue) { }
}




