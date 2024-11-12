// Module Assignment.cs


namespace DO;

/// <summary>
/// Assignment Entity
/// </summary>
/// <param name="Id">A unique identifier for the assignment</param>
/// <param name="CallId">The call ID that the volunteer chose to handle</param>
/// <param name="VolunteerId">ID number of the volunteer who chose to take care of the reading</param>
/// <param name="TimeStart">Treatment entry time (date and time)</param>
/// <param name="TimeEnd">Actual treatment end time (date and time)</param>
/// <param name="TypeEndTreat">Type of treatment termination (treated, self-cancellation, administrator cancellation, expired cancellation)</param>

public class Assignment
{
    public int Id { get; set; }           
    public int CallId { get; set; }         
    public int VolunteerId { get; set; }    
    public DateTime TimeStart { get; set; } 
    public DateTime? TimeEnd { get; set; }  
    public TypeEnd TypeEndTreat { get; set; }

    /// <summary>
    /// Default constructor for Assignment
    /// </summary>

}
