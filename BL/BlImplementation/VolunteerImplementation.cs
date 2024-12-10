
namespace BlImplementation;
using Helpers;
using BlApi;
using System;
using System.Linq;
using System.Linq.Expressions;
using DO;
using BO;
using System.Globalization;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void Create(BO.Volunteer boVolunteer)
    {
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
        catch (DO.DalExsitException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
    }

    public void Delete(int id)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(id);
        IEnumerable < DO.Assignment> assignments= _dal.Assignment.ReadAll(ass=>ass.VolunteerId==id);
        
        if (assignments != null&& assignments.Count(ass => ass.TimeEnd == null) > 0)
       
            throw new Exception();
        try
        {
            _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDeletImposible doEx)
        {
            throw new BO.DeleteNotPossibleException("id not valid", doEx);
        }

    }
    /// <summary>
    /// המתודה מקבלת שם מתשמש וסיסמא ומחזירה את תפקיד המשתמש, אם השם מתשמש והסיסמא נכונים
    /// </summary>
    /// <param name="usingName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref=""></exception>
    /// לעשות חריגות!!!
    public BO.Role EnterSystem(int usingName, string password)
    {
        DO.Volunteer? volunteer;
        try
        {
            volunteer = _dal.Volunteer.Read(usingName);
        }
        catch (DO.DalDeletImposible doEx)
        {
            throw new BO.BlDoesNotExistException("id and password does not exist", doEx);
        }
        if (volunteer.Password != password) throw new BO.BlDoesNotExistException("the password dont match");
        return (BO.Role)volunteer.Job;
    }


    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? active, BO.EVolunteerInList? sortBy)
    {
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();
        // Convert IEnumerable<DO.Volunteer> to IEnumerable<BO.VolunteerInList>
        IEnumerable<BO.VolunteerInList> boVolunteersInList = volunteers
            .Select(doVolunteer => VolunteerManager.convertDOToBOInList(doVolunteer));
        var filteredVolunteers = active.HasValue
              ? boVolunteersInList.Where(v => v.Active == active)
              : boVolunteersInList;
        var sortedVolunteers = sortBy.HasValue
            ? filteredVolunteers.OrderBy(v =>
                sortBy switch
                {
                    BO.EVolunteerInList.Id => (object)v.Id, // מיון לפי ת.ז
                    BO.EVolunteerInList.FullName => v.FullName, // מיון לפי שם מלא
                    BO.EVolunteerInList.Active => v.Active, // מיון לפי מצב פעיל
                    BO.EVolunteerInList.SumCalls => v.SumCalls, // מיון לפי מספר שיחות
                    BO.EVolunteerInList.Sumcanceled => v.Sumcanceled, // מיון לפי מספר ביטולים
                    BO.EVolunteerInList.SumExpired => v.SumExpired, // מיון לפי שיחות שפג תוקפן
                    BO.EVolunteerInList.IdCall => v.IdCall ?? null, // מיון לפי ת.ז שיחה
                    BO.EVolunteerInList.Ctype => v.Ctype.ToString(), // מיון לפי סוג שיחה
                })
            : filteredVolunteers.OrderBy(v => v.Id); // מיון ברירת מחדל לפי ת.ז

        return sortedVolunteers;
    }




    public BO.Volunteer Read(int id)
    {
        var doVolunteer = _dal.Volunteer.Read(id) ??
        throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");
        return new()
        {
            Id = id,
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

        DO.Volunteer doVolunteer;
        DO.Volunteer ismanager;
        try
        {
            ismanager = _dal.Volunteer.Read(id);
            doVolunteer = _dal.Volunteer.Read(boVolunteer.Id);

        }
        catch (DO.DalDeletImposible doEx)
        {
            throw new BO.DeleteNotPossibleException($"Volteer with ID={boVolunteer.Id} not exists", doEx);
        }
        if (ismanager.Job != DO.Role.Boss || boVolunteer.Id != id)
            throw new BO.BlWrongItemtException("id and  does not correct or not manager");
        if (boVolunteer.FullAddress != doVolunteer.FullAddress)
        {
            double[] cordinat = VolunteerManager.GetCoordinates(boVolunteer.FullAddress);
            boVolunteer.Latitude = cordinat[0];
            boVolunteer.Longitude = cordinat[1];
        }
        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);
        if (ismanager.Job != DO.Role.Boss)
        {
            if (boVolunteer.Job != (BO.Role)doVolunteer.Job)
                throw new BO.BlWrongItemtException("not have promition to change the role");
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
        catch (DO.DalExsitException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volteer with ID={boVolunteer.Id} not exists", ex);
        }

    }
}

