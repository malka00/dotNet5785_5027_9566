using System;
using System.ComponentModel.Design;
using BO;
using DalApi;

namespace Helpers;


internal class CallManager
{
    private static IDal s_dal = Factory.Get;

    private static BO.CallInList GetCallsInList(DO.Call doCall)
        => new()
        {
            Id = doCall.Id,
            Type = (BO.CallType)doCall.Type,

        };

    public static bool IsInRisk(DO.Call call) => call!.MaxTimeToClose - s_dal.Config.Clock <= s_dal.Config.RiskRange;
    internal static IEnumerable<BO.CallInList> GetCallsInList(Predicate<DO.Call> condition)
        => s_dal.Call.ReadAll(call => condition(call)).Select(call => GetCallsInList(call));

    // פונקציית העזר הסטטית לשליפת סטטוס קריאה
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
        return BO.StatusTreat.Close;//default
    }


    internal static void CheckAddress(BO.Call call)
    {
        double[] coordinates = VolunteerManager.GetCoordinates(call.FullAddress);
        if (coordinates[0] != call.Latitude || coordinates[1] == call.Longitude)
            throw new BO.BlWrongItemtException($"not math coordinates");
    }

    /// <summary>
    /// כתובת תקינה וזמן מקסימלי תקין -  בדיקת הקריאה מבחינה לוגית
    /// </summary>
    internal static void CheckLogic(BO.Call boCall)
    {
        try
        {
            CheckAddress(boCall);
            if ((boCall.MaxTimeToClose <= ClockManager.Now) || (boCall.MaxTimeToClose <= boCall.TimeOpened))
                throw new BO.BlWrongItemtException("Error input");

        }
        catch (BO.BlWrongItemtException ex)
        {
            throw new BO.BlWrongItemtException($"the item have logic problem", ex);
        }
    }
    internal static BO.CallInList ConvertDOCallToBOCallInList (DO.Call doCall)
    {
       var assignmentsForCall = s_dal.Assignment.ReadAll(A => A.CallId == doCall.Id);
       var lastAssignmentsForCall= assignmentsForCall.OrderByDescending(item => item.TimeStart).FirstOrDefault();
        return new()
        {
            Id = lastAssignmentsForCall!=null? lastAssignmentsForCall.Id:null,
            CallId = lastAssignmentsForCall.CallId,
            Type = (BO.CallType)doCall.Type,
            TimeOpened = doCall.TimeOpened,
            TimeLeft = doCall.MaxTimeToClose!=null? doCall.MaxTimeToClose - s_dal.Config.Clock:null,
            LastVolunteer = lastAssignmentsForCall != null? s_dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)!.FullName:null,
            TotalTime= lastAssignmentsForCall.TimeEnd!=null? lastAssignmentsForCall.TimeEnd - lastAssignmentsForCall.TimeStart:null,
            Status= GetCallStatus(doCall),
            SumAssignment= assignmentsForCall.Count()
        };
    }

    /// <summary>
    /// ממיר את הקריאה לקריאה סגורה ברשימה
    /// </summary>
    /// <param name="doCall"></param>
    /// <param name="lastAssignment"></param>
    /// <returns></returns>
    internal static BO.ClosedCallInList ConvertDOCallToBOCloseCallInList(DO.Call doCall, CallAssignInList lastAssignment)
    {
        return new BO.ClosedCallInList
        {
            Id = doCall.Id,
            Type = (BO.CallType)doCall.Type,
            FullAddress = doCall.FullAddress,
            TimeOpen = doCall.TimeOpened,
            StartTreat = lastAssignment.StartTreat,
            TimeClose = lastAssignment.TimeClose,
            TypeEndTreat = lastAssignment.TypeEndTreat
        };
    }
    internal static BO.OpenCallInList ConvertDOCallToBOOpenCallInList(DO.Call doCall, int id )
    {
        var vol = s_dal.Volunteer.Read(id);
        double idLat = vol.Latitude??0;
        double idLon = vol.Longitude ?? 0;
        return new BO.OpenCallInList
        {
            Id = doCall.Id,
           CType = (BO.CallType)doCall.Type,
            Description =  doCall.Description,
            FullAddress = doCall.FullAddress,
            TimeOpen = doCall.TimeOpened,
            MaxTimeToClose =doCall.MaxTimeToClose,
            distanceCallVolunteer = VolunteerManager.CalculateDistance(doCall.Latitude, doCall.Longitude,idLat,idLon),
        };
    }



}








