

using BO;

namespace BlApi;

public interface ICall
{
    int CountCall();
    IEnumerable<BO.CallInList> GetCallInLists(ECallInList? sortBy, object? obj, ECallInList filter);
    BO.Call Read(int id);
    void Update(BO.Call call);
    void Delete(int id);
    void Creat(BO.Call call);
    IEnumerable<ClosedCallInList> GetClosedCall(int id, CallType? type, EClosedCallInList? sortBy);
    IEnumerable<OpenCallInList> GetOpenCall(int id, CallType? type, EClosedCallInList? sortBy);
    void CloseTreat(int idVol, int idAssig);
    void CancalTreat(int idVol, int idAssig);
    void ChoseForTreat(int idVol, int idAssig);
}

