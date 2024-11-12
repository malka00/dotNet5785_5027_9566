namespace Dal;
using DO;
using DalApi;

public class CallImplementation : ICall
{
    private List<Call> calls = new List<Call>();

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
        if (calls.Exists(c => c.Id == item.Id))
        {
            throw new ArgumentException("Call with the same ID already exists.");
        }
        calls.Add(item);
     //   return newItem.Id;
    }

    public void Delete(int id)
    {
        Call call = calls.Find(c => c.Id == id)
            ?? throw new KeyNotFoundException("Call not found.");
        calls.Remove(call);
    }

    public void DeleteAll()
    {
        calls.Clear();
    }

    public Call? Read(int id)
    {
        return calls.Find(c => c.Id == id)
            ?? throw new KeyNotFoundException("Call not found.");
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(calls);
    }

    public void Update(Call item)
    {
        int index = calls.FindIndex(c => c.Id == item.Id);
        if (index == -1)
        {
            throw new KeyNotFoundException("Call not found.");
        }
         calls[index] = item;

        // הסרה של האובייקט הישן
       // calls.RemoveAt(index);

        // הוספה של האובייקט המעודכן
      //  calls.Add(item);


    }
}
