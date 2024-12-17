
using Helpers;

namespace BO;

public class ClosedCallInList
{
    public int Id { get; init; }
    public CallType Type { get; set; }
    public string? FullAddress { get; set; }
    public DateTime TimeOpen { get; set; }
    public DateTime StartTreat { get; set; }
    public DateTime? TimeClose { get; set; }
    public TypeEnd? TypeEndTreat {  get; set; }
    public override string ToString() => this.ToStringProperty();
}
