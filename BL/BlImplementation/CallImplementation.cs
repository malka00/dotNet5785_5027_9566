
using BlApi;
using System;
using Helpers;


using System.Collections.Generic;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    //  public ICall Call { get; } = new CallImplementation();

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
        if (assigmnetToCancel.TypeEndTreat != null || (_dal.Call.Read(assigmnetToCancel.CallId).MaxTimeToClose > ClockManager.Now)| assigmnetToCancel.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assigmnet not open or exspaired");

        DO.Assignment assigmnetToUP = new DO.Assignment {
            Id = assigmnetToCancel.Id,
            CallId = assigmnetToCancel.CallId,
            VolunteerId =assigmnetToCancel.VolunteerId,
             TimeStart=assigmnetToCancel.TimeStart,
             TimeEnd = ClockManager.Now,
            TypeEndTreat = ismanager?DO.TypeEnd.ManagerCancel:DO.TypeEnd.SelfCancel,
            };
        try
        {
            _dal.Assignment.Update(assigmnetToUP);
        }
        catch ( DO.DalExsitException ex)
        {
            throw new BO.BlDeleteNotPossibleException("canot delete in DO");
        }

    }

    public void ChoseForTreat(int idVol, int idCall)
    {
        DO.Volunteer vol = _dal.Volunteer.Read(idVol) ?? throw new BO.BlNullPropertyException($"there is no volunterr with this ID {idVol}");
        BO.Call bocall = Read(idCall) ?? throw new BO.BlNullPropertyException($"there is no call with this ID {idCall}");
        if (bocall.Status == BO.StatusTreat.Open || bocall.Status == BO.StatusTreat.Expired)
            throw new BO.BlAlreadyExistsException($"the call is open or expired Idcall is={idCall}");
        DO.Assignment assigmnetToCreat = new DO.Assignment
        {
            Id = 0,
            CallId = idCall,
            VolunteerId = idVol,
            TimeStart = ClockManager.Now,
            TimeEnd = null,
            TypeEndTreat = null
        };
        try
        {
            _dal.Assignment.Create(assigmnetToCreat);
        }
        catch (DO.DalDeletImposible)
        { throw new BO.BlAlreadyExistsException("impossible to creat"); }
    }

    public void CloseTreat(int idVol, int idAssig)
    {
        DO.Assignment assigmnetToClose = _dal.Assignment.Read(idAssig) ?? throw new BO.BlDeleteNotPossibleException("there is no assigment with this ID");
        if (assigmnetToClose.VolunteerId != idVol)
        {
           throw new BO.BlWrongInputException("the volunteer is not treat in this assignment");
        }
        BO.Call bocall = Read(assigmnetToClose.CallId);
        if (assigmnetToClose.TypeEndTreat != null || (bocall.Status!=BO.StatusTreat.Open&& bocall.Status != BO.StatusTreat.RiskOpen) ||assigmnetToClose.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assigmnet not open");

        DO.Assignment assigmnetToUP = new DO.Assignment
        {
            Id = assigmnetToClose.Id,
            CallId = assigmnetToClose.CallId,
            VolunteerId = assigmnetToClose.VolunteerId,
            TimeStart = assigmnetToClose.TimeStart,
            TimeEnd = ClockManager.Now,
            TypeEndTreat = DO.TypeEnd.Treated,
        };
        try
        {
            _dal.Assignment.Update(assigmnetToUP);
        }
        catch (DO.DalExsitException ex)
        {
            throw new BO.BlDeleteNotPossibleException("canot update in DO");
        }
    }

    /// <summary>
    /// מחזירה כמות קריאות בסטטוסים שווים
    /// </summary>
    /// <returns></returns>
    public int[] CountCall()
    {
        IEnumerable<DO.Call>? calls = _dal.Call.ReadAll();
        int[] count = (from item in calls
                       group item by CallManager.GetCallStatus(item) into groupedCalls
                       orderby groupedCalls.Key
                       select groupedCalls.Count()).ToArray();
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
        catch (DO.DalExsitException ex)
        {
            throw new BO.BlAlreadyExistsException($"Student with ID={boCall.Id} already exists", ex);
        }
    }

    /// <summary>
    /// Delete a call
    /// </summary>
    public void Delete(int id)
    {
        try
        {
            if (Read(id).Status == BO.StatusTreat.Open)
            {
                _dal.Call.Delete(id);
                return;
            }
            throw new BO.BlDeleteNotPossibleException($"Call with id={id} can not be deleted");
        }
        catch (DO.DalExsitException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with id={id} does not exist");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="obj"></param>
    /// <param name="sortBy"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlNullPropertyException"></exception>
    public IEnumerable<BO.CallInList> GetCallInLists(BO.ECallInList? filter, object? obj, BO.ECallInList? sortBy)
    {
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are not calls int database");
        IEnumerable<BO.CallInList> boCallsInList = _dal.Call.ReadAll().Select(call => CallManager.ConvertDOCallToBOCallInList(call));
        if (filter != null && obj != null)
        {
            switch (filter)
            {
                case BO.ECallInList.Id:
                    boCallsInList.Where(item => item.Id == (int)obj).Select(item => item);
                    break;

                case BO.ECallInList.CallId:
                    boCallsInList.Where(item => item.CallId == (int)obj).Select(item => item);
                    break;

                case BO.ECallInList.CType:
                    boCallsInList.Where(item => item.Type == (BO.CallType)obj).Select(item => item);
                    break;

                case BO.ECallInList.TimeOpened:
                    boCallsInList.Where(item => item.TimeOpened == (DateTime)obj).Select(item => item);
                    break;

                case BO.ECallInList.TimeLeft:
                    boCallsInList.Where(item => item.TimeLeft == (TimeSpan)obj).Select(item => item);
                    break;

                case BO.ECallInList.LastVolunteer:
                    boCallsInList.Where(item => item.LastVolunteer == (string)obj).Select(item => item);
                    break;

                case BO.ECallInList.TotalTime:
                    boCallsInList.Where(item => item.TotalTime == (TimeSpan)obj).Select(item => item);
                    break;

                case BO.ECallInList.Status:
                    boCallsInList.Where(item => item.Status == (BO.StatusTreat)obj).Select(item => item);
                    break;

                case BO.ECallInList.SumAssignment:
                    boCallsInList.Where(item => item.SumAssignment == (int)obj).Select(item => item);
                    break;
            }
        }
        if (sortBy == null)
            sortBy = BO.ECallInList.CallId;
        switch (sortBy)
        {
            case BO.ECallInList.Id:
                boCallsInList= boCallsInList.OrderBy(item => item.Id.HasValue ? 0 : 1)
    .ThenBy(item => item.Id)
    .ToList();
                break;

            case BO.ECallInList.CallId:
                boCallsInList= boCallsInList.OrderBy(item => item.CallId).ToList();
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
        return boCallsInList;
    }




    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="sortBy"></param>
    /// <returns></returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCall(int id, BO.CallType? type, BO.EClosedCallInList? sortBy)
    {
        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        List<BO.ClosedCallInList>? Calls = new List<BO.ClosedCallInList>();
        
        Calls.AddRange(from item in previousCalls
                       let DataCall = Read(item.Id)
                       where DataCall.Status == BO.StatusTreat.Close && DataCall.AssignmentsToCalls?.Any() == true
                       let lastAssugnment = DataCall.AssignmentsToCalls.OrderBy(c => c.StartTreat).Last()
                       select CallManager.ConvertDOCallToBOCloseCallInList(item, lastAssugnment));
        IEnumerable<BO.ClosedCallInList>? closedCallInLists = Calls.Where(call=>call.Id == id);
        if(type!=null)
        {
            closedCallInLists.Where(c => c.Type == type).Select(c => c);
        }
        if (sortBy == null)
        {
            sortBy = BO.EClosedCallInList.Id;
        }
        switch (sortBy)
        {
            case BO.EClosedCallInList.Id:
                    closedCallInLists.OrderBy(item => item.Id);
                break;
            case BO.EClosedCallInList.CType:
                    closedCallInLists.OrderBy(item => item.Type);
                    break;
            case BO.EClosedCallInList.FullAddress:
                    closedCallInLists.OrderBy(item => item.FullAddress);
                    break;
            case BO.EClosedCallInList.TimeOpen:
                    closedCallInLists.OrderBy(item => item.TimeOpen);
                    break;
            case BO.EClosedCallInList.StartTreat:
                    closedCallInLists.OrderBy(item => item.StartTreat);
                    break;
            case BO.EClosedCallInList.TimeClose:
                    closedCallInLists.OrderBy(item => item.TimeClose);
                    break;
            case BO.EClosedCallInList.TypeEndTreat:
                    closedCallInLists.OrderBy(item => item.TypeEndTreat);
                    break;
           
        }
        return closedCallInLists;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="sortBy"></param>
    /// <returns></returns>
    public IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.CallType? type, BO.EOpenCallInList? sortBy)
    {
        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        List<BO.OpenCallInList>? Calls = new List<BO.OpenCallInList>();

        Calls.AddRange(from item in previousCalls
                       let DataCall = Read(item.Id)
                       where DataCall.Status == BO.StatusTreat.Open || DataCall.Status == BO.StatusTreat.RiskOpen
                       let lastAssugnment = DataCall.AssignmentsToCalls.OrderBy(c => c.StartTreat).Last()
                       select CallManager.ConvertDOCallToBOOpenCallInList(item, id));
        IEnumerable<BO.OpenCallInList>? openCallInLists = Calls.Where(call => call.Id == id);
        if (type != null)
        {
            openCallInLists.Where(c => c.CType == type).Select(c => c);
        }
        if (sortBy == null)
        {
            sortBy =BO.EOpenCallInList.Id;
        }
        switch (sortBy)
        {
            case BO.EOpenCallInList.Id:
                openCallInLists.OrderBy(item => item.Id);
                break;
            case BO.EOpenCallInList.CType:
                openCallInLists.OrderBy(item => item.CType);
                break;
            case BO.EOpenCallInList.FullAddress:
                openCallInLists.OrderBy(item => item.FullAddress);
                break;
            case BO.EOpenCallInList.TimeOpen:
                openCallInLists.OrderBy(item => item.TimeOpen);
                break;
            case BO.EOpenCallInList.MaxTimeToClose:
                openCallInLists.OrderBy(item => item.MaxTimeToClose);
                break;
            case  BO.EOpenCallInList.distanceCallVolunteer :
                openCallInLists.OrderBy(item => item.distanceCallVolunteer);
                break;

        }
        return openCallInLists;
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.CallType? type, BO.EClosedCallInList? sortBy)
    {
        throw new NotImplementedException();
    }


    //public IEnumerable<BO.CallInList> ReadAll(BO.CallAssignInList? filter = null, BO.CallAssignInList? sort = null, object? ValueTask = null)
    //{

    //}



    /// <summary>
    /// קריאת הקריאה, צריך לבדוק איזה חריגה מתאימה
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
            // Status = CallManager.GetCallStatus(dataOfAssignments, doCall.MaxTimeToClose),
            Status = CallManager.GetCallStatus(doCall),
            AssignmentsToCalls = dataOfAssignments.Select(assign => new BO.CallAssignInList
            {
                VolunteerId = assign.VolunteerId,
                VolunteerName = _dal.Volunteer.Read(id).FullName,
                StartTreat = assign.TimeStart,
                TimeClose = assign.TimeEnd,
                TypeEndTreat = (BO.TypeEnd)assign.TypeEndTreat,
            }).ToList()
        };
        throw new Exception();
    }

    /// <summary>
    /// עדכון הקריאה ובדיקה האם היתה חריגה משכבת ה DO
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
        catch (DO.DalDeletImposible ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={boCall.Id} does Not exist", ex);
        }
    }
}


