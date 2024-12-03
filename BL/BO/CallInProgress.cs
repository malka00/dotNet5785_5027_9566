

namespace BO;

internal class CallInProgress
{
    int Id { get; init; }
    int CallId { get; init; }
    CallType Type { get; set; }
    string? Description { get; set; }
    string FullCallAddress { get; set; }
    DateTime TimeOpened {  get; set; }
    DateTime? MaxTimeToClose { get; set; }
    DateTime StertTreet {  get; set; }
    double distanceCallVolunteer { get; set; }
    StatusTreat Status { get; set; }
}
