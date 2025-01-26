namespace BlImplementation;
using Helpers;
using BlApi;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

    public void Create(BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        //double[] coordinate = VolunteerManager.GetCoordinatesAsync(boVolunteer.FullAddress);
        //double latitude = coordinate[0];
        //double longitude = coordinate[1];
        //boVolunteer.Latitude = latitude;
        //boVolunteer.Longitude = longitude;
        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);
        string password=VolunteerManager.EncryptPassword(boVolunteer.Password);
        DO.Volunteer doVolunteer = new
            (
            boVolunteer.Id,
            boVolunteer.FullName,
            boVolunteer.PhoneNumber,
            boVolunteer.Email,
            (DO.Distance)boVolunteer.TypeDistance,
            (DO.Role)boVolunteer.Job,
            boVolunteer.Active,
            password,
            boVolunteer.FullAddress,
            null,
           null,
            boVolunteer.MaxReading
            );

        try
        {
            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalExistException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
        VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id);  //stage 5
        VolunteerManager.Observers.NotifyListUpdated(); //stage 5 
        _ = VolunteerManager.updateCoordinatesForVolunteerAddressAsync(doVolunteer);
    }

    public bool CanDelete(int id)
    {
        return (Read(id).CallIn == null) && (Read(id).SumCalls == 0);
    }

    public void Delete(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex) //stage 7
        {
            DO.Volunteer? doVolunteer = _dal.Volunteer.Read(id);
            assignments = _dal.Assignment.ReadAll(ass => ass.VolunteerId == id);
        }

        if (assignments != null&& assignments.Count(ass => ass.TimeEnd == null) > 0)
            throw new BO.BlWrongInputException("A volunteer handling a call cannot be deleted");
        if (assignments != null && assignments.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated) > 0)
            throw new BO.BlWrongInputException("A volunteer who has handled calls in the past cannot be deleted");
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDeleteImpossible doEx)
        {
            throw new BO.BlDeleteNotPossibleException("id not valid", doEx);
        }
        VolunteerManager.Observers.NotifyListUpdated();  //stage 5 
    }
  
    public BO.Role EnterSystem(int usingName, string password)
    {
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = _dal.Volunteer.Read(usingName) ?? throw new BO.BlNullPropertyException("the volunteer is null");
        
        if (volunteer.Password != password) throw new BO.BlWrongInputException("The password do not match");
        return (BO.Role)volunteer.Job;
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? active, BO.EVolunteerInList? sortBy)
    {
        return VolunteerManager.GetVolunteerListHelp(active, sortBy);
    }

    public BO.Volunteer Read(int id)
    {
        DO.Volunteer doVolunteer;
        lock (AdminManager.BlMutex)//stage 7
             doVolunteer = _dal.Volunteer.Read(id) ??throw new BO.BlWrongInputException($"Volunteer with ID={id} does Not exist");

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
            Password =VolunteerManager.DecryptPassword( doVolunteer.Password),
            FullAddress = doVolunteer.FullAddress,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            SumCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.TypeEndTreat == DO.TypeEnd.Treated),
                SumCanceled = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id &&
                    (a.TypeEndTreat == DO.TypeEnd.ManagerCancel || a.TypeEndTreat == DO.TypeEnd.SelfCancel)), // ביטול עצמי או מהנל
                SumExpired = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.TypeEndTreat == DO.TypeEnd.ExpiredCancel),

         CallIn = VolunteerManager.GetCallIn(doVolunteer),
        };
    }

    public void Update(int id, BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        DO.Volunteer doVolunteer;
        DO.Volunteer ismanager;
        lock (AdminManager.BlMutex) //stage 7
        {
           doVolunteer = _dal.Volunteer.Read(boVolunteer.Id) ?? throw new BO.BlWrongInputException($"Volunteer with ID={boVolunteer.Id} does Not exist");
           ismanager = _dal.Volunteer.Read(id) ?? throw new BO.BlWrongInputException($"Volunteer with ID={id} does Not exist");
        }
        if (ismanager.Job != DO.Role.Boss && boVolunteer.Id != id)
            throw new BO.BlWrongInputException("id and does not correct or not manager");
        
            //double[] coordinate = VolunteerManager.GetCoordinatesAsync(boVolunteer.FullAddress);
            //boVolunteer.Latitude = coordinate[0];
            //boVolunteer.Longitude = coordinate[1];
        
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
      

        DO.Volunteer volunteerUpdate = new(

      boVolunteer.Id,
      boVolunteer.FullName,
      boVolunteer.PhoneNumber,
      boVolunteer.Email,
      (DO.Distance)boVolunteer.TypeDistance,
      (DO.Role)boVolunteer.Job,
      boVolunteer.Active,
      VolunteerManager.EncryptPassword(boVolunteer.Password),
      boVolunteer.FullAddress,
      null,null,
      boVolunteer.MaxReading
      );
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Update(volunteerUpdate);
          
        }
        catch (DO.DalExistException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} not exists", ex);
        }
        VolunteerManager.Observers.NotifyItemUpdated(volunteerUpdate.Id);  //stage 5
        VolunteerManager.Observers.NotifyListUpdated();  //stage 5
        try
        { 
        _ = VolunteerManager.updateCoordinatesForVolunteerAddressAsync(volunteerUpdate); //stage 7
                                                                                       
        }
        catch(Exception ex)
        {  throw new Exception(ex.Message, ex); }
            }
}

