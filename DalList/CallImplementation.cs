namespace Dal;
using DO;
using DalApi;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{
    /// <summary>
    /// Create a new call
    /// </summary>
    /// <param name="item"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        int newId = Config.NextCallId;
        Call copy = item with { Id = newId };

        DataSource.Calls.Add(copy);
        // return newItem.Id;
    }

    /// <summary>
    /// Delete call with id  that the user chose
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="DalDeletImposible"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Call? call1 = DataSource.Calls.Find(c => c.Id == id);
        if (call1 == null)
            throw new DalDeleteImpossible($"Call with ID={id} not exists");
        DataSource.Calls.Remove(call1);
    }

    /// <summary>
    /// Delete all the calls
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    /// <summary>
    /// Read call with id  that the user chose
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(item => item.Id == id); //stage 2
        //return DataSource.Calls.Find(c => c.Id == id);
    }

    /// <summary>
    /// The function returns the first call according to the filter parameter
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);  //stage 2
    }

    /// <summary>
    /// Read all the calls
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
        => filter == null ? DataSource.Calls.Select(item => item)
           : DataSource.Calls.Where(filter);

    /// <summary>
    /// Update call with id  that the user chose
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DO.DalDeletImposible"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        Call? old = DataSource.Calls.Find(x => x?.Id == item.Id);

        if (old == null)
        {
            //throw new Exception($"Volunteer with ID={id} does not exist"); // stag 1
            throw new DO.DalDeleteImpossible($"Call with ID={item.Id} does not exist"); // stag 2
        }
        else
        {
            DataSource.Calls.Remove(old);
            DataSource.Calls.Add(item);
        }
    }
}
