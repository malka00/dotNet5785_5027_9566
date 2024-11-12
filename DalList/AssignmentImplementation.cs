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
            throw new KeyNotFoundException($"Assignment עם המזהה {id} לא נמצא");
        return assignment;

    }

    public List<Assignment> ReadAll()
    {
        // החזרת עותקים של כל האובייקטים כדי להגן על הנתונים המקוריים
        //return assignments.Select(a => new Assignment
        //{
        //    Id = a.Id,
        //    Name = a.Name,
        //    Description = a.Description,
        //    DueDate = a.DueDate
        //}).ToList();
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item)," assigment item is null");

        // חיפוש פריט לפי מזהה
        var assignment = DataSource.Assignments.FirstOrDefault(a => a.Id == item.Id);
        if (assignment == null)
            throw new KeyNotFoundException($"Assignment not found");

        // עדכון הפריט הקיים
        assignment.Name = item.Name;
        assignment.Description = item.Description;
        assignment.DueDate = item.DueDate;
    }
}
