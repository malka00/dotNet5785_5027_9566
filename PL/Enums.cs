

namespace PL;

internal class VolunteersCollection : IEnumerable
{
    static readonly IEnumerable<BO.EVolunteerInList> s_enums =
        (Enum.GetValues(typeof(BO.EVolunteerInList)) as IEnumerable<BO.EVolunteerInList>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}


