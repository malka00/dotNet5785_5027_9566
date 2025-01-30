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
/// </summary>
internal class CallManager
{
    private static IDal s_dal = Factory.Get;   //stage 4

    internal static ObserverManager Observers = new(); //stage 5 
    internal static async Task updateCoordinatesForCallsAddressAsync(DO.Call doCall)
    {
        if (doCall.FullAddress is not null)
        {
            double[] coordinates = await Tools.GetCoordinatesAsync(doCall.FullAddress);
            if (coordinates==null)
                doCall = doCall with { Latitude = null, Longitude = null};
            else
            doCall = doCall with { Latitude = coordinates[0], Longitude = coordinates[1] };
            lock (AdminManager.BlMutex)
                    s_dal.Call.Update(doCall);
           Observers.NotifyListUpdated();
          Observers.NotifyItemUpdated(doCall.Id);
            
        }
    }

    internal static void ChoseForTreatHelp(int idVol, int idCall)
    {
        // Retrieve volunteer and call; throw exception if not found.
        DO.Volunteer vol;
        lock (AdminManager.BlMutex) //stage 7
            vol = s_dal.Volunteer.Read(idVol);       
        if (vol==null)
            throw new BO.BlNullPropertyException($"There is no volunteer with this ID {idVol}");
        BO.Call boCall = ReadHelp(idCall) ?? throw new BO.BlNullPropertyException($"There is no call with this ID {idCall}");

        // Check if the call is open; throw exception if not.
        if (boCall.Status != BO.StatusTreat.Open && boCall.Status != BO.StatusTreat.RiskOpen)
            throw new BO.BlAlreadyExistsException($"The call is open or expired. IdCall is = {idCall}");

        // Create a new assignment for the volunteer and the call.
        DO.Assignment assigmnetToCreat = new DO.Assignment
        {
            Id = 0, // ID will be generated automatically
            CallId = idCall,
            VolunteerId = idVol,
            TimeStart = AdminManager.Now,
            TimeEnd = null,
            TypeEndTreat = null
        };

        try
        {
            // Try to create the assignment in the database 
            lock (AdminManager.BlMutex) //stage 7
                s_dal.Assignment.Create(assigmnetToCreat);
            VolunteerManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(idVol);
            CallManager.Observers.NotifyListUpdated();
            CallManager.Observers.NotifyItemUpdated(idCall);

        }
        catch (DO.DalDeleteImpossible)
        {
            // Handle error if creation fails.
            throw new BO.BlAlreadyExistsException("Impossible to create the assignment.");
        }
    }

    internal static IEnumerable<BO.OpenCallInList> GetOpenCallHelp(int id, BO.CallType? type, BO.EOpenCallInList? sortBy)
    {
        if (type == BO.CallType.None)
            type = null;

        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = s_dal.Volunteer.Read(id);
        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

        // Retrieve all calls from the BO
        IEnumerable<BO.CallInList> allCalls = GetCallInListsHelp(null, null, null);

        // Retrieve all assignments from the DAL
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex) //stage 7
            calls = s_dal.Call.ReadAll();
        
     

        // Filter for only "Open" or "Risk Open" status
        IEnumerable<BO.OpenCallInList> filteredCalls = from call in allCalls
                                                       where (call.Status == BO.StatusTreat.Open || call.Status == BO.StatusTreat.RiskOpen)
                                                       let boCall = ReadHelp(call.CallId)
                                                       select new BO.OpenCallInList
                                                       {
                                                           Id = call.CallId,
                                                           CType = call.Type,
                                                           Description = boCall.Description,
                                                           FullAddress = boCall.FullAddress,
                                                           TimeOpen = call.TimeOpened,
                                                           MaxTimeToClose = boCall.MaxTimeToClose,
                                                           Status = boCall.Status,
                                                           distanceCallVolunteer = /*volunteer?.FullAddress != null ?*/
                                                          VolunteerManager.CalculateDistance(volunteer.Latitude, volunteer.Longitude, boCall.Latitude, boCall.Longitude) // Calculate the distance between the volunteer and the call

                                                       };
        filteredCalls = from call in filteredCalls
                        where (volunteer.MaxReading == null || volunteer.MaxReading > call.distanceCallVolunteer)
                        select call;


