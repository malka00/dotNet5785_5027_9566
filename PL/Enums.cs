
using System.Collections;
using System.Collections.Generic;
namespace PL;
internal class RoleCollection : IEnumerable
{
    static readonly IEnumerable<BO.Role> s_enums =
        (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class VolunteerInListCollection : IEnumerable 
{
    static readonly IEnumerable<BO.EVolunteerInList> s_enums =
        (Enum.GetValues(typeof(BO.EVolunteerInList)) as IEnumerable<BO.EVolunteerInList>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class DistanceCollection : IEnumerable
{
    static readonly IEnumerable<BO.Distance> s_enums =
        (Enum.GetValues(typeof(BO.Distance)) as IEnumerable<BO.Distance>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}








