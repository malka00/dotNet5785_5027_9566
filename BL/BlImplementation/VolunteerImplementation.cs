
namespace BlImplementation;
using Helpers;
using BlApi;
using System;
using System.Linq;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void Create(BO.Volunteer boVolunteer)
    {
        double[] cordinate = VolunteerManager.GetCoordinates(boVolunteer.FullAddress);
        double latitude = cordinate[0];
        double longitude = cordinate[1];
        boVolunteer.Latitude = latitude;
        boVolunteer.Longitude = longitude;
        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);
        DO.Volunteer doVolunteer = new
            (
            boVolunteer.Id,
            boVolunteer.FullName,
            boVolunteer.PhoneNumber,
            boVolunteer.Email,
            (DO.Distance)boVolunteer.TypeDistance,
            (DO.Role)boVolunteer.Job,
            boVolunteer.Active,
            boVolunteer.Password,
            boVolunteer.FullAddress,
             boVolunteer.Latitude,
            boVolunteer.Longitude,
            boVolunteer.MaxReading

            );

        try
        {
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalExistException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
    }

    public void Delete(int id)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(id);
        IEnumerable < DO.Assignment> assignments= _dal.Assignment.ReadAll(ass=>ass.VolunteerId==id);
        
        if (assignments != null&& assignments.Count(ass => ass.TimeEnd == null) > 0)
       
            throw new BO.BlWrongInputException("canot delete have assignemnt in treat");
        try
        {
            _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDeleteImpossible doEx)
        {
            throw new BO.BlDeleteNotPossibleException("id not valid", doEx);
        }

    }
  
    public BO.Role EnterSystem(int usingName, string password)
    {
        DO.Volunteer volunteer = _dal.Volunteer.Read(usingName) ?? throw new BO.BlNullPropertyException("the volunteer is null");
       if (volunteer.Password != password) throw new BO.BlWrongInputException("The password dont match");
        return (BO.Role)volunteer.Job;
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? active, BO.EVolunteerInList? sortBy)
    {
        // Retrieve all volunteers from the data layer
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll()??throw new BO.BlNullPropertyException ("There are not volunteers int database");

        // Convert IEnumerable<DO.Volunteer> to IEnumerable<BO.VolunteerInList>
        // Using the 'convertDOToBOInList' method to map each DO.Volunteer to BO.VolunteerInList
        IEnumerable<BO.VolunteerInList> boVolunteersInList = volunteers
            .Select(doVolunteer => VolunteerManager.convertDOToBOInList(doVolunteer));

        // If an 'active' filter is provided, filter the volunteers based on their active status
        // Otherwise, keep all volunteers without filtering
        var filteredVolunteers = active.HasValue
              ? boVolunteersInList.Where(v => v.Active == active)
              : boVolunteersInList;

        // If a 'sortBy' criteria is provided, sort the filtered volunteers by the selected property
        var sortedVolunteers = sortBy.HasValue
            ? filteredVolunteers.OrderBy(v =>
                sortBy switch
                {
                    // Sorting by different properties of the volunteer (ID, Full Name, etc.)
                    BO.EVolunteerInList.Id => (object)v.Id, // Sorting by ID (T.Z)
                    BO.EVolunteerInList.FullName => v.FullName, // Sorting by full name
                    BO.EVolunteerInList.Active => v.Active, // Sorting by active status
                    BO.EVolunteerInList.SumCalls => v.SumCalls, // Sorting by total number of calls
                    BO.EVolunteerInList.SumCanceled => v.Sumcanceled, // Sorting by total number of cancellations
                    BO.EVolunteerInList.SumExpired => v.SumExpired, // Sorting by total number of expired calls
                    BO.EVolunteerInList.IdCall => v.IdCall ?? null, // Sorting by call ID (nullable)
                    BO.EVolunteerInList.CType => v.Ctype.ToString(), // Sorting by call type (converted to string)
                })
            : filteredVolunteers.OrderBy(v => v.Id); // Default sorting by ID (T.Z) if no 'sortBy' is provided

        // Return the sorted and filtered list of volunteers
        return sortedVolunteers;
    }


    public BO.Volunteer Read(int id)
    {
        var doVolunteer = _dal.Volunteer.Read(id) ??
        throw new BO.BlWrongInputException($"Volunteer with ID={id} does Not exist");
        return new()
        {
            Id = id,
            Email = doVolunteer.Email,
            MaxReading = doVolunteer.MaxReading,
            FullName = doVolunteer.FullName,
            PhoneNumber = doVolunteer.PhoneNumber,
            TypeDistance = (BO.Distance)doVolunteer.TypeDistance,
            Job = (BO.Role)doVolunteer.Job,
            Active = doVolunteer.Active,
            Password = doVolunteer.Password,
            FullAddress = doVolunteer.FullAddress,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            CallIn = VolunteerManager.GetCallIn(doVolunteer),
        };
    }

    public void Update(int id, BO.Volunteer boVolunteer)
    {
        DO.Volunteer doVolunteer = _dal.Volunteer.Read(boVolunteer.Id) ?? throw new BO.BlWrongInputException($"Volunteer with ID={boVolunteer.Id} does Not exist");
        DO.Volunteer ismanager = _dal.Volunteer.Read(id) ?? throw new BO.BlWrongInputException($"Volunteer with ID={id} does Not exist");
        if (ismanager.Job != DO.Role.Boss || boVolunteer.Id != id)
            throw new BO.BlWrongInputException("id and  does not correct or not manager");
        
            double[] cordinat = VolunteerManager.GetCoordinates(boVolunteer.FullAddress);
            boVolunteer.Latitude = cordinat[0];
            boVolunteer.Longitude = cordinat[1];
        if (boVolunteer.Id != doVolunteer.Id)
        {
            
                throw new BO.BlWrongInputException("can not update ID");
        }
        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);
        if (ismanager.Job != DO.Role.Boss)
        {
            if (boVolunteer.Job != (BO.Role)doVolunteer.Job)
                throw new BO.BlWrongInputException("not have promition to change the role");
        }
      

        ///לבדוק על פעיחל אם מותר לשנות...
        DO.Volunteer volunteerUpdate = new(

      boVolunteer.Id,
      boVolunteer.FullName,
      boVolunteer.PhoneNumber,
      boVolunteer.Email,
      (DO.Distance)boVolunteer.TypeDistance,
      (DO.Role)boVolunteer.Job,
      boVolunteer.Active,
      boVolunteer.Password,
      boVolunteer.FullAddress,
      boVolunteer.Latitude,
      boVolunteer.Longitude,
      boVolunteer.MaxReading
      );
        try
        {
            _dal.Volunteer.Update(volunteerUpdate);
        }
        catch (DO.DalExistException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} not exists", ex);
        }

    }
}

