
namespace BO;

public class OpenCallInList
{
    int Id { get; init; }
    CallType CType { get; set; }
    string? Description { get; set; }
    string FullAdress { get; set; }
    DateTime TimeOpen { get; set; }
    DateTime? MaxTimeToClose { get; set; }
    double distanceCallVolunteer { get; set; }
}
