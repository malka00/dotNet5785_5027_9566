using BlApi;
using System;
using Helpers;
using System.Collections.Generic;
using BO;
using DO;
namespace BlImplementation;

/// <summary>
/// Implementation of the methods of call
/// </summary>
internal class CallImplementation : ICall
{

    #region Stage 5
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;   //stage 4

    /// <summary>
    /// The CancelTreat function performs the action of canceling an assignment (Assignment) for a volunteer (Volunteer). 
    /// It is executed by a specific volunteer (identified by the ID idVol) and relates to a specific assignment (identified by the ID idAssig).
    /// </summary>
    /// <param name="idVol"></param>
    /// <param name="idAssig"></param>
    /// <exception cref="BO.BlDeleteNotPossibleException"></exception>
    public void CancelTreat(int idVol, int idAssig)
    {  
        DO.Assignment assigmnetToCancel = _dal.Assignment.Read(idAssig) ?? throw new BO.BlDeleteNotPossibleException("there is no assigment with this ID");
        bool ismanager=false;
        if (assigmnetToCancel.VolunteerId != idVol)
        {
            if (_dal.Volunteer.Read(idVol).Job == DO.Role.Boss)
                ismanager = true;
            else throw new BO.BlDeleteNotPossibleException("the volunteer is not manager or not in this call");
        }
        if (assigmnetToCancel.TypeEndTreat != null ||/* (_dal.Call.Read(assigmnetToCancel.CallId).MaxTimeToClose > AdminManager.Now)||*/ assigmnetToCancel.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assigmnet not in treat");

        DO.Assignment assigmnetToUP = new DO.Assignment 
        {
            Id = assigmnetToCancel.Id,
            CallId = assigmnetToCancel.CallId,
            VolunteerId =assigmnetToCancel.VolunteerId,
             TimeStart=assigmnetToCancel.TimeStart,
             TimeEnd = AdminManager.Now,
            TypeEndTreat = ismanager?DO.TypeEnd.ManagerCancel:DO.TypeEnd.SelfCancel,
          
        };
        try
        {
            _dal.Assignment.Update(assigmnetToUP);
            VolunteerManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(idVol);
        }
        catch ( DO.DalExistException ex)
        {
            throw new BO.BlDeleteNotPossibleException("can not delete in DO");
        }
        
    }

    /// <summary>
    /// The function assigns an assignment (task) to a volunteer for a specific call.
    /// </summary>
    /// <param name="idVol"></param>
    /// <param name="idCall"></param>
    /// <exception cref="BO.BlNullPropertyException"></exception>
    /// <exception cref="BO.BlAlreadyExistsException"></exception>
    public void ChoseForTreat(int idVol, int idCall)
    {
        // Retrieve volunteer and call; throw exception if not found.
        DO.Volunteer vol = _dal.Volunteer.Read(idVol) ?? throw new BO.BlNullPropertyException($"There is no volunteer with this ID {idVol}");
        BO.Call boCall = Read(idCall) ?? throw new BO.BlNullPropertyException($"There is no call with this ID {idCall}");

        // Check if the call is open; throw exception if not.
        if (boCall.Status != BO.StatusTreat.Open || boCall.Status == BO.StatusTreat.RiskOpen)
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
            // Try to create the assignment in the database.
            _dal.Assignment.Create(assigmnetToCreat);
            VolunteerManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(idVol);

        }
        catch (DO.DalDeleteImpossible)
        {
            // Handle error if creation fails.
            throw new BO.BlAlreadyExistsException("Impossible to create the assignment.");
        }
    }

