using System;
using System.ComponentModel.Design;
using System.Net;
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

    /// <summary>
    /// An asynchronous method for updating the address of a call
    /// </summary>
    /// <param name="doCall"></param>
    /// <param name="newAddress"></param>
    /// <returns>Task</returns>
    internal static async Task updateCoordinatesForCallsAddressAsync(DO.Call doCall, string newAddress)
    {
        if (newAddress == null)
            newAddress = "";

        double[] coordinates;

        //Coordinate search. Occurs asynchronously
        coordinates = await Tools.GetCoordinatesAsync(newAddress);   

        //The address was not found
        if (coordinates == null || coordinates.Length != 2)
            doCall = doCall with { Latitude = null, Longitude = null };
        
        //The address is found and longitude and latitude are given values
        else
            doCall = doCall with { Latitude = coordinates[0], Longitude = coordinates[1] };

        lock (AdminManager.BlMutex)
            s_dal.Call.Update(doCall);
        Observers.NotifyListUpdated();
        Observers.NotifyItemUpdated(doCall.Id);
    }

    /// <summary>
    /// A method that assigns call processing to a specific volunteer
    /// </summary>
    /// <param name="idVol"></param>
    /// <param name="idCall"></param>
    /// <exception cref="BO.BlNullPropertyException"></exception>
    /// <exception cref="BO.BlAlreadyExistsException"></exception>
    internal static void ChoseForTreatHelp(int idVol, int idCall)
    {
        // Retrieve volunteer and call; throw exception if not found.
        DO.Volunteer vol;
        lock (AdminManager.BlMutex) //stage 7
            vol = s_dal.Volunteer.Read(idVol);
        if (vol == null)
            throw new BO.BlNullPropertyException($"There is no volunteer with this ID {idVol}");
        BO.Call boCall = ReadHelp(idCall) ?? throw new BO.BlNullPropertyException($"There is no call with this ID {idCall}");

        // Check if the call is open; throw exception if not.
        if (boCall.Status != BO.StatusTreat.Open && boCall.Status != BO.StatusTreat.RiskOpen)
            throw new BO.BlAlreadyExistsException($"The call is not open.");

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

    /// <summary>
    /// A method that returns all open calls, filters them by call type, and sorts them by a selected field
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="sortBy"></param>
    /// <returns> IEnumerable<BO.OpenCallInList> </returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
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
                                                           distanceCallVolunteer = /*volunteer?.FullAddress != null ?*/
                                                           VolunteerManager.CalculateDistance(volunteer.Latitude, volunteer.Longitude, boCall.Latitude, boCall.Longitude) // Calculate the distance between the volunteer and the call
                                                       };

        // Filter by maximum reading range of the volunteer
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
                BO.EOpenCallInList.FullAddress => filteredCalls.OrderBy(c => c.FullAddress),
                BO.EOpenCallInList.TimeOpen => filteredCalls.OrderBy(c => c.TimeOpen),
                BO.EOpenCallInList.MaxTimeToClose => filteredCalls.OrderBy(c => c.MaxTimeToClose),
                BO.EOpenCallInList.distanceCallVolunteer => filteredCalls.OrderBy(c => c.distanceCallVolunteer),
                _ => filteredCalls.OrderBy(c => c.Id)
            };
        }

        // If no value is passed to sort, the default is to sort by ID
        else
        {
            filteredCalls = filteredCalls.OrderBy(c => c.Id);
        }

        return filteredCalls;
    }

    /// <summary>
    /// A method for reading a call by ID value
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    internal static BO.Call ReadHelp(int id)
    {
        //Defining a function to filter the assignments
        Func<DO.Assignment, bool> func = item => item.CallId == id;

        // Retrieving all allocations that are suitable for this call
        IEnumerable<DO.Assignment> dataOfAssignments;
        lock (AdminManager.BlMutex) //stage 7
            dataOfAssignments = s_dal.Assignment.ReadAll(func);

        // Reads the reading from the DAL layer
        DO.Call? doCall;
        lock (AdminManager.BlMutex) //stage 7
            doCall = s_dal.Call.Read(id);
        if (doCall == null)
            throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist");

        // Retrieving the assignments and creating a list of BO.CallAssignInList
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

        // Returning a BO.Call object
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

    /// <summary>
    /// A method that returns a list of calls with an option to sort and filter according to the selected value
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="obj"></param>
    /// <param name="sortBy"></param>
    /// <returns> IEnumerable<BO.CallInList> </returns>
    /// <exception cref="BO.BlNullPropertyException"></exception>
    internal static IEnumerable<BO.CallInList> GetCallInListsHelp(BO.ECallInList? filter, object? obj, BO.ECallInList? sortBy)
    {
        // Retrieve all calls from the database, or throw an exception if none exist
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex) //stage 7
            calls = s_dal.Call.ReadAll();
        if (calls == null)
            throw new BO.BlNullPropertyException("There are no calls");

        // Convert all DO calls to BO calls in list
        IEnumerable<BO.CallInList> boCallsInList;
        lock (AdminManager.BlMutex) //stage 7
            boCallsInList = s_dal.Call.ReadAll().Select(call => ConvertDOCallToBOCallInList(call)).ToList();

        // Apply filter if specified
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

    /// <summary>
    /// A method for updating the completion of call treatment
    /// </summary>
    /// <param name="idVol"></param>
    /// <param name="idAssig"></param>
    /// <exception cref="BO.BlDeleteNotPossibleException"></exception>
    /// <exception cref="BO.BlWrongInputException"></exception>
    internal static void CloseTreatHelp(int idVol, int idAssig)
    {
        // Retrieve the assignment by its ID; throw an exception if not found.
        DO.Assignment assignmentToClose;
        lock (AdminManager.BlMutex) //stage 7
            assignmentToClose = s_dal.Assignment.Read(idAssig);
        if (assignmentToClose == null)
            throw new BO.BlDeleteNotPossibleException("There is no assignment with this ID");

        // Check if the volunteer matches the one in the assignment; throw an exception if not.
        if (assignmentToClose.VolunteerId != idVol)
            throw new BO.BlWrongInputException("The volunteer is not treating in this assignment");
      
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

        // Attempt to update the assignment in the database.
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                s_dal.Assignment.Update(assignmentToUP);
            VolunteerManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(idVol);
            CallManager.Observers.NotifyListUpdated();
            CallManager.Observers.NotifyItemUpdated(assignmentToClose.CallId);
        }

        // Handle error if updating the assignment fails.
        catch (DO.DalExistException ex)
        { 
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
        if (assignmentToCancel == null)
            throw new BO.BlDeleteNotPossibleException("there is no assigment with this ID");


        // Checks whether the canceler is the owner of the assignment 
        // If not - checks whether the canceler is a manager
        bool isManager = false;
        if (assignmentToCancel.VolunteerId != idVol)
        {
            lock (AdminManager.BlMutex) //stage 7
                if (s_dal.Volunteer.Read(idVol).Job == DO.Role.Boss)
                    isManager = true;
                else throw new BO.BlDeleteNotPossibleException("the volunteer is not manager and also dont treat in this call");
        }

        // Checking that the assignment has not already been closed/cancelled.
        if (assignmentToCancel.TypeEndTreat != null || assignmentToCancel.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assignment has already been closed");

        // Creating a new assignment to update
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
            VolunteerManager.Observers.NotifyItemUpdated(assignmentToCancel.VolunteerId);
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
        bool isInRisk;
        lock (AdminManager.BlMutex) //stage 7
            isInRisk = call!.MaxTimeToClose - s_dal.Config.Clock <= s_dal.Config.RiskRange;
        return isInRisk;
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
            callsInRisk = s_dal.Call.ReadAll(call => condition(call)).Select(call => GetCallsInList(call));
        return callsInRisk;
    }

    /// <summary>
    /// Static helper function for retrieving status call
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

        // Retrieving the last allocation of the call
        Assignment? lastAssignment;
        lock (AdminManager.BlMutex)//stage 7
            lastAssignment = s_dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id).OrderByDescending(a => a.Id).FirstOrDefault();

        // Read test without assignment
        if (lastAssignment == null)
        {
            if (IsInRisk(doCall!))
                return BO.StatusTreat.RiskOpen;
            else return BO.StatusTreat.Open;
        }

        // Checking the treatment completion status of the last assignment
        if (lastAssignment.TypeEndTreat == DO.TypeEnd.Treated)
            return BO.StatusTreat.Closed;

        // Checking if the assignment is still open
        if (lastAssignment.TypeEndTreat == null)
        {
            if (IsInRisk(doCall!))
                return BO.StatusTreat.TreatInRisk;
            else return BO.StatusTreat.Treat;
        }

        // Checking self-cancellation or cancellation by an administrator
        if (lastAssignment.TypeEndTreat.ToString() == "SelfCancel" || lastAssignment.TypeEndTreat.ToString() == "ManagerCancel")
        {
            if (IsInRisk(doCall!))
                return BO.StatusTreat.RiskOpen;
            else return BO.StatusTreat.Open;
        }

        return BO.StatusTreat.Closed;  //default
    }

    /// <summary>
    /// check the call from a logical point of view - maximum time (which has not yet passed)
    /// </summary>
    internal static void CheckLogic(BO.Call boCall)
    {
        if (boCall.Type == BO.CallType.None)
            throw new BO.BlDeleteNotPossibleException("Please enter a call type");

        if ((boCall.MaxTimeToClose <= boCall.TimeOpened))
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
        // Reading assignments related to this call
        IEnumerable<Assignment>? assignmentsForCall;
        lock (AdminManager.BlMutex)
            assignmentsForCall = s_dal.Assignment.ReadAll(a => a.CallId == doCall.Id);

        // Setting the last assignment
        var lastAssignmentsForCall = assignmentsForCall.OrderByDescending(item => item.TimeStart).FirstOrDefault();

        // Determining the call status
        BO.StatusTreat status = GetCallStatus(doCall);

        // Calculation of time left (TimeLeft)
        TimeSpan? timeLeft;
        lock (AdminManager.BlMutex)
            timeLeft = (doCall.MaxTimeToClose != null && doCall.MaxTimeToClose >= s_dal.Config.Clock && status != BO.StatusTreat.Closed) ? doCall.MaxTimeToClose - s_dal.Config.Clock : null;

        // Determining the name of the last volunteer
        string? lastVolunteer;
        lock (AdminManager.BlMutex)
            lastVolunteer = (lastAssignmentsForCall != null) ? s_dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)?.FullName : null;

        // Returns an object of type BO.CallInList
        return new()
        {
            Id = (lastAssignmentsForCall == null) ? null : lastAssignmentsForCall.Id,
            CallId = doCall.Id,
            Type = (BO.CallType)doCall.Type,
            TimeOpened = doCall.TimeOpened,
            TimeLeft = timeLeft,
            LastVolunteer = lastVolunteer,
            TotalTime = status == BO.StatusTreat.Closed ? lastAssignmentsForCall.TimeEnd - doCall.TimeOpened : null,
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
        // Define filter function for assignments
        Func<DO.Assignment, bool> func;
        IEnumerable<DO.Assignment> dataOfAssignments;

        // Assign filtering function for matching CallId
        func = item => item.CallId == doCall.Id;
        List<BO.CallAssignInList>? assignmentsToCalls;

        lock (AdminManager.BlMutex)      //stage 7
        {
            // Retrieve all assignments related to the call
            dataOfAssignments = s_dal.Assignment.ReadAll(func);

            // Map assignments to BO.CallAssignInList if available
            assignmentsToCalls = dataOfAssignments.Any()
                ? dataOfAssignments.Select(assign => new BO.CallAssignInList
                {
                    VolunteerId = assign.VolunteerId,
                    VolunteerName = s_dal.Volunteer.Read(assign.VolunteerId)?.FullName,
                    StartTreat = assign.TimeStart,
                    TimeClose = assign.TimeEnd,
                    TypeEndTreat = assign.TypeEndTreat == null ? null : (BO.TypeEnd)assign.TypeEndTreat,
                }).ToList() : null;
        }

        // Return a new BO.Call object with the converted data
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

        // The selection of expired readings
        lock (AdminManager.BlMutex)   //stage 7
        {
            calls = s_dal.Call.ReadAll().ToList();
            boCalls = from dCall in calls
                      where dCall.MaxTimeToClose != null && dCall.MaxTimeToClose < s_dal.Config.Clock
                      select (convertDOtoBO(dCall));
        }

        // Updating the assignments of the calls
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

        //observers
        if (assignmentUpdated)
        {
            Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyListUpdated();
        }
    }
}
