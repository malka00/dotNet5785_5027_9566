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
    internal static string GetCallStatus(int callId)
    {
        // לדוגמה: שליפת נתוני הקריאה מבסיס נתונים (מדומה כאן כקריאה לשירות אחר)
        var call = s_dal.Call.Read(callId);

        if (call == null)
        {
            throw new Exception($"Call with ID {callId} was not found.");
        }

        // קביעת הסטטוס על בסיס המידע בבסיס הנתונים ושעון המערכת
        DateTime currentTime = DateTime.Now;

        if ((BO.StatusTreat)call.Status == "Open")
        {
            return "Overdue";
        }
        else if (call.Status == "Treat")
        {
            return "In Progress";
        }
        else if (call.Status == "Close")
        {
            return "Closed";
        }
        else if (call.Status == "RiskOpen")
        {
            return "RiskOpen";
        }
    }
}

