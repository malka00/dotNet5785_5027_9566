﻿

namespace Dal;
using DalApi;

sealed public class DalList : IDal
{
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }
}
