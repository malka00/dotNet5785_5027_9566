namespace Dal;
using DO;
using DalApi;
using System;


public class VolunteerlImplementation : IVolunteer
{

    public void Create(Volunteer item)
    {
        

        // בדיקה אם קיים אובייקט עם אותו מזהה
        if (DataSource.Volunteers.Any(v => v.Id == item.Id))
        {
            throw new ArgumentException("item whith this ID already exsist");
        }

        // הוספת האובייקט לרשימה ישירות
        DataSource.Volunteers.Add(item);

        // החזרת ה-ID של האובייקט החדש
       // return item.Id;
    }


    public Volunteer? Read(int id)
    {
        var volunteer1 = DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        if (volunteer1 == null) 
        return null;
        return volunteer1;
    }

    public List<Volunteer> ReadAll()
    {
       


        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        

        var index = DataSource.Volunteers.FindIndex(v => v.Id == item.Id);
        if (index == -1) throw new ArgumentException("Volunteer with this ID not found");

        DataSource.Volunteers[index] = new Volunteer
        {
           Id=item.Id,  
      FullName=item.FullName,
     PhoneNumber=item.PhoneNumber,
     Email=item.Email,
     TypeDistance=item.TypeDistance,
     Job=item.Job,
     Active=item.Active,
     Password = item.Password,
     FullAddress = item.FullAddress,
     Latitude =item.Latitude, 
     Longitude = item.Longitude,
     MaxReading = item.MaxReading,
          
        };
    }

    public void Delete(int id)
    {
        var volunteer = DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        if (volunteer == null) throw new ArgumentException("Volunteer not found", nameof(id));

        DataSource.Volunteers.Remove(volunteer);
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }
}
