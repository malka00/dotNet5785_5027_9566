using BlApi;
namespace BlImplementation;
internal class Bl : IBl
{
    public IVolunteer Volunteers { get; } = new VolunteerImplementation();
    public ICall Calls { get; } = new CallImplementation();
    public IAdmin Admin { get; } = new AdminImplementation();
}

