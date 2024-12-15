
using BlApi;
using System;
using Helpers;
using BO;
using DO;
using DalApi;

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

    public int[] CountCall()
    {

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


    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.CallInList> GetCallInLists(BO.ECallInList? sortBy, object? obj, BO.ECallInList filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCall(int id, BO.CallType? type, BO.EClosedCallInList? sortBy)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.CallType? type, BO.EClosedCallInList? sortBy)
    {
        throw new NotImplementedException();
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


