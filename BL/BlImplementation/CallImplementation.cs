using BlApi;
using System;
using Helpers;
using System.Collections.Generic;

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
        AdminManager.ThrowOnSimulatorIsRunning();

        CallManager.CancelTreatHelp(idVol, idAssig);
        
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
        AdminManager.ThrowOnSimulatorIsRunning();

        CallManager.ChoseForTreatHelp(idVol,idCall);
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
        AdminManager.ThrowOnSimulatorIsRunning();

        CallManager.CloseTreatHelp(idVol, idAssig);
    }

    /// <summary>
    ///The function counts how many calls exist for each status defined in the BO.StatusTreat enum and returns an array with the count for each status.
    /// </summary>
    /// <returns> int[] </returns>
    public int[] CountCall()
    {
        IEnumerable<DO.Call>? calls;
        lock (AdminManager.BlMutex) //stage 7
            calls = _dal.Call.ReadAll() ?? Enumerable.Empty<DO.Call>();

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
        AdminManager.ThrowOnSimulatorIsRunning();
        //double[] coordinate = VolunteerManager.GetCoordinatesAsync(boCall.FullAddress);
        //double latitude = coordinate[0];
        //double longitude = coordinate[1];
        //boCall.Latitude = latitude;
        //boCall.Longitude = longitude;
        DO.Call doCall;
        try
        {
            CallManager.CheckLogic(boCall);
             doCall = new
                (
                boCall.Id,
                (DO.CallType)boCall.Type,
                boCall.Description,
                boCall.FullAddress,
                null,
                null,
                boCall.TimeOpened,
                boCall.MaxTimeToClose
                );
            lock (AdminManager.BlMutex) //stage 7
                _dal.Call.Create(doCall);
        }
        catch (DO.DalExistException ex)
        {
            throw new BO.BlAlreadyExistsException($"Student with ID={boCall.Id} already exists", ex);
        }

        CallManager.Observers.NotifyItemUpdated(doCall.Id);  //stage 5
        CallManager.Observers.NotifyListUpdated();  //stage 5
       // _ = CallManager.updateCoordinatesForCallsAddressAsync(doCall);
    }

    /// <summary>
    /// Delete a call
    /// </summary>
    public void Delete(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            if ((Read(id).Status == BO.StatusTreat.Open)&&(Read(id).AssignmentsToCalls ==null))
            {
                lock (AdminManager.BlMutex) //stage 7
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
        return CallManager.GetCallInListsHelp(filter, obj, sortBy); 
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
        IEnumerable<DO.Call>? allCalls;
        lock (AdminManager.BlMutex) //stage 7
            allCalls = _dal.Call.ReadAll();

        // Retrieve all assignments from the DAL
        IEnumerable<DO.Assignment>? allAssignments;
        lock (AdminManager.BlMutex) //stage 7
            allAssignments = _dal.Assignment.ReadAll();
        
        if(type == BO.CallType.None)
            type = null;

        if (type == BO.CallType.None)
            type = null;

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

    /// <summary>
    /// This function retrieves open or risk-open calls assigned to a specific volunteer, filters them based on optional call type and sorting criteria, 
    /// and returns the filtered and sorted list, including information such as call details, distance from the volunteer, and call status.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="sortBy"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>


    public IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.CallType? type, BO.EOpenCallInList? sortBy)
    {
        return CallManager.GetOpenCallHelp(id, type, sortBy);
    }

    /// <summary>
    /// The Read function retrieves a call by its ID and maps the data from the database (DO objects) into a business object (BO). 
    /// It also retrieves the assignments associated with that call and returns them along with the call details in a structured format.
    /// </summary>
    public BO.Call Read(int id)
    {
        return CallManager.ReadHelp(id);
    }

    /// <summary>
    /// Updating the reading and checking whether there was an exception from the DAL layer
    /// </summary>
    public void Update(BO.Call boCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        //double[] coordinate = VolunteerManager.GetCoordinatesAsync(boCall.FullAddress);
        //double latitude = coordinate[0];
        //double longitude = coordinate[1];
        //boCall.Latitude = latitude;
        //boCall.Longitude = longitude;
        CallManager.CheckLogic(boCall);
        DO.Call doCall = new
                    (
                    boCall.Id,
                    (DO.CallType)boCall.Type,
                    boCall.Description,
                    boCall.FullAddress,
                    null,
                    null,
                    boCall.TimeOpened,
                    boCall.MaxTimeToClose
                    );
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                _dal.Call.Update(doCall);
            lock (AdminManager.BlMutex) //stage 7
              doCall=  _dal.Call.ReadAll().OrderByDescending(s=>s.Id).First();
        }
        catch (DO.DalDeleteImpossible ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={boCall.Id} does Not exist", ex);
        }
        CallManager.Observers.NotifyItemUpdated(doCall.Id);  //stage 5
        CallManager.Observers.NotifyListUpdated();  //stage 5
        _ = CallManager.updateCoordinatesForCallsAddressAsync(doCall);
    }
}


