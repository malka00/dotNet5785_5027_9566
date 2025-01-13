using Helpers;
namespace BO;


/// <summary>
///  The entity definition of open call in  list
/// </summary>
public class OpenCallInList
{
    public  int Id { get; init; }
    public CallType CType { get; set; }
    public string? Description { get; set; }
    public string FullAddress { get; set; }
    public DateTime TimeOpen { get; set; }
    public DateTime? MaxTimeToClose { get; set; }
    public  double distanceCallVolunteer { get; set; }
    public StatusTreat Status {  get; set; }
    public override string ToString() => this.ToStringProperty();
}
