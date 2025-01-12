
using System.Collections;
using System.Collections.Generic;
namespace PL;
internal class RoleCollection : IEnumerable
{
    static readonly IEnumerable<BO.Role> s_enums =
        (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class ClosedCallColection : IEnumerable
{
    static readonly IEnumerable<BO.EClosedCallInList> s_enums =
        (Enum.GetValues(typeof(BO.EClosedCallInList)) as IEnumerable<BO.EClosedCallInList>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class VolunteerInListCollection : IEnumerable 
{
    static readonly IEnumerable<BO.EVolunteerInList> s_enums =
        (Enum.GetValues(typeof(BO.EVolunteerInList)) as IEnumerable<BO.EVolunteerInList>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class CallInListCollection : IEnumerable
{
    static readonly IEnumerable<BO.ECallInList> s_enums =
        (Enum.GetValues(typeof(BO.ECallInList)) as IEnumerable<BO.ECallInList>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class OpenCallInListCollection : IEnumerable
{
    static readonly IEnumerable<BO.EOpenCallInList> s_enums =
        (Enum.GetValues(typeof(BO.EOpenCallInList)) as IEnumerable<BO.EOpenCallInList>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class DistanceCollection : IEnumerable
{
    static readonly IEnumerable<BO.Distance> s_enums =
        (Enum.GetValues(typeof(BO.Distance)) as IEnumerable<BO.Distance>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}


internal class StatusCollection : IEnumerable
{
    static readonly IEnumerable<BO.StatusTreat> s_enums =
        (Enum.GetValues(typeof(BO.StatusTreat)) as IEnumerable<BO.StatusTreat>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}





