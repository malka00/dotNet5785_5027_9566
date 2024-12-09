
using DalApi;
using System;
using System.Net.NetworkInformation;

namespace Helpers;

internal class VolunteerManager
{
    private static IDal s_dal = Factory.Get;

    internal static void InputIntegrity(BO.Volunteer volunteer)
    {

    }
    /// <summary>
    /// func for convert DO.volunteer for BO.VolunteerInList
    /// </summary>
    /// <param name="doVolunteer"></param>
    /// <returns></returns>
    internal static BO.VolunteerInList convertDOToBOInList(DO.Volunteer doVolunteer)
    {
        var call = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();
        int sumCalls= call.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated);
        int sumCanceld = call.Count(ass => ass.TypeEndTreat == DO.TypeEnd.SelfCancel);
        int sumExpired = call.Count(ass => ass.TypeEndTreat == DO.TypeEnd.ExpiredCancel);
        int? idCall= call.Count(ass => ass.TimeEnd == null);
        return new()
         {
         Id=  doVolunteer.Id,
         FullName=   doVolunteer.FullName,
          Active=  doVolunteer.Active, 
          SunCalls=  sumCalls,
          Sumcanceled= sumCanceld,
          SumExpired= sumExpired,
          IdCall= idCall,
        }; 
    }
    internal static BO.CallInProgress GetCallIn(DO.Volunteer doVolunteer)
    {
     
        var call = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();
        DO.Assignment? assignmentTreat = call.Find(ass => ass.TimeEnd == null);
        DO.Call? callTreat = s_dal.Call.Read(assignmentTreat.CallId);
            return new()
        {
            Id = assignmentTreat.Id,
            IdCall = assignmentTreat.CallId,
            Type=(BO.CallType)callTreat.Type,
            Description=callTreat.Description,
            FullCallAddress=callTreat.FullAddress,
            TimeOpen=callTreat.TimeOpened,
            MaxTimeToClose=callTreat.MaxTimeToClose,
            StertTreet= assignmentTreat.TimeStart,
            distanceCallVolunteer= Tools.CalculateDistance(callTreat.Latitude, callTreat.Longitude,doVolunteer.Latitude, doVolunteer.Longitude),
            Status= (callTreat.MaxTimeToClose - ClockManager.Now <= GetMaxRange())
             ? BO.StatusTreat.RiskOpen : BO.StatusTreat.Treat,
    };

    }
}
