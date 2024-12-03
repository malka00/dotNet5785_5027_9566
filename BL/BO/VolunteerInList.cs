

namespace BO;

public class VolunteerInList
    {
    int Id {  get; set; }
    string FullName { get; set; }
    Boolean Active { get; set; }
    int SunCalls { get; set; }
    int Sumcanceled { get; set; }
    int SunExpired { get; set; }
    int? IdCall { get; init; }
    CallType Ctype { get; set; }

}

