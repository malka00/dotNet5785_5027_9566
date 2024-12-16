

using Helpers;

namespace BO;
public class OpenCallInList
{
    public  int Id { get; init; }
    public CallType CType { get; set; }
    public string? Description { get; set; }
    public string FullAddress { get; set; }
    public DateTime TimeOpen { get; set; }
    public DateTime? MaxTimeToClose { get; set; }
    public  double distanceCallVolunteer { get; set; }
    public override string ToString() => this.ToStringProperty();
}
