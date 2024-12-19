namespace BlApi;


public interface IBl
{
    IVolunteer Volunteers { get; }
    ICall Calls { get; }
    IAdmin Admin { get; }
}
