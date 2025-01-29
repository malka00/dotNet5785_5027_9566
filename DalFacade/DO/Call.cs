using System.Xml.Serialization;

namespace DO;

/// <summary>
/// Course Entity
/// </summary>
/// <param name="Id">Unique readable identifier</param>
/// <param name="Type">the type of reading</param>
/// <param name="Description">Verbal description of the reading</param>
/// <param name="FullAddress">Full address of the call</param>
/// <param name="Latitude">Latitude of reading location</param>
/// <param name="Longitude">Longitude of reading location</param>
/// <param name="TimeOpened">Reading opening time</param>
/// <param name="MaxTimeToClose">Maximum time to finish reading (if any)</param>

public record Call
(
        int Id,
        CallType Type,
        string? Description,
        string FullAddress,
        double? Latitude,
        double? Longitude,
        DateTime TimeOpened,
        DateTime? MaxTimeToClose
  )


/// <summary>
/// Default constructor for Call with default values
/// </summary>
{
    public Call() : this(0, default(CallType), "", "", null, null, DateTime.MinValue,null) { }
}




