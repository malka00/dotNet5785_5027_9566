﻿
using DalApi;
namespace Dal;

//stage 3
sealed internal class DalXml : IDal
{

    public static IDal Instance { get; } = new DalXml();
    private DalXml() { }
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}
    

