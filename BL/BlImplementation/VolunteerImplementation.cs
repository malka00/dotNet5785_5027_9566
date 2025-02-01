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

    /// <summary>
    /// Function to add a volunteer
    /// </summary>
    /// <param name="boVolunteer"></param>
    /// <exception cref="BO.BlAlreadyExistsException"></exception>
    public void Create(BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        //Checking that the logic and format of all the details are correct
        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);

        string password = VolunteerManager.EncryptPassword(boVolunteer.Password);

        // Produces a new volunteer
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

        // Add it to the DAL layer
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
        _ = VolunteerManager.updateCoordinatesForVolunteerAddressAsync(doVolunteer, boVolunteer.FullAddress);
    }

    /// <summary>
    /// check whether the volunteer can be deleted
    /// </summary>
    /// <param name="id"></param>
    /// <returns>bool</returns>
    public bool CanDelete(int id)
    {
        var volunteer = VolunteerManager.readHelp(id);
        return (volunteer.CallIn == null) && (volunteer.SumCalls == 0);
    }

    /// <summary>
    /// A method that deletes a volunteer
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="BO.BlWrongInputException"></exception>
    /// <exception cref="BO.BlDeleteNotPossibleException"></exception>
    public void Delete(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        // Finds the volunteer and his total allocations
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex) //stage 7
        {
            DO.Volunteer? doVolunteer = _dal.Volunteer.Read(id);
            assignments = _dal.Assignment.ReadAll(ass => ass.VolunteerId == id);
        }

        // Checking that he is not currently handling the reading or in the past
        if (assignments != null && assignments.Count(ass => ass.TimeEnd == null) > 0)
            throw new BO.BlWrongInputException("A volunteer handling a call cannot be deleted");
        if (assignments != null && assignments.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated) > 0)
            throw new BO.BlWrongInputException("A volunteer who has handled calls in the past cannot be deleted");

        // Deletes the volunteer from the Dal layer
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

    /// <summary>
    /// System login method - returns the role of the user
    /// </summary>
    /// <param name="usingName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlNullPropertyException"></exception>
    /// <exception cref="BO.BlWrongInputException"></exception>
    public BO.Role EnterSystem(int usingName, string password)
    {
        //Reading the volunteer's details from the system
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = _dal.Volunteer.Read(usingName);
        if (volunteer == null)
            throw new BO.BlNullPropertyException("the volunteer is null");
        
        // Checking that the password matches the id
        if (volunteer.Password != password) throw new BO.BlWrongInputException("The password do not match");
        return (BO.Role)volunteer.Job;
    }

    /// <summary>
    /// Returns a list of volunteers
    /// </summary>
    /// <param name="active"></param>
    /// <param name="sortBy"></param>
    /// <returns> IEnumerable<BO.VolunteerInList> </returns>
    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? active, BO.EVolunteerInList? sortBy)
    {
        return VolunteerManager.GetVolunteerListHelp(active, sortBy);
    }

    /// <summary>
    /// A function that reads a single volunteer from the data by their ID number
    /// </summary>
    /// <param name="id"></param>
    /// <returns> BO.Volunteer </returns>
    public BO.Volunteer ReadString(string id)
    {
        if(id==null)
            throw new BO.BlWrongInputException("You didn't enter ID");
        if (id.Length != 9)
            throw new BO.BlWrongInputException("ID must contain exactly 9 characters.");

        if (!id.All(char.IsDigit))
            throw new BO.BlWrongInputException("ID must contain only digits.");

        if (!int.TryParse(id, out int numericId))
            throw new BO.BlWrongInputException("ID is too large to be stored as an integer.");
        return VolunteerManager.readHelp(numericId);
    }

    public BO.Volunteer Read(int id)
    {
        return VolunteerManager.readHelp(id);
    }

    /// <summary>
    /// Volunteer update function
    /// </summary>
    /// <param name="id"></param>
    /// <param name="boVolunteer"></param>
    /// <exception cref="BO.BlWrongInputException"></exception>
    /// <exception cref="BO.BlAlreadyExistsException"></exception>
    public void Update(int id, BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        // Reads the volunteer who asks to update the volunteer who updates
        DO.Volunteer doVolunteer;
        DO.Volunteer ismanager;
        lock (AdminManager.BlMutex) //stage 7
        {
            doVolunteer = _dal.Volunteer.Read(boVolunteer.Id);
            ismanager = _dal.Volunteer.Read(id);
        }
        if (doVolunteer == null)
            throw new BO.BlWrongInputException($"Volunteer with ID={boVolunteer.Id} does Not exist");
        if (ismanager == null)
            throw new BO.BlWrongInputException($"Volunteer with ID={id} does Not exist");

        // The updater checks if he is an administrator or the one who is updating
        if (ismanager.Job != DO.Role.Boss && boVolunteer.Id != id)
            throw new BO.BlWrongInputException("id and does not correct or not manager");

        if (boVolunteer.Id != doVolunteer.Id)
        {
            throw new BO.BlWrongInputException("can not update ID");
        }

        // Logical integrity check and format. Checking if he is a manager and if not - he cannot update the position
        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);
        if (ismanager.Job != DO.Role.Boss)
        {
            if (boVolunteer.Job != (BO.Role)doVolunteer.Job)
                throw new BO.BlWrongInputException("not have promition to change the role");
        }

        // Creating a new volunteer according to the updates
        DO.Volunteer volunteerUpdate = new
            (
               boVolunteer.Id,
               boVolunteer.FullName,
               boVolunteer.PhoneNumber,
               boVolunteer.Email,
               (DO.Distance)boVolunteer.TypeDistance,
               (DO.Role)boVolunteer.Job,
               boVolunteer.Active,
               VolunteerManager.EncryptPassword(boVolunteer.Password),
               boVolunteer.FullAddress,
               null,
               null,
               boVolunteer.MaxReading
            );

        // Volunteer update
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
        _ = VolunteerManager.updateCoordinatesForVolunteerAddressAsync(volunteerUpdate, boVolunteer.FullAddress); //stage 7
    }
}

