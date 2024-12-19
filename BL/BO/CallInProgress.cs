using Helpers;
namespace BO;


/// <summary>
/// The entity definition of call in progress
/// </summary>
public class CallInProgress
{
    public int Id { get; init; }
    public int IdCall { get; init; }
    public CallType Type { get; set; }
    public string? Description { get; set; }
    public string? FullCallAddress { get; set; }
    public DateTime TimeOpen {  get; set; }
    public DateTime? MaxTimeToClose { get; set; }
    public DateTime StartTreat {  get; set; }
    public double distanceCallVolunteer { get; set; }
    public StatusTreat Status { get; set; }
    public override string ToString() => this.ToStringProperty();
}
