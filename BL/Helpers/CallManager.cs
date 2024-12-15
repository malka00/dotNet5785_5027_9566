using System.ComponentModel.Design;
using BlApi;
using BO;
using DalApi;
using DO;
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
    internal static StatusTreat GetCallStatus(DO.Call doCall)
    {
        if (doCall.MaxTimeToClose < s_dal.Config.Clock)
            return StatusTreat.Expired;
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
            else return StatusTreat.Treat;
        }
        return StatusTreat.Close;//default
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
}






