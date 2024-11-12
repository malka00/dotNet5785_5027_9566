using DalApi;
using DO;

namespace Dal;

public class AssignmentImplementation : IAssignment
{
    
   
    public void Create(Assignment item)
    {
        // בדיקה שהאובייקט אינו null
        if (item == null)
            throw new ArgumentNullException(nameof(item), "item is null");

        // יצירת עותק עמוק של הפריט
       Assignment newAssignment = new Assignment
        {
            Id= Config.NextAssignmenteID,          
            CallId=item.CallId,       
            VolunteerId=item.VolunteerId,   
            TimeStart=item.TimeStart,
            TimeEnd =item.TimeEnd , 
            TypeEndTreat =item.TypeEndTreat,   
        };

        DataSource.Assignments.Add(newAssignment); // הוספה לרשימה
     //   return ID;
    }

    public void Delete(int id)
    {
      
        var assignment = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            throw new KeyNotFoundException($"Assignment עם המזהה {id} לא נמצא");

       DataSource.Assignments.Remove(assignment); 
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear(); // ניקוי כל הרשימה
    }

    public Assignment? Read(int id)
    {
        // חיפוש פריט לפי מזהה
        var assignment = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
           return null;
        return assignment;

    }

    public List<Assignment> ReadAll()
    {
       
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item)," assigment item is null");

        int index = DataSource.Assignments.FindIndex(c => c.Id == item.Id);
        if (index == -1)
        {
            throw new KeyNotFoundException("assigmant not found.");
        }
        DataSource.Assignments[index] = item;
    }
}
