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

    internal static IEnumerable<BO.CallInList> GetCallsInList(Predicate<DO.Call> condition)
        => s_dal.Call.ReadAll(call => condition(call)).Select(call => GetCallsInList(call));

    // פונקציית העזר הסטטית לשליפת סטטוס קריאה
    internal static StatusTreat GetCallStatus(IEnumerable<DO.Assignment> assignments, DateTime maxTimeToClose )
    {
       var endTreatment =frome item in assignments
            select item.
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

