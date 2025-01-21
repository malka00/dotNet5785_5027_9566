using System;
using System.ComponentModel.Design;
using System.Net.NetworkInformation;
using BlImplementation;
using BO;
using DalApi;
using DO;
namespace Helpers;


/// <summary>
/// Auxiliary functions for the implementation of the call
/// </summary>
internal class CallManager
{
    private static IDal s_dal = Factory.Get;   //stage 4

    internal static ObserverManager Observers = new(); //stage 5 


    

    ///// <summary>
    ///// Converts an object of type DO.Call to an object of type BO.CallInList
    ///// </summary>
    ///// <param name="doCall"></param>
    ///// <returns> callInList </returns>
    private static BO.CallInList GetCallsInList(DO.Call doCall)
        => new()
        {
            Id = doCall.Id,
            Type = (BO.CallType)doCall.Type,
        };

    /// <summary>
    /// Checks if a call (DO.Call) is in the risk time range
    /// </summary>
    /// <param name="call"></param>
    /// <returns> bool </returns>
    public static bool IsInRisk(DO.Call call) => call!.MaxTimeToClose - s_dal.Config.Clock <= s_dal.Config.RiskRange;

    /// <summary>
    /// Makes a call to all the calls from the data source (the DAL), filters them according to a condition given as a parameter,
    /// and then returns a list of objects of type BO.CallInList
    /// </summary>
    /// <param name="condition"></param>
    /// <returns> CallInList </returns>
    internal static IEnumerable<BO.CallInList> GetCallsInList(Predicate<DO.Call> condition)
        => s_dal.Call.ReadAll(call => condition(call)).Select(call => GetCallsInList(call));

