
namespace BlApi;

public interface IVolunteer
{
    BO.Role EnterSystem(int usingName, String password) ;
    IEnumerable< BO.VolunteerInList> GetVolunteerList(bool? activ, BO.EVolunteerInList sortBy);
    BO.Volunteer Read(int id);
    void Update(int id, BO.Volunteer boVolunteer);
    void Delete(int id);
    void Create(BO.Volunteer boVolunteer);

}
