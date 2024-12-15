using Helpers;


namespace BO;

public class Call
{
    public  int Id { get; init; }
    public CallType Type { get; set; }
    public string? Description { get; set; }
    public string? FullAddress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime TimeOpened { get; set; }
    public DateTime? MaxTimeToClose { get; set; }
    public StatusTreat Status  { get; set; }
    public  List<BO.CallAssignInList>? AssignmentsToCalls { get; set; }
    public override string ToString() => this.ToStringProperty();
}
