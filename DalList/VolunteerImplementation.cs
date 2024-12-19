namespace Dal;
using DO;
using DalApi;
using System;
using System.Linq;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Create a new volunteer
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DalDeletImposible"></exception>
    public void Create(Volunteer item)
    {
        // Check if an object with the same ID exists
        if (DataSource.Volunteers.Any(v => v.Id == item.Id))
        {
            throw new DalExistException($"Volunteer with ID={item.Id} already exists"); ;
        }
        DataSource.Volunteers.Add(item);
    }

    /// <summary>
    /// Read volunteer with id that the user chose
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Volunteer? Read(int id)
    {
        //return DataSource.Volunteers.FirstOrDefault(v => v.Id == id); //stage 1
        return DataSource.Volunteers.FirstOrDefault(item => item.Id == id); //stage 2
    }

    /// <summary>
    /// Read all the volunteers
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
        => filter == null ? DataSource.Volunteers.Select(item => item)
           :DataSource.Volunteers.Where(filter);
    
    /// <summary>
    /// Update volunteer 
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DO.DalDeletImposible"></exception>
    public void Update(Volunteer item)
    {
        Volunteer? old = DataSource.Volunteers.Find(x => x?.Id == item.Id);

        if (old == null)
        {
            //throw new Exception($"Volunteer with ID={item.id} does not exist"); // // stag 1
            throw new DO.DalExistException($"Volunteer with ID={item.Id} does not exist"); // stage 2
        }
        else
        {
            DataSource.Volunteers.Remove(old);
            DataSource.Volunteers.Add(item);
        }
    }

    /// <summary>
    ///Delete volunteer with id that the user chose
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="DalExistException"></exception>
    public void Delete(int id)
    {
        var volunteer = DataSource.Volunteers.FirstOrDefault(v => v.Id == id) 
            ?? throw new DalExistException($"Volunteer with ID={id} does not exists");

        DataSource.Volunteers.Remove(volunteer);
    }

    /// <summary>
    /// Delete all the volunteers
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    /// <summary>
    /// The function returns the first volunteer according to the filter parameter
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter); //stage 2
    }
}
