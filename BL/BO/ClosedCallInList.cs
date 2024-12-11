
namespace BO;

public class ClosedCallInList
{
    int Id { get; init; }
    CallType Type { get; set; }
    string? FullAddress { get; set; }
    DateTime TimeOpen { get; set; }
    DateTime StartTreat { get; set; }
    DateTime? TimeClose { get; set; }
    TypeEnd? TypeEndTreat {  get; set; }
}
