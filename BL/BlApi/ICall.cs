﻿namespace BlApi;


/// <summary>
/// Definition of the call methods
/// </summary>
public interface ICall : IObservable
{
    int[] CountCall();
    IEnumerable<BO.CallInList> GetCallInLists(BO.ECallInList? filter, object? obj, BO.ECallInList? sortBy);
    BO.Call Read(int id);
    void Update(BO.Call call);
    void Delete(int id);
    void Create(BO.Call call);
    IEnumerable<BO.ClosedCallInList> GetClosedCall(int id, BO.CallType? type, BO.EClosedCallInList? sortBy);
    IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.CallType? type, BO.EOpenCallInList? sortBy);
    void CloseTreat(int idVol, int idAssig);
    void CancelTreat(int idVol, int idAssig);
    void ChoseForTreat(int idVol, int idCall);
    bool CanDelete(int id);
}