        // Filter by call type if provided
        if (type.HasValue)
        {
            filteredCalls = filteredCalls.Where(c => c.CType == type.Value);
        }

        // Sort by the requested field or by default (call ID)
        if (sortBy.HasValue)
        {
            filteredCalls = sortBy.Value switch
            {
                BO.EOpenCallInList.Id => filteredCalls.OrderBy(c => c.Id),
                BO.EOpenCallInList.CType => filteredCalls.OrderBy(c => c.CType),
                //BO.EOpenCallInList.Description => filteredCalls.OrderBy(c => c.Description),
                BO.EOpenCallInList.FullAddress => filteredCalls.OrderBy(c => c.FullAddress),
                BO.EOpenCallInList.TimeOpen => filteredCalls.OrderBy(c => c.TimeOpen),
                BO.EOpenCallInList.MaxTimeToClose => filteredCalls.OrderBy(c => c.MaxTimeToClose),
                BO.EOpenCallInList.distanceCallVolunteer => filteredCalls.OrderBy(c => c.distanceCallVolunteer),

                _ => filteredCalls.OrderBy(c => c.Id)
            };
        }
        else
        {
            filteredCalls = filteredCalls.OrderBy(c => c.Id);
        }

        return filteredCalls;
    }

    internal static BO.Call ReadHelp(int id)
    {
        Func<DO.Assignment, bool> func = item => item.CallId == id;

        IEnumerable<DO.Assignment> dataOfAssignments;
        lock (AdminManager.BlMutex) //stage 7
            dataOfAssignments = s_dal.Assignment.ReadAll(func);

        DO.Call? doCall;
        lock (AdminManager.BlMutex) //stage 7
            doCall = s_dal.Call.Read(id);
        if (doCall==null)
            throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist");

        List<BO.CallAssignInList>? assignmentsToCalls;
        lock (AdminManager.BlMutex) //stage 7
        {
            assignmentsToCalls = dataOfAssignments.Any() ? dataOfAssignments.Select(assign => new BO.CallAssignInList
            {
                VolunteerId = assign.VolunteerId,
                VolunteerName = s_dal.Volunteer.Read(assign.VolunteerId)?.FullName,
                StartTreat = assign.TimeStart,
                TimeClose = assign.TimeEnd,
                TypeEndTreat = assign.TypeEndTreat == null ? null : (BO.TypeEnd)assign.TypeEndTreat,
            }).ToList() : null;  
        }

        return new()
        {
                Id = id,
                Type = (BO.CallType)doCall.Type,
                Description = doCall.Description,
                FullAddress = doCall.FullAddress,
                Latitude = doCall.Latitude,
                Longitude = doCall.Longitude,
                TimeOpened = doCall.TimeOpened,
                MaxTimeToClose = doCall.MaxTimeToClose,
                Status = CallManager.GetCallStatus(doCall),
                AssignmentsToCalls = assignmentsToCalls
        };
    }
    

    internal static IEnumerable<BO.CallInList> GetCallInListsHelp(BO.ECallInList? filter, object? obj, BO.ECallInList? sortBy)
    {
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex) //stage 7
            // Retrieve all calls from the database, or throw an exception if none exist.
            calls = s_dal.Call.ReadAll();
                if(calls==null)
            throw new BO.BlNullPropertyException("There are no calls in the database");

        // Convert all DO calls to BO calls in list.
        IEnumerable<BO.CallInList> boCallsInList;
        lock (AdminManager.BlMutex) //stage 7
            boCallsInList = s_dal.Call.ReadAll().Select(call => CallManager.ConvertDOCallToBOCallInList(call)).ToList();

        // Apply filter if specified.
        if (filter != null && obj != null)
        {
            switch (filter)
            {
                case BO.ECallInList.Id:
                    boCallsInList = boCallsInList.Where(item => item.Id == (int)obj);
                    break;
                case BO.ECallInList.CallId:
                    boCallsInList = boCallsInList.Where(item => item.CallId == (int)obj);
                    break;
                case BO.ECallInList.CType:
                    boCallsInList = boCallsInList.Where(item => item.Type == (BO.CallType)obj);
                    break;
                case BO.ECallInList.TimeOpened:
                    boCallsInList = boCallsInList.Where(item => item.TimeOpened == (DateTime)obj);
                    break;
                case BO.ECallInList.TimeLeft:
                    boCallsInList = boCallsInList.Where(item => item.TimeLeft == (TimeSpan)obj);
                    break;
                case BO.ECallInList.LastVolunteer:
                    boCallsInList = boCallsInList.Where(item => item.LastVolunteer == (string)obj);
                    break;
                case BO.ECallInList.TotalTime:
                    boCallsInList = boCallsInList.Where(item => item.TotalTime == (TimeSpan)obj);
                    break;
                case BO.ECallInList.Status:
                    if ((BO.StatusTreat)obj == BO.StatusTreat.None)
                        break;
                    boCallsInList = boCallsInList.Where(item => item.Status == (BO.StatusTreat)obj);
                    break;
                case BO.ECallInList.SumAssignment:
                    boCallsInList = boCallsInList.Where(item => item.SumAssignment == (int)obj);
                    break;
            }
        }

        // Default sort by CallId if no sorting is specified.
        if (sortBy == null)
            sortBy = BO.ECallInList.CallId;

        // Apply sorting based on the specified field.
        switch (sortBy)
        {
            case BO.ECallInList.Id:
                boCallsInList = boCallsInList.OrderBy(item => item.Id.HasValue ? 0 : 1)
                                             .ThenBy(item => item.Id)
                                             .ToList();
                break;
            case BO.ECallInList.CallId:
                boCallsInList = boCallsInList.OrderBy(item => item.CallId).ToList();
                break;
            case BO.ECallInList.CType:
                boCallsInList = boCallsInList.OrderBy(item => item.Type).ToList();
                break;
            case BO.ECallInList.TimeOpened:
                boCallsInList = boCallsInList.OrderBy(item => item.TimeOpened).ToList();
                break;
            case BO.ECallInList.TimeLeft:
                boCallsInList = boCallsInList.OrderBy(item => item.TimeLeft).ToList();
                break;
            case BO.ECallInList.LastVolunteer:
                boCallsInList = boCallsInList.OrderBy(item => item.LastVolunteer).ToList();
                break;
            case BO.ECallInList.TotalTime:
                boCallsInList = boCallsInList.OrderBy(item => item.TotalTime).ToList();
                break;
            case BO.ECallInList.Status:
                boCallsInList = boCallsInList.OrderBy(item => item.Status).ToList();
                break;
            case BO.ECallInList.SumAssignment:
                boCallsInList = boCallsInList.OrderBy(item => item.SumAssignment).ToList();
                break;
        }

        // Return the filtered and sorted list of calls.
        return boCallsInList;
    }
    internal static void CloseTreatHelp(int idVol, int idAssig)
    {
        // Retrieve the assignment by its ID; throw an exception if not found.
        DO.Assignment assignmentToClose;
        lock (AdminManager.BlMutex) //stage 7
            assignmentToClose = s_dal.Assignment.Read(idAssig);
        if (assignmentToClose==null)
            throw new BO.BlDeleteNotPossibleException("There is no assignment with this ID");

        // Check if the volunteer matches the one in the assignment; throw an exception if not.
        if (assignmentToClose.VolunteerId != idVol)
        {
            throw new BO.BlWrongInputException("The volunteer is not treating in this assignment");
        }

        // Ensure the assignment is still open (not already closed); throw an exception if it is.
        if (assignmentToClose.TypeEndTreat != null || assignmentToClose.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assignment is not open");

        // Update the assignment to mark it as closed, setting end time and status.
        DO.Assignment assignmentToUP = new DO.Assignment
        {
            Id = assignmentToClose.Id,
            CallId = assignmentToClose.CallId,
            VolunteerId = assignmentToClose.VolunteerId,
            TimeStart = assignmentToClose.TimeStart,
            TimeEnd = AdminManager.Now,
            TypeEndTreat = DO.TypeEnd.Treated,
        };

        try
        {
            // Attempt to update the assignment in the database.
            lock (AdminManager.BlMutex) //stage 7
                   s_dal.Assignment.Update(assignmentToUP);
            VolunteerManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(idVol);
            CallManager.Observers.NotifyListUpdated();
            CallManager.Observers.NotifyItemUpdated(assignmentToClose.CallId);

        }
        catch (DO.DalExistException ex)
        {
            // Handle error if updating the assignment fails.
            throw new BO.BlDeleteNotPossibleException("Cannot update in DO");
        }
    }


    /// <summary>
    /// A method to cancel call treatment
    /// </summary>
    /// <param name="idVol"></param>
    /// <param name="idAssig"></param>
    /// <exception cref="BO.BlDeleteNotPossibleException"></exception>
    internal static void CancelTreatHelp(int idVol, int idAssig)
    {
        DO.Assignment assignmentToCancel;
        lock (AdminManager.BlMutex) //stage 7
            assignmentToCancel = s_dal.Assignment.Read(idAssig);
        if(assignmentToCancel==null)
            throw new BO.BlDeleteNotPossibleException("there is no assigment with this ID");


        //Checks whether the canceler is the owner of the assignment 
        //If not - checks whether the canceler is a manager
        bool isManager = false;
        if (assignmentToCancel.VolunteerId != idVol)
        {
            lock (AdminManager.BlMutex) //stage 7
                if (s_dal.Volunteer.Read(idVol).Job == DO.Role.Boss)
                    isManager = true;
                else throw new BO.BlDeleteNotPossibleException("the volunteer is not manager and also dont treat in this call");
        }

        //Checking that the assignment has not already been closed/cancelled.
        if (assignmentToCancel.TypeEndTreat != null || assignmentToCancel.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assignment has already been closed");

        //Creating a new assignment to update
        DO.Assignment assignmentToUP = new DO.Assignment
        {
            Id = idAssig,
            CallId = assignmentToCancel.CallId,
            VolunteerId = assignmentToCancel.VolunteerId,
            TimeStart = assignmentToCancel.TimeStart,
            TimeEnd = AdminManager.Now,
            TypeEndTreat = isManager ? DO.TypeEnd.ManagerCancel : DO.TypeEnd.SelfCancel,
        };
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                s_dal.Assignment.Update(assignmentToUP);
            VolunteerManager.Observers.NotifyListUpdated();
            //VolunteerManager.Observers.NotifyItemUpdated(idVol);
            CallManager.Observers.NotifyListUpdated();
            CallManager.Observers.NotifyItemUpdated(assignmentToCancel.CallId);
        }
        catch (DO.DalDeleteImpossible ex)
        {
            throw new BO.BlDeleteNotPossibleException("can not delete in DO");
        }
}
   
    ///// <summary>
    ///// Converts an object of type DO.Call to an object of type BO.CallInList
    ///// </summary>
    ///// <param name="doCall"></param>
    ///// <returns> callInList </returns>
    internal static BO.CallInList GetCallsInList(DO.Call doCall)
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
    internal static bool IsInRisk(DO.Call call)
    {
        lock (AdminManager.BlMutex) //stage 7
            return call!.MaxTimeToClose - s_dal.Config.Clock <= s_dal.Config.RiskRange;
    }
    

    /// <summary>
    /// Makes a call to all the calls from the data source (the DAL), filters them according to a condition given as a parameter,
    /// and then returns a list of objects of type BO.CallInList
    /// </summary>
    /// <param name="condition"></param>
    /// <returns> CallInList </returns>
    internal static IEnumerable<BO.CallInList> GetCallsInList(Predicate<DO.Call> condition)
    {
        IEnumerable<BO.CallInList> callsInRisk;
        lock (AdminManager.BlMutex)//stage 7
            callsInRisk= s_dal.Call.ReadAll(call => condition(call)).Select(call => GetCallsInList(call));
        return callsInRisk;
    }

    /// <summary>
    /// Static helper function for retrieving read call
    /// </summary>
    /// <param name="doCall"></param>
    /// <returns> BO.StatusTreat </returns>
    internal static BO.StatusTreat GetCallStatus(DO.Call doCall)
    {
        DateTime now;
        lock (AdminManager.BlMutex) //stage 7
            now = s_dal.Config.Clock;
            if (doCall.MaxTimeToClose < s_dal.Config.Clock)
         return BO.StatusTreat.Expired;
        // var lastAssignment = s_dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id).OrderByDescending(a => a.TimeStart).FirstOrDefault();
        Assignment? lastAssignment;
        lock (AdminManager.BlMutex)//stage 7
             lastAssignment = s_dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id).OrderByDescending(a => a.Id).FirstOrDefault();
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
    //internal static void CheckAddress(BO.Call call)
    //{
    //    double[] coordinates = VolunteerManager.GetCoordinatesAsync(call.FullAddress);
    //    if (coordinates[0] != call.Latitude || coordinates[1] != call.Longitude)
    //        throw new BO.BlWrongItemException($"not math coordinates");
    //}

    /// <summary>
    /// check the call from a logical point of view - correct address and maximum time (which has not yet passed)
    /// </summary>
    internal static void CheckLogic(BO.Call boCall)
    {
        if(boCall.Type==BO.CallType.None)
            throw new BO.BlDeleteNotPossibleException("Type can not be  None  please enter type");
          //  CheckAddress(boCall);
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

        //  int? id = (lastAssignmentsForCall == null) ? null : lastAssignmentsForCall.Id;
        //  int callId = doCall.Id;
        //  BO.CallType type = (BO.CallType)doCall.Type;
        //  DateTime timeOpened = doCall.TimeOpened;
        //  TimeSpan? timeLeft = (doCall.MaxTimeToClose != null) ? doCall.MaxTimeToClose - s_dal.Config.Clock : null;
        //  // string? lastVolunteer = (lastAssignmentsForCall != null) ? s_dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)!.FullName : null;
        //  string? lastVolunteer = (lastAssignmentsForCall != null)
        //? s_dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)?.FullName
        //: null;

        ////  TimeSpan? totalTime = (lastAssignmentsForCall != null && lastAssignmentsForCall.TimeEnd != null) ? lastAssignmentsForCall.TimeEnd - lastAssignmentsForCall.TimeStart : null;

        //int sumAssignment = (assignmentsForCall == null) ? 0 : assignmentsForCall.Count();
        BO.StatusTreat status = GetCallStatus(doCall);
        TimeSpan? timeLeft;
        lock (AdminManager.BlMutex)
             timeLeft = (doCall.MaxTimeToClose != null && doCall.MaxTimeToClose >= s_dal.Config.Clock && status != BO.StatusTreat.Closed) ? doCall.MaxTimeToClose - s_dal.Config.Clock : null;

        string? lastVolunteer;
        lock (AdminManager.BlMutex)
            lastVolunteer = (lastAssignmentsForCall != null) ? s_dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)?.FullName: null;

            return new()
            {
                Id = (lastAssignmentsForCall == null) ? null : lastAssignmentsForCall.Id,
                CallId = doCall.Id,
                Type = (BO.CallType)doCall.Type,
                TimeOpened = doCall.TimeOpened,
                TimeLeft = timeLeft,
                LastVolunteer = lastVolunteer,
                TotalTime = status == BO.StatusTreat.Closed ? lastAssignmentsForCall.TimeEnd - doCall.TimeOpened : null,
                // Status = GetCallStatus(doCall),
                Status = status,
                SumAssignment = (assignmentsForCall == null) ? 0 : assignmentsForCall.Count()
            };
    }

    /// <summary>
    /// convert DO call to BO call
    /// </summary>
    /// <param name="doCall"></param>
    /// <returns> BO.Call </returns>
    internal static BO.Call convertDOtoBO(DO.Call doCall)
    {
        Func<DO.Assignment, bool> func;
        IEnumerable<DO.Assignment> dataOfAssignments;

        func = item => item.CallId == doCall.Id;
        List<BO.CallAssignInList>? assignmentsToCalls;
        lock (AdminManager.BlMutex)      //stage 7
        {
           dataOfAssignments = s_dal.Assignment.ReadAll(func);
           
            assignmentsToCalls = dataOfAssignments.Any()
          ? dataOfAssignments.Select(assign => new BO.CallAssignInList
          {
              VolunteerId = assign.VolunteerId,
              VolunteerName = s_dal.Volunteer.Read(assign.VolunteerId)?.FullName,
              StartTreat = assign.TimeStart,
              TimeClose = assign.TimeEnd,
              TypeEndTreat = assign.TypeEndTreat == null ? null : (BO.TypeEnd)assign.TypeEndTreat,
          }).ToList(): null;
        }

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
                AssignmentsToCalls = assignmentsToCalls,
            };
    }
    

    /// <summary>
    /// The `UpdateExpiredCalls` function is responsible for updating the status of expired calls (calls whose maximum time to close has passed).
    /// </summary>
    internal static void UpdateExpiredCalls()
    {
        bool assignmentUpdated = false; //stage 5
        IEnumerable<DO.Call> calls;
        IEnumerable<BO.Call> boCalls;
        lock (AdminManager.BlMutex)
        {
            calls = s_dal.Call.ReadAll().ToList();
             boCalls = from dCall in calls
                           //where (dCall.MaxTimeToClose == null ? true : dCall.MaxTimeToClose < s_dal.Config.Clock)
                       where dCall.MaxTimeToClose != null && dCall.MaxTimeToClose < s_dal.Config.Clock
                                 select (convertDOtoBO(dCall));

           
         
        }
        Assignment? assignment;
        foreach (BO.Call call in boCalls)
        {
            if (call.AssignmentsToCalls == null)
            {
                assignmentUpdated = true; //stage 5
                lock (AdminManager.BlMutex)
                    s_dal.Assignment.Create(new DO.Assignment(0, call.Id, 0, s_dal.Config.Clock, s_dal.Config.Clock, DO.TypeEnd.ExpiredCancel));
                  Observers.NotifyItemUpdated(call.Id); //stage 5
            }
            else
            {
                var lastAss = call.AssignmentsToCalls.OrderByDescending(a => a.StartTreat).First();
                if (lastAss.TypeEndTreat == null)
                {
                    assignmentUpdated = true; //stage 5
                    lock (AdminManager.BlMutex)
                         assignment = s_dal.Assignment.Read(a => a.VolunteerId == lastAss.VolunteerId && a.TimeEnd == null && a.TypeEndTreat == null);

                    lock (AdminManager.BlMutex)
                        s_dal.Assignment.Update(new DO.Assignment(assignment.Id, assignment.CallId, assignment.VolunteerId, lastAss.StartTreat, s_dal.Config.Clock, DO.TypeEnd.ExpiredCancel));
                    Observers.NotifyItemUpdated(call.Id); //stage 5
                    VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId); //stage 5
                }
                
            }
        }
        if (assignmentUpdated)
        {
            Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyListUpdated();
        }
        //  Observers.NotifyListUpdated(assignment.Id);
    }
}
