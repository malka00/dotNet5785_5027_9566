
namespace BO;

public class CallInList
{
    int Id {  get; init; }
    int CallId {  get; init; }
    CallType CTypy { get; set; }
    DateTime TimeOpened { get; set; }
    TimeSpan? TimeLeft { get; set; }
    string? LastVolunteer {  get; set; }
    TimeSpan? TotalTime {  get; set; }
    StatusTreat Status { get; set; }
int SumAssignment {  get; set; }
}
