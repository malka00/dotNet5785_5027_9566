using Helpers;
namespace BO;


/// <summary>
///  The entity definition of volunteer in  list
/// </summary>
public class VolunteerInList
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public bool Active { get; set; }
    public int SumCalls { get; set; }
    public int SumCanceled { get; set; }
    public int SumExpired { get; set; }
    public int? IdCall { get; init; }
    public CallType CType { get; set; }
    public override string ToString() => this.ToStringProperty();
}

