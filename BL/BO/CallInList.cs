using Helpers;
namespace BO;


/// <summary>
/// The entity definition of call in list
/// </summary>
public class CallInList : IComparable<CallInList>
{
    public int? Id {  get; init; }
    public int CallId {  get; init; }
    public  CallType Type  { get; set; }
    public DateTime TimeOpened { get; set; }
    public TimeSpan? TimeLeft { get; set; }
    public string? LastVolunteer {  get; set; }
    public TimeSpan? TotalTime {  get; set; }
    public StatusTreat Status { get; set; }
    public int SumAssignment {  get; set; }

    public int CompareTo(CallInList? other)
    {
        return CallId.CompareTo(other?.CallId);
    }

    public override string ToString() => this.ToStringProperty();
}
