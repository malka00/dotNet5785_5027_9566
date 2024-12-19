using System;
using System.ComponentModel.Design;
using System.Net.NetworkInformation;
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
        var lastAssignment = s_dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id).OrderByDescending(a => a.TimeStart).FirstOrDefault();

        if (lastAssignment == null)
        {
            if (IsInRisk(doCall!))
                return BO.StatusTreat.RiskOpen;
            else return BO.StatusTreat.Open;
        }
        if (lastAssignment.TypeEndTreat.ToString() == "Treated")
        {
            return BO.StatusTreat.Close;
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
        return BO.StatusTreat.Close;//default
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
        try
        {
            CheckAddress(boCall);
            if ((boCall.MaxTimeToClose <= ClockManager.Now) || (boCall.MaxTimeToClose <= boCall.TimeOpened))
                throw new BO.BlWrongItemException("Error input");

        }
        catch (BO.BlWrongItemException ex)
        {
            throw new BO.BlWrongItemException($"the item have logic problem", ex);
        }
    }

    /// <summary>
    /// The function converts an object of type DO.Call to an object of type BO.CallInList. 
    /// It does this by creating a new object of type BO.CallInList and filling its fields with data from the DO.Call and the related Assignments.
    /// </summary>
    /// <param name="doCall"></param>
    /// <returns> BO.CallInList </returns>
    internal static BO.CallInList ConvertDOCallToBOCallInList(DO.Call doCall)
    {
        var assignmentsForCall = s_dal.Assignment.ReadAll(a => a.CallId == doCall.Id);
        var lastAssignmentsForCall = assignmentsForCall.OrderByDescending(item => item.TimeStart).FirstOrDefault();

        int? id = (lastAssignmentsForCall == null) ? null : lastAssignmentsForCall.Id;
        int callId = doCall.Id;
        BO.CallType type = (BO.CallType)doCall.Type;
        DateTime timeOpened = doCall.TimeOpened;
        TimeSpan? timeLeft = (doCall.MaxTimeToClose != null) ? doCall.MaxTimeToClose - s_dal.Config.Clock : null;
        // string? lastVolunteer = (lastAssignmentsForCall != null) ? s_dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)!.FullName : null;
        string? lastVolunteer = (lastAssignmentsForCall != null)
      ? s_dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)?.FullName
      : null;

        TimeSpan? totalTime = (lastAssignmentsForCall != null && lastAssignmentsForCall.TimeEnd != null) ? lastAssignmentsForCall.TimeEnd - lastAssignmentsForCall.TimeStart : null;
        BO.StatusTreat status = GetCallStatus(doCall);
        int sumAssignment = (assignmentsForCall == null) ? 0 : assignmentsForCall.Count();

        return new()
        {
            Id = (lastAssignmentsForCall == null) ? null : lastAssignmentsForCall.Id,
            CallId = doCall.Id,
            Type = (BO.CallType)doCall.Type,
            TimeOpened = doCall.TimeOpened,
            TimeLeft = doCall.MaxTimeToClose != null ? doCall.MaxTimeToClose - s_dal.Config.Clock : null,
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



    ///// <summary>
    ///// ממיר את הקריאה לקריאה סגורה ברשימה
    ///// </summary>
    ///// <param name="doCall"></param>
    ///// <param name="lastAssignment"></param>
    ///// <returns></returns>
    //internal static BO.ClosedCallInList ConvertDOCallToBOCloseCallInList(DO.Call doCall, CallAssignInList lastAssignment)
    //{
    //    return new BO.ClosedCallInList
    //    {
    //        Id = doCall.Id,
    //        Type = (BO.CallType)doCall.Type,
    //        FullAddress = doCall.FullAddress,
    //        TimeOpen = doCall.TimeOpened,
    //        StartTreat = lastAssignment.StartTreat,
    //        TimeClose = lastAssignment.TimeClose,
    //        TypeEndTreat = lastAssignment.TypeEndTreat
    //    };
    //}
    //internal static BO.OpenCallInList ConvertDOCallToBOOpenCallInList(DO.Call doCall, int id )
    //{
    //    var vol = s_dal.Volunteer.Read(id);
    //    double idLat = vol.Latitude??0;
    //    double idLon = vol.Longitude ?? 0;
    //    return new BO.OpenCallInList
    //    {
    //        Id = doCall.Id,
    //       CType = (BO.CallType)doCall.Type,
    //        Description =  doCall.Description,
    //        FullAddress = doCall.FullAddress,
    //        TimeOpen = doCall.TimeOpened,
    //        MaxTimeToClose =doCall.MaxTimeToClose,
    //        distanceCallVolunteer = VolunteerManager.CalculateDistance(doCall.Latitude, doCall.Longitude,idLat,idLon),
    //    };
    //}