    /// <summary>
    /// The function closes an assignment for a volunteer. 
    /// </summary>
    /// <param name="idVol"></param>
    /// <param name="idAssig"></param>
    /// <exception cref="BO.BlDeleteNotPossibleException"></exception>
    /// <exception cref="BO.BlWrongInputException"></exception>
    public void CloseTreat(int idVol, int idAssig)
    {
        // Retrieve the assignment by its ID; throw an exception if not found.
        DO.Assignment assignmentToClose = _dal.Assignment.Read(idAssig) ?? throw new BO.BlDeleteNotPossibleException("There is no assignment with this ID");

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
            CallId=assignmentToClose.CallId,
            VolunteerId=assignmentToClose.VolunteerId,
            TimeStart = assignmentToClose.TimeStart,
            TimeEnd = AdminManager.Now,
            TypeEndTreat = DO.TypeEnd.Treated,
        };

        try
        {
            // Attempt to update the assignment in the database.
            _dal.Assignment.Update(assignmentToUP);
            VolunteerManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(idVol);
        }
        catch (DO.DalExistException ex)
        {
            // Handle error if updating the assignment fails.
            throw new BO.BlDeleteNotPossibleException("Cannot update in DO");
        }
    }

    /// <summary>
    ///The function counts how many calls exist for each status defined in the BO.StatusTreat enum and returns an array with the count for each status.
    /// </summary>
    /// <returns> int[] </returns>
    public int[] CountCall()
    {
        IEnumerable<DO.Call>? calls = _dal.Call.ReadAll() ?? Enumerable.Empty<DO.Call>();

        // Gets the number of statuses (based on Enum)
        int statusCount = Enum.GetValues(typeof(BO.StatusTreat)).Length;

        // Creates an array of size equal to the number of statuses
        int[] count = new int[statusCount];

        // Groups the calls by status and updates the array
        var groupedCounts = from call in calls
                            let status = CallManager.GetCallStatus(call)
                            group call by status into groupedCalls
                            select new { Status = groupedCalls.Key, Count = groupedCalls.Count() };

        foreach (var group in groupedCounts)
        {
            int statusIndex = (int)group.Status; // Explicitly cast the Enum to int

            if (statusIndex >= 0 && statusIndex < statusCount)
            {
                count[statusIndex] = group.Count;
            }
        }
        return count;
    }

    /// <summary>
    /// add a new call
    /// </summary>
    public void Create(BO.Call boCall)
    {
        double[] coordinate = VolunteerManager.GetCoordinates(boCall.FullAddress);
        double latitude = coordinate[0];
        double longitude = coordinate[1];
        boCall.Latitude = latitude;
        boCall.Longitude = longitude;
        try
        {
            CallManager.CheckLogic(boCall);
            DO.Call doCall = new
                (
                boCall.Id,
                (DO.CallType)boCall.Type,
                boCall.Description,
                boCall.FullAddress,
                latitude,
                longitude,
                boCall.TimeOpened,
                boCall.MaxTimeToClose
                );
            _dal.Call.Create(doCall);
        }
        catch (DO.DalExistException ex)
        {
            throw new BO.BlAlreadyExistsException($"Student with ID={boCall.Id} already exists", ex);
        }
        CallManager.Observers.NotifyListUpdated();  //stage 5 
    }

    /// <summary>
    /// Delete a call
    /// </summary>
    public void Delete(int id)
    {
        try
        {
            if ((Read(id).Status == BO.StatusTreat.Open)&&(Read(id).AssignmentsToCalls ==null))
            {
                _dal.Call.Delete(id);
            }
            else
            {
                throw new BO.BlDeleteNotPossibleException($"Call {id} can not be deleted");
            }
        }
        catch (DO.DalExistException ex)
        {
            throw new BO.BlDeleteNotPossibleException($"Call {id} does not exist");
        }
        CallManager.Observers.NotifyListUpdated();  //stage 5 
    }

  public  bool CanDelete(int id)
    {
        return (Read(id).Status == BO.StatusTreat.Open) && (Read(id).AssignmentsToCalls == null);
    }

    /// <summary>
    /// The function retrieves calls from the database, applies filters if specified, 
    /// sorts them by a given field, and then returns the result as a list of CallInList objects.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="obj"></param>
    /// <param name="sortBy"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlNullPropertyException"></exception>
    public IEnumerable<BO.CallInList> GetCallInLists(BO.ECallInList? filter, object? obj, BO.ECallInList? sortBy)
    {
        // Retrieve all calls from the database, or throw an exception if none exist.
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are no calls in the database");

        // Convert all DO calls to BO calls in list.
        IEnumerable<BO.CallInList> boCallsInList = _dal.Call.ReadAll().Select(call => CallManager.ConvertDOCallToBOCallInList(call));

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

    /// <summary>
    /// The function retrieves closed calls for a specific volunteer, applies filters based on the volunteer's ID and optional call type, 
    /// sorts the results by a specified field, and returns the filtered and sorted list of closed calls.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="sortBy"></param>
    /// <returns> BO.ClosedCallInList </returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCall(int id, BO.CallType? type, BO.EClosedCallInList? sortBy)
    {
        // Retrieve all calls from the DAL
        var allCalls = _dal.Call.ReadAll();

        // Retrieve all assignments from the DAL
        var allAssignments = _dal.Assignment.ReadAll();

        // Filter by volunteer ID and closed status (calls that have an end treatment type)
        IEnumerable<BO.ClosedCallInList> filteredCalls = from call in allCalls
                                                         join assignment in allAssignments
                                                         on call.Id equals assignment.CallId
                                                         where assignment.VolunteerId == id && assignment.TypeEndTreat != null
                                                         select new BO.ClosedCallInList
                                                         {
                                                             Id = call.Id,
                                                             Type = (BO.CallType)call.Type,
                                                             FullAddress = call.FullAddress,
                                                             TimeOpen = call.TimeOpened,
                                                             StartTreat = assignment.TimeStart,
                                                             TimeClose = assignment.TimeEnd,
                                                             TypeEndTreat = (BO.TypeEnd)assignment.TypeEndTreat
                                                         };

        // Filter by call type if provided
        if (type.HasValue)
        {
            filteredCalls = filteredCalls.Where(c => c.Type == type.Value);
        }

        // Sort by the requested field or by default (call ID)
        if (sortBy.HasValue)
        {
            filteredCalls = sortBy.Value switch
            {
                BO.EClosedCallInList.Id => filteredCalls.OrderBy(c => c.Id),
                BO.EClosedCallInList.CType => filteredCalls.OrderBy(c => c.Type),
                BO.EClosedCallInList.FullAddress => filteredCalls.OrderBy(c => c.FullAddress),
                BO.EClosedCallInList.TimeOpen => filteredCalls.OrderBy(c => c.TimeOpen),
                BO.EClosedCallInList.StartTreat => filteredCalls.OrderBy(c => c.StartTreat),
                BO.EClosedCallInList.TimeClose => filteredCalls.OrderBy(c => c.TimeClose),
                BO.EClosedCallInList.TypeEndTreat => filteredCalls.OrderBy(c => c.TypeEndTreat),
                _ => filteredCalls.OrderBy(c => c.Id)
            };
        }
        else
        {
            filteredCalls = filteredCalls.OrderBy(c => c.Id);
        }

        return filteredCalls;
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.CallType? type, BO.EOpenCallInList? sortBy)
    {
        if (type == BO.CallType.None)
            type = null;
        DO.Volunteer volunteer = _dal.Volunteer.Read(id);
        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

        // Retrieve all calls from the BO
        IEnumerable<BO.CallInList> allCalls = GetCallInLists(null, null, null);

        // Retrieve all assignments from the DAL
        var calls = _dal.Call.ReadAll();
        double lonVol = (double)volunteer.Longitude;
        double latVol = (double)volunteer.Latitude;

        // Filter for only "Open" or "Risk Open" status
        IEnumerable<BO.OpenCallInList> filteredCalls = from call in allCalls
                                                       where (call.Status == BO.StatusTreat.Open || call.Status == BO.StatusTreat.RiskOpen)
                                                       let boCall = Read(call.CallId)
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
                                                          VolunteerManager.CalculateDistance(latVol, lonVol, boCall.Latitude, boCall.Longitude)/* : 0*/  // Calculate the distance between the volunteer and the call
          
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


    /// <summary>
    /// This function retrieves open or risk-open calls assigned to a specific volunteer, filters them based on optional call type and sorting criteria, 
    /// and returns the filtered and sorted list, including information such as call details, distance from the volunteer, and call status.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="sortBy"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    //public IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.CallType? type, BO.EOpenCallInList? sortBy)
    //{
    //    DO.Volunteer volunteer = _dal.Volunteer.Read(id);
    //    if (volunteer == null)
    //        throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

    //    // Retrieve all calls from the BO
    //    IEnumerable<BO.CallInList> allCalls = GetCallInLists(null, null, null);

    //    // Retrieve all assignments from the DAL
    //    var calls = _dal.Assignment.ReadAll();
    //    double lonVol = (double)volunteer.Longitude;
    //    double latVol = (double)volunteer.Latitude;

    //    // Filter for only "Open" or "Risk Open" status
    //    IEnumerable<BO.OpenCallInList> filteredCalls = from call in allCalls
    //                                                   join assignment in allAssignments on call.Id equals assignment.CallId into callAssignments
    //                                                   from assignment in callAssignments.DefaultIfEmpty()
    //                                                   where (call.Status == BO.StatusTreat.Open || call.Status == BO.StatusTreat.RiskOpen)
    //                                                   let boCall = Read(call.CallId)
    //                                                   select new BO.OpenCallInList
    //                                                   {
    //                                                       Id = call.CallId,
    //                                                       CType = call.Type,
    //                                                       Description = boCall.Description,
    //                                                       FullAddress = boCall.FullAddress,
    //                                                       TimeOpen = call.TimeOpened,
    //                                                       MaxTimeToClose = boCall.MaxTimeToClose,
    //                                                       distanceCallVolunteer = volunteer?.FullAddress != null ? VolunteerManager.CalculateDistance
    //                                                       (latVol, lonVol, boCall.Latitude, boCall.Longitude) : 0  // Calculate the distance between the volunteer and the call
    //                                                   };

    //    // Filter by call type if provided
    //    if (type.HasValue)
    //    {
    //        filteredCalls = filteredCalls.Where(c => c.CType == type.Value);
    //    }

    //    // Sort by the requested field or by default (call ID)
    //    if (sortBy.HasValue)
    //    {
    //        filteredCalls = sortBy.Value switch
    //        {
    //            BO.EOpenCallInList.Id => filteredCalls.OrderBy(c => c.Id),
    //            BO.EOpenCallInList.CType => filteredCalls.OrderBy(c => c.CType),
    //            BO.EOpenCallInList.Description => filteredCalls.OrderBy(c => c.Description),
    //            BO.EOpenCallInList.FullAddress => filteredCalls.OrderBy(c => c.FullAddress),
    //            BO.EOpenCallInList.TimeOpen => filteredCalls.OrderBy(c => c.TimeOpen),
    //            BO.EOpenCallInList.MaxTimeToClose => filteredCalls.OrderBy(c => c.MaxTimeToClose),
    //            BO.EOpenCallInList.distanceCallVolunteer => filteredCalls.OrderBy(c => c.distanceCallVolunteer),
    //            _ => filteredCalls.OrderBy(c => c.Id)
    //        };
    //    }
    //    else
    //    {
    //        filteredCalls = filteredCalls.OrderBy(c => c.Id);
    //    }

    //    return filteredCalls;
    //}




    //IEnumerable<OpenCallInList> BlApi.ICall.ReadOpenCallsVolunteer(int id, BO.CallType? callT, FiledOfOpenCallInList? filedTosort)
    //{


    //    IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
    //    List<BO.OpenCallInList> Calls = new List<BO.OpenCallInList>();

    //    Calls.AddRange(from item in previousCalls
    //                   let DataCall = ReadCall(item.ID)
    //                   where DataCall.statusC == BO.Status.Open || DataCall.statusC == BO.Status.OpenInRisk
    //                   let volunteerData = _dal.Volunteer.Read(v => v.ID == id)
    //                   where volunteerData.maxDistance == null ? true : volunteerData.maxDistance >= Tools.CalculateDistance(volunteerData.Latitude ?? DataCall.latitude, volunteerData.Longitude ?? DataCall.longitude, DataCall.latitude, DataCall.longitude, (BO.Distance)volunteerData.distanceType)
    //                   select CallsManager.ConvertDOCallToBOOpenCallInList(item, id));

    //    IEnumerable<BO.OpenCallInList> openCallInLists = Calls;

    //    if (callT != null)
    //    {
    //        openCallInLists = openCallInLists.Where(c => c.callT == callT);
    //    }

    //    if (filedTosort != null)
    //    {
    //        switch (filedTosort)
    //        {
    //            case BO.FiledOfOpenCallInList.ID:
    //                openCallInLists = openCallInLists.OrderBy(item => item.ID);
    //                break;
    //            case BO.FiledOfOpenCallInList.address:
    //                openCallInLists = openCallInLists.OrderBy(item => item.address);
    //                break;
    //            case BO.FiledOfOpenCallInList.callT:
    //                openCallInLists = openCallInLists.OrderBy(item => item.callT);
    //                break;
    //            case BO.FiledOfOpenCallInList.openTime:
    //                openCallInLists = openCallInLists.OrderBy(item => item.openTime);
    //                break;
    //            case BO.FiledOfOpenCallInList.maxTime:
    //                openCallInLists = openCallInLists.OrderBy(item => item.maxTime);
    //                break;
    //            case BO.FiledOfOpenCallInList.verbalDescription:
    //                openCallInLists = openCallInLists.OrderBy(item => item.verbalDescription);
    //                break;
    //        }

    //    }

    //    return openCallInLists;
    //}

    /// <summary>
    /// The Read function retrieves a call by its ID and maps the data from the database (DO objects) into a business object (BO). 
    /// It also retrieves the assignments associated with that call and returns them along with the call details in a structured format.
    /// </summary>
    public BO.Call Read(int id)
    {
        Func<DO.Assignment, bool> func = item => item.CallId == id;
        IEnumerable<DO.Assignment> dataOfAssignments = _dal.Assignment.ReadAll(func);


        var doCall = _dal.Call.Read(id) ?? throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist");

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
            AssignmentsToCalls = dataOfAssignments.Any()
    ? dataOfAssignments.Select(assign => new BO.CallAssignInList
    {
        VolunteerId = assign.VolunteerId,
        VolunteerName = _dal.Volunteer.Read(assign.VolunteerId)?.FullName,
        StartTreat = assign.TimeStart,
        TimeClose = assign.TimeEnd,
        TypeEndTreat = assign.TypeEndTreat == null ? null : (BO.TypeEnd)assign.TypeEndTreat,
    }).ToList()
    : null,

        };
        }

    /// <summary>
    /// Updating the reading and checking whether there was an exception from the DAL layer
    /// </summary>
    public void Update(BO.Call boCall)
    {
        double[] coordinate = VolunteerManager.GetCoordinates(boCall.FullAddress);
        double latitude = coordinate[0];
        double longitude = coordinate[1];
        boCall.Latitude = latitude;
        boCall.Longitude = longitude;
        CallManager.CheckLogic(boCall);
        DO.Call doCall = new
                    (
                    boCall.Id,
                    (DO.CallType)boCall.Type,
                    boCall.Description,
                    boCall.FullAddress,
                    boCall.Latitude,
                    boCall.Longitude,
                    boCall.TimeOpened,
                    boCall.MaxTimeToClose
                    );
        try
        {
            _dal.Call.Update(doCall);
        }
        catch (DO.DalDeleteImpossible ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={boCall.Id} does Not exist", ex);
        }
        CallManager.Observers.NotifyItemUpdated(doCall.Id);  //stage 5
        CallManager.Observers.NotifyListUpdated();  //stage 5
    }
}


