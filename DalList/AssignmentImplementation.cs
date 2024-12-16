using DalApi;
using DO;

namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    
   
    public void Create(Assignment item)
    {


       
        int newId =Config.NextAssignmenteID ;
      
       Assignment copy = item with { Id = newId };
        DataSource.Assignments.Add(copy);

     //   return ID;
    }

    public void Delete(int id)
    {
      
        var assignment = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            throw new DalDeletImposible($"Assignment with ID={id} not exists");

       DataSource.Assignments.Remove(assignment); 
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear(); // ניקוי כל הרשימה
    }

    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(item => item.Id == id); //stage 2
        //return DataSource.Assignments.FirstOrDefault(a => a.Id == id);

    }


    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
        => filter == null ? DataSource.Assignments.Select(item => item)
           : DataSource.Assignments.Where(filter);


    //public void Update(Assignment item)
    //{


    //    int index = DataSource.Assignments.FindIndex(c => c.Id == item.Id);
    //    if (index == )
    //    {
    //        throw new DalDeletImposible($"Assignment with ID={item.Id} not exists");
    //    }
    //    DataSource.Assignments[index] = item;
    //}
    public void Update(Assignment item)
    {
        Assignment? old = DataSource.Assignments.Find(x => x?.Id == item.Id);

        if (old == null)
        {

            //throw new Exception($"Volunteer with ID={id} does not exist"); // stag 1
            throw new DO.DalDeletImposible($"Assignment with ID={item.Id} does not exist"); // stag 2

        }
        else
        {
            DataSource.Assignments.Remove(old);
            DataSource.Assignments.Add(item);
        }
    }


}
