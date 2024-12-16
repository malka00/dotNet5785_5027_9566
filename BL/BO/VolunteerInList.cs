

using Helpers;

namespace BO;

public class VolunteerInList
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public bool Active { get; set; }
    public int SumCalls { get; set; }
    public int Sumcanceled { get; set; }
    public int SumExpired { get; set; }
    public int? IdCall { get; init; }
    public CallType Ctype { get; set; }
    public override string ToString() => this.ToStringProperty();

}

