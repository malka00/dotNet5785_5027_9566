namespace Dal;
using DO;
using DalApi;

internal class CallImplementation : ICall
{



    public void Create(Call item)
    {

        int newId = Config.NextCallId;
        Call copy = item with { Id = newId };

        DataSource.Calls.Add(copy);
        //   return newItem.Id;
    }

    public void Delete(int id)
    {
        Call? call1 = DataSource.Calls.Find(c => c.Id == id);
        if (call1 == null)
            throw new DalDeletImposible($"Call with ID={id} not exists");
        DataSource.Calls.Remove(call1);
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(item => item.Id == id); //stage 2
        //return DataSource.Calls.Find(c => c.Id == id);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
        => filter == null ? DataSource.Calls.Select(item => item)
           : DataSource.Calls.Where(filter);


    //public void Update(Call item)
    //{
    //    int index = DataSource.Calls.FindIndex(c => c.Id == item.Id);
    //    if (index == -1)
    //        throw new DalDeletImposible($"Call with ID={item.Id} does not exist.");


    //    DataSource.Calls[index] = item;
    //}


    public void Update(Call item)
    {
        Call? old = DataSource.Calls.Find(x => x?.Id == item.Id);

        if (old == null)
        {
            //throw new Exception($"Volunteer with ID={id} does not exist"); // stag 1
            throw new DO.DalDeletImposible($"Call with ID={item.Id} does not exist"); // stag 2
        }
        else
        {
            DataSource.Calls.Remove(old);
            DataSource.Calls.Add(item);
        }
    }
}
