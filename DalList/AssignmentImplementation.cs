using DalApi;
using DO;

namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Create a new assignment
    /// </summary>
    /// <param name="item"></param>
    public void Create(Assignment item)
    {
        int newId = Config.NextAssignmentID ;
        Assignment copy = item with { Id = newId };
        DataSource.Assignments.Add(copy);
        // return ID;
    }

    /// <summary>
    /// Delete assignment with id that the user chose
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="DalDeleteImpossible"></exception>
    public void Delete(int id)
    {
        var assignment = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            throw new DalExistException($"Assignment with ID={id} does not exists");

       DataSource.Assignments.Remove(assignment); 
    }

    /// <summary>
    /// Delete all the assignments
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Assignments.Clear(); 
    }

    /// <summary>
    /// Read assignment with id that the user chose
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(item => item.Id == id); //stage 2
    }

    /// <summary>
    /// The function returns the first assignment according to the filter parameter
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter); //stage 2
    }

    /// <summary>
    /// Read all the assignments
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
        => filter == null ? DataSource.Assignments.Select(item => item)
           : DataSource.Assignments.Where(filter);

    /// <summary>
    /// Update assignment
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DO.DalDeleteImpossible"></exception>
    public void Update(Assignment item)
    {
        Assignment? old = DataSource.Assignments.Find(x => x?.Id == item.Id);
        if (old == null)
        {
            //throw new Exception($"Volunteer with ID={id} does not exist"); // stag 1
            throw new DO.DalExistException($"Assignment with ID={item.Id} does not exist"); // stag 2
        }
        else
        {
            DataSource.Assignments.Remove(old);
            DataSource.Assignments.Add(item);
        }
    }
}
