
using BlApi;
using System;
using Helpers;
using DalApi;
using System.Data.Common;
using BO;
using System.Collections.Generic;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    //  public ICall Call { get; } = new CallImplementation();

    public void CancelTreat(int idVol, int idAssig)
    {
        throw new NotImplementedException();
    }

    public void ChoseForTreat(int idVol, int idAssig)
    {
        throw new NotImplementedException();
    }

    public void CloseTreat(int idVol, int idAssig)
    {
        throw new NotImplementedException();
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
        try
        {
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
                case ECallInList.Id:
                    boCallsInList.Where(item => item.Id == (int)obj).Select(item => item);
                    break;

                case ECallInList.CallId:
                    boCallsInList.Where(item => item.CallId == (int)obj).Select(item => item);
                    break;

                case ECallInList.CType:
                    boCallsInList.Where(item => item.Type == (CallType)obj).Select(item => item);
                    break;

                case ECallInList.TimeOpened:
                    boCallsInList.Where(item => item.TimeOpened == (DateTime)obj).Select(item => item);
                    break;

                case ECallInList.TimeLeft:
                    boCallsInList.Where(item => item.TimeLeft == (TimeSpan)obj).Select(item => item);
                    break;

                case ECallInList.LastVolunteer:
                    boCallsInList.Where(item => item.LastVolunteer == (string)obj).Select(item => item);
                    break;

                case ECallInList.TotalTime:
                    boCallsInList.Where(item => item.TotalTime == (TimeSpan)obj).Select(item => item);
                    break;

                case ECallInList.Status:
                    boCallsInList.Where(item => item.Status == (StatusTreat)obj).Select(item => item);
                    break;

                case ECallInList.SumAssignment:
                    boCallsInList.Where(item => item.SumAssignment == (int)obj).Select(item => item);
                    break;


            }
        }
        if (sortBy == null)
            sortBy = ECallInList.CallId;
        switch (sortBy)
        {
            case ECallInList.Id:
                boCallsInList.OrderBy(item => item.Id);
                break;

            case ECallInList.CallId:
                boCallsInList.OrderBy(item => item.CallId);
                break;

            case ECallInList.CType:
                boCallsInList.OrderBy(item => item.Type);
                break;

            case ECallInList.TimeOpened:
                boCallsInList.OrderBy(item => item.TimeOpened);
                break;

            case ECallInList.TimeLeft:
                boCallsInList.OrderBy(item => item.TimeLeft);
                break;

            case ECallInList.LastVolunteer:
                boCallsInList.OrderBy(item => item.LastVolunteer);
                break;

            case ECallInList.TotalTime:
                boCallsInList.OrderBy(item => item.TotalTime);
                break;

            case ECallInList.Status:
                boCallsInList.OrderBy(item => item.Status);
                break;

            case ECallInList.SumAssignment:
                boCallsInList.OrderBy(item => item.SumAssignment);
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
            sortBy = EClosedCallInList.Id;
        }
        switch (sortBy)
        {
            case EClosedCallInList.Id:
                    closedCallInLists.OrderBy(item => item.Id);
                break;
            case EClosedCallInList.CType:
                    closedCallInLists.OrderBy(item => item.Type);
                    break;
            case EClosedCallInList.FullAddress:
                    closedCallInLists.OrderBy(item => item.FullAddress);
                    break;
            case EClosedCallInList.TimeOpen:
                    closedCallInLists.OrderBy(item => item.TimeOpen);
                    break;
            case EClosedCallInList.StartTreat:
                    closedCallInLists.OrderBy(item => item.StartTreat);
                    break;
            case EClosedCallInList.TimeClose:
                    closedCallInLists.OrderBy(item => item.TimeClose);
                    break;
            case EClosedCallInList.TypeEndTreat:
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
            sortBy = EOpenCallInList.Id;
        }
        switch (sortBy)
        {
            case EOpenCallInList.Id:
                openCallInLists.OrderBy(item => item.Id);
                break;
            case EOpenCallInList.CType:
                openCallInLists.OrderBy(item => item.CType);
                break;
            case EOpenCallInList.FullAddress:
                openCallInLists.OrderBy(item => item.FullAddress);
                break;
            case EOpenCallInList.TimeOpen:
                openCallInLists.OrderBy(item => item.TimeOpen);
                break;
            case EOpenCallInList.MaxTimeToClose:
                openCallInLists.OrderBy(item => item.MaxTimeToClose);
                break;
            case  EOpenCallInList.distanceCallVolunteer :
                openCallInLists.OrderBy(item => item.distanceCallVolunteer);
                break;

        }
        return openCallInLists;
    }


    public IEnumerable<BO.CallInList> ReadAll(BO.CallAssignInList? filter = null, BO.CallAssignInList? sort = null, object? ValueTask = null)
    {

    }



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
            Status = CallManager.GetCallStatus(dataOfAssignments, doCall.MaxTimeToClose),
            AssignmentsToCalls = dataOfAssignments.Select(assign => new BO.CallAssignInList
            {
                VolunteerId = assign.VolunteerId,
                VolunteerName = _dal.Volunteer.Read(id).FullName,
                StartTreat = assign.TimeStart,
                TimeClose = assign.TimeEnd,
                TypeEndTreat = (BO.TypeEndTreat)assign.TypeEndTreat,
            }).ToList()
        };
        throw new Exception();
    }

    /// <summary>
    /// עדכון הקריאה ובדיקה האם היתה חריגה משכבת ה DO
    /// </summary>
    public void Update(BO.Call boCall)
    {
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


