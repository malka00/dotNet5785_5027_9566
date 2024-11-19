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

    //public List<Call> ReadAll()
    //{
    //    return new List<Call>(DataSource.Calls);
    //}
   
    //public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) //stage 2
    //{ => filter == null
    //        ? from item in DataSource.Calls
    //          where filter(item)
    //          select item;
    //        : from item in DataSource.Calls
    //          select item;
    //}



public void Update(Call item)
    {
        int index = DataSource.Calls.FindIndex(c => c.Id == item.Id);
        if (index == -1)
            throw new DalDeletImposible($"Call with ID={item.Id} does not exist.");

        
        DataSource.Calls[index] = item;
    }
}
