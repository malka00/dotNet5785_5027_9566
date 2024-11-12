namespace Dal;
using DO;
using DalApi;

public class CallImplementation : ICall
{
   // private List<Call> calls = new List<Call>();


    public void Create(Call item)
    {
        // יצירת מזהה חדש על בסיס המספר הרץ הבא
        int newId = Config.NextCallId;

        
        Call newItem = new Call 
        {
            Id = newId,
            
             Type=item.Type,
             Description=item.Description,
             FullAddress=item.FullAddress,
             Latitude=item.Latitude,
             Longitude=item.Longitude,
             TimeOpened = item.TimeOpened,
             MaxTimeToClose = item.MaxTimeToClose    
         
        };
       if (DataSource.Calls.Exists(c => c.Id == item.Id))
        {
            throw new ArgumentException("Call with the same ID already exists.");
        }
        DataSource.Calls.Add(item);
     //   return newItem.Id;
    }

    public void Delete(int id)
    {
        Call call = DataSource.Calls.Find(c => c.Id == id)
            ?? throw new KeyNotFoundException("Call not found.");
        DataSource.Calls.Remove(call);
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
       var findCall =DataSource.Calls.Find(c => c.Id == id);
        if (findCall != null) 
         return findCall; 

        return null;
           
           
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        int index = DataSource.Calls.FindIndex(c => c.Id == item.Id);
        if (index == -1)
        {
            throw new KeyNotFoundException("Call not found.");
        }
        DataSource.Calls[index] = item;

        // הסרה של האובייקט הישן
       // calls.RemoveAt(index);

        // הוספה של האובייקט המעודכן
      //  calls.Add(item);


    }
}
