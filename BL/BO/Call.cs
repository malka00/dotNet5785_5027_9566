using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;

public class Call
{
    int Id { get; init; }
    CallType Typy { get; set; }
    string Description { get; set; }
    string FullAddress { get; set; }
    double Latitude { get; set; }
    double Longitude { get; set; }
    DateTime TimeOpened { get; set; }
    DateTime? MaxTimeToClose { get; set; }
    Status stat {  get; set; }
    List<BO.CallAssignInList>? AssignemtsToCalls { get; set; }

}
