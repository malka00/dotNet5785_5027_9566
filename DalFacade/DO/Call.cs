

using Microsoft.VisualBasic;

// Module Call.cs
namespace DO;

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
    public int Id { get; set; }
    public CallType Type { get; set; }
    public string Description { get; set; }
    public string FullAddress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime TimeOpened { get; set; }
    public DateTime? MaxTimeToClose { get; set; }


    /// <summary>
    /// Default constructor for Call
    /// </summary>
}