    /// <summary>
    /// Static helper function for retrieving read call
    /// </summary>
    /// <param name="doCall"></param>
    /// <returns> BO.StatusTreat </returns>
    internal static BO.StatusTreat GetCallStatus(DO.Call doCall)
    {
        if (doCall.MaxTimeToClose < s_dal.Config.Clock)
            return BO.StatusTreat.Expired;
        // var lastAssignment = s_dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id).OrderByDescending(a => a.TimeStart).FirstOrDefault();
        var lastAssignment = s_dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id).OrderByDescending(a => a.Id).FirstOrDefault();
        if (lastAssignment == null)
        {
            if (IsInRisk(doCall!))
                return BO.StatusTreat.RiskOpen;
            else return BO.StatusTreat.Open;
        }
        if (lastAssignment.TypeEndTreat==DO.TypeEnd.Treated)
        {
            return BO.StatusTreat.Closed;
        }
        if (lastAssignment.TypeEndTreat == null)
        {
            if (IsInRisk(doCall!))
                return BO.StatusTreat.TreatInRisk;
            else return BO.StatusTreat.Treat;
        }
        if (lastAssignment.TypeEndTreat.ToString() == "SelfCancel" || lastAssignment.TypeEndTreat.ToString() == "ManagerCancel")
        {

            return BO.StatusTreat.Open;

        }
        return BO.StatusTreat.Closed;//default
    }

    /// <summary>
    /// A function that checks the address
    /// </summary>
    /// <param name="call"></param>
    /// <exception cref="BO.BlWrongItemException"></exception>
    internal static void CheckAddress(BO.Call call)
    {
        double[] coordinates = VolunteerManager.GetCoordinates(call.FullAddress);
        if (coordinates[0] != call.Latitude || coordinates[1] != call.Longitude)
            throw new BO.BlWrongItemException($"not math coordinates");
    }

    /// <summary>
    /// check the call from a logical point of view - correct address and maximum time (which has not yet passed)
    /// </summary>
    internal static void CheckLogic(BO.Call boCall)
    {
            CheckAddress(boCall);
            if (/*(boCall.MaxTimeToClose <= AdminManager.Now) ||*/ (boCall.MaxTimeToClose <= boCall.TimeOpened))
                throw new BO.BlWrongItemException("Time to close must be after time open");
    }

    /// <summary>
    /// The function converts an object of type DO.Call to an object of type BO.CallInList. 
    /// It does this by creating a new object of type BO.CallInList and filling its fields with data from the DO.Call and the related Assignments.
    /// </summary>
    /// <param name="doCall"></param>
    /// <returns> BO.CallInList </returns>
    internal static BO.CallInList ConvertDOCallToBOCallInList(DO.Call doCall)
    {
        IEnumerable<Assignment>? assignmentsForCall;
        lock (AdminManager.BlMutex)
             assignmentsForCall = s_dal.Assignment.ReadAll(a => a.CallId == doCall.Id);
        var lastAssignmentsForCall = assignmentsForCall.OrderByDescending(item => item.TimeStart).FirstOrDefault();

        lock (AdminManager.BlMutex)
        {
            int? id = (lastAssignmentsForCall == null) ? null : lastAssignmentsForCall.Id;
            int callId = doCall.Id;
            BO.CallType type = (BO.CallType)doCall.Type;
            DateTime timeOpened = doCall.TimeOpened;
            TimeSpan? timeLeft = (doCall.MaxTimeToClose != null) ? doCall.MaxTimeToClose - s_dal.Config.Clock : null;
            // string? lastVolunteer = (lastAssignmentsForCall != null) ? s_dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)!.FullName : null;
            string? lastVolunteer = (lastAssignmentsForCall != null)
          ? s_dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)?.FullName
          : null;
        }
           
        TimeSpan? totalTime = (lastAssignmentsForCall != null && lastAssignmentsForCall.TimeEnd != null) ? lastAssignmentsForCall.TimeEnd - lastAssignmentsForCall.TimeStart : null;
        BO.StatusTreat status = GetCallStatus(doCall);
        int sumAssignment = (assignmentsForCall == null) ? 0 : assignmentsForCall.Count();

        lock (AdminManager.BlMutex)
        {
            return new()
            {
                Id = (lastAssignmentsForCall == null) ? null : lastAssignmentsForCall.Id,
                CallId = doCall.Id,
                Type = (BO.CallType)doCall.Type,
                TimeOpened = doCall.TimeOpened,
                TimeLeft = (doCall.MaxTimeToClose != null && doCall.MaxTimeToClose >= s_dal.Config.Clock) ? doCall.MaxTimeToClose - s_dal.Config.Clock : null,
                LastVolunteer = (lastAssignmentsForCall != null)
                ? s_dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)?.FullName
                : null,
                //LastVolunteer = (lastAssignmentsForCall != null) ? s_dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)!.FullName : null,
                TotalTime = (lastAssignmentsForCall != null && lastAssignmentsForCall.TimeEnd != null) ? lastAssignmentsForCall.TimeEnd - lastAssignmentsForCall.TimeStart : null,
                Status = GetCallStatus(doCall),
                SumAssignment = (assignmentsForCall == null) ? 0 : assignmentsForCall.Count()

            };
        }
    }

    /// <summary>
    /// convert DO call to BO call
    /// </summary>
    /// <param name="doCall"></param>
    /// <returns> BO.Call </returns>
    internal static BO.Call convertDOtoBO(DO.Call doCall)
    {
        Func<DO.Assignment, bool> func;
        lock (AdminManager.BlMutex)
            func = item => item.CallId == doCall.Id;
        IEnumerable<DO.Assignment> dataOfAssignments = s_dal.Assignment.ReadAll(func);
        return new()
        {
            Id = doCall.Id,
            Type = (BO.CallType)doCall.Type,
            Description = doCall.Description,
            FullAddress = doCall.FullAddress,
            Latitude = doCall.Latitude,
            Longitude = doCall.Longitude,
            TimeOpened = doCall.TimeOpened,
            MaxTimeToClose = doCall.MaxTimeToClose,
            Status = GetCallStatus(doCall),
            AssignmentsToCalls = dataOfAssignments.Any()
        ? dataOfAssignments.Select(assign => new BO.CallAssignInList
        {
            VolunteerId = assign.VolunteerId,
            VolunteerName = s_dal.Volunteer.Read(assign.VolunteerId)?.FullName,
            StartTreat = assign.TimeStart,
            TimeClose = assign.TimeEnd,
            TypeEndTreat = assign.TypeEndTreat == null ? null : (BO.TypeEnd)assign.TypeEndTreat,
        }).ToList()
        : null,

        };
    }

    /// <summary>
    /// The `UpdateExpiredCalls` function is responsible for updating the status of expired calls (calls whose maximum time to close has passed).
    /// </summary>
    internal static void UpdateExpiredCalls()
    {
        bool assignmentUpdated = false; //stage 5

        IEnumerable<DO.Call> calls = s_dal.Call.ReadAll();
        IEnumerable<BO.Call> boCalls = from dCall in calls
                                       where (dCall.MaxTimeToClose == null ? true : dCall.MaxTimeToClose < s_dal.Config.Clock)
                                       select (convertDOtoBO(dCall));
        foreach (BO.Call call in boCalls)
        {
            if (call.AssignmentsToCalls == null)
            { 
                s_dal.Assignment.Create(new DO.Assignment(0, call.Id, 0, s_dal.Config.Clock, s_dal.Config.Clock, DO.TypeEnd.ExpiredCancel));
            }
             
            else
            {
                var lastAss = call.AssignmentsToCalls.OrderByDescending(a => a.StartTreat).First();
                if (lastAss.TypeEndTreat == null)
                {
                    assignmentUpdated = true; //stage 5
                    var assignment = s_dal.Assignment.Read(a => a.VolunteerId == lastAss.VolunteerId && a.TimeEnd == null && a.TypeEndTreat == null);
                    Observers.NotifyItemUpdated(assignment.Id); //stage 5
                    s_dal.Assignment.Update(new DO.Assignment(assignment.Id, assignment.CallId, assignment.VolunteerId, lastAss.StartTreat, s_dal.Config.Clock, DO.TypeEnd.ExpiredCancel));
                }
            }
        }
        if(assignmentUpdated)
            Observers.NotifyListUpdated();
    }
}
