
using System.Collections;
using System.Collections.Generic;
namespace PL;

public class VolunteerInListCollection : IEnumerable 
{
    static readonly IEnumerable<BO.EVolunteerInList> s_enums =
        (Enum.GetValues(typeof(BO.EVolunteerInList)) as IEnumerable<BO.EVolunteerInList>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}








