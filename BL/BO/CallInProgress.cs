

namespace BO;

public class CallInProgress
{
    int Id { get; init; }
    int IdCall { get; init; }
    CallType Type { get; set; }
    string? Description { get; set; }
    string FullCallAddress { get; set; }
    DateTime TimeOpen {  get; set; }
    DateTime? MaxTimeToClose { get; set; }
    DateTime StertTreet {  get; set; }
    double distanceCallVolunteer { get; set; }
    StatusTreat Status { get; set; }
}
