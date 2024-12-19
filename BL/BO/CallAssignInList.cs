using Helpers;
namespace BO;


/// <summary>
/// The entity definition of call assignment in list
/// </summary>
public class CallAssignInList
{
    public int? VolunteerId { get; set; }
    public string? VolunteerName { get; set; }
    public DateTime StartTreat { get; set; }
    public DateTime? TimeClose { get; set; }
    public TypeEnd? TypeEndTreat { get; set; }
    public override string ToString() => this.ToStringProperty();
}

