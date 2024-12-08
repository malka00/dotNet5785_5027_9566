

using BlApi;
using System;
using Helpers;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    //  public ICall Call { get; } = new CallImplementation();

    public void CancalTreat(int idVol, int idAssig)
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

    public int CountCall()
    {
        throw new NotImplementedException();
    }

    public void Create(BO.Call boCall)
    {
        DO.Call doCall =
    new(boCall.Id, (DO.CallType)boCall.Type, boCall.Description, boCall.FullAddress, boCall.Latitude, boCall.Longitude,
      boCall.TimeOpened, boCall.MaxTimeToClose);

        try
        {
            _dal.Call.Create(doCall) ;
        }
        catch (DO.DalExsitException ex)
        {
          //  throw new BO.BlAlreadyExistsException($"Student with ID={boStudent.Id} already exists", ex);
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

    public BO.Call Read(int id)
    {
        var doCall = _dal.Call.Read(id);// ??
//throw new BO.BlDoesNotExistException($"Student with ID={id} does Not exist");
        return new()
        {
            Id = id,
            Type =(BO.CallType) doCall.Type,
            Description= doCall.Description,
            FullAddress= doCall.FullAddress,
            Latitude= doCall.Latitude,
            Longitude= doCall.Longitude,
            TimeOpened=doCall.TimeOpened,
            MaxTimeToClose=doCall.MaxTimeToClose,
        };
    }

    public void Update(BO.Call call)
    {
        throw new NotImplementedException();
    }
}
