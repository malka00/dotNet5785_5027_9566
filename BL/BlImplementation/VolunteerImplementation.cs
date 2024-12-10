
namespace BlImplementation;
using Helpers;
using BlApi;
using System;
using System.Linq;
using System.Linq.Expressions;
using DO;
using BO;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void Create(BO.Volunteer boVolunteer)
    {
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
        DO.Volunteer? doVolunteer;
        try
        {
            doVolunteer = _dal.Volunteer.Read(id);
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



    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? activ, BO.EVolunteerInList sortBy)
    {




        //    // סינון לפי מתנדבים פעילים / לא פעילים אם הערך 'activ' אינו null
        //    if (activ.HasValue)
        //    {
        //        volunteerList = volunteerList.Where(v => v.Active == activ.Value).ToList();
        //    }

        //    // מיון הרשימה לפי הפרמטר 'sortBy'
        //    if (sort)
        //    {
        //        switch (sortBy.Value)
        //        {
        //            case BO.EVolunteerInList.Id:
        //                volunteerList = volunteerList.OrderBy(v => v.Id).ToList();
        //                break;
        //            case BO.EVolunteerInList.FullName:
        //                volunteerList = volunteerList.OrderBy(v => v.FullName).ToList();
        //                break;
        //            case BO.EVolunteerInList.Active:
        //                volunteerList = volunteerList.OrderBy(v => v.IsActive).ToList();
        //                break;
        //            case BO.EVolunteerInList.SunCalls:
        //                volunteerList = volunteerList.OrderBy(v => v.SunCalls).ToList();
        //                break;
        //            case BO.EVolunteerInList.Sumcanceled:
        //                volunteerList = volunteerList.OrderBy(v => v.Sumcanceled).ToList();
        //                break;
        //            case BO.EVolunteerInList.SunExpired:
        //                volunteerList = volunteerList.OrderBy(v => v.SunExpired).ToList();
        //                break;
        //            case BO.EVolunteerInList.IdCall:
        //                volunteerList = volunteerList.OrderBy(v => v.IdCall).ToList();
        //                break;
        //            case BO.EVolunteerInList.Ctype:
        //                volunteerList = volunteerList.OrderBy(v => v.Ctype).ToList();
        //                break;
        //            default:
        //                throw new ArgumentException("Invalid sortBy value");
        //        }
        //    }
        //    else
        //    {
        //        // מיון ברירת מחדל לפי תעודת זהות
        //        volunteerList = volunteerList.OrderBy(v => v.Id).ToList();
        //    }

        //    return volunteerList;
        //}

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
        if (boVolunteer.Job != BO.Role.Boss || boVolunteer.Id != id)
            throw new BO.BlWrongItemtException("id and  does not correct or not manager");

        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);
        DO.Volunteer doVolunteer;
        try
        {
            doVolunteer = _dal.Volunteer.Read(id);
        }
        catch (DO.DalDeletImposible doEx)
        { 
            throw new BO.DeleteNotPossibleException($"Volteer with ID={boVolunteer.Id} not exists", doEx); 
        }
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
        double[] cordinate = VolunteerManager.GetCoordinates(boVolunteer.FullAddress);
        var changedFields = typeof(BO.Volunteer).GetProperties()
        .Where(prop => !Equals(prop.GetValue(doVolunteer), prop.GetValue(boVolunteer)))
        .Select(prop => prop.Name)
        .ToList();
        var allowedFields = new List<string>();

        if (boVolunteer.Job == BO.Role.Boss) // אם המבקש הוא מנהל
        {
            allowedFields = typeof(BO.Volunteer).GetProperties().Select(p => p.Name).ToList(); // כל השדות מותרים
        }
        else if (boVolunteer.Job == BO.Role.Volunteer) // אם המבקש הוא אותו מתנדב עצמו
        {
            allowedFields = new List<string>
        {
            nameof(BO.Volunteer.FullName),
            nameof(BO.Volunteer.PhoneNumber),
            nameof(BO.Volunteer.Email),
            nameof(BO.Volunteer.Password),
            nameof(BO.Volunteer.FullAddress),
            nameof(BO.Volunteer.MaxReading),
            nameof(BO.Volunteer.TypeDistance)
        };
            // מעבר על שמות השדות שהשתנו ושינוי הערכים ב-existingVolunteer אם הם מורשים
            foreach (var fieldName in changedFields)
            {
                // בדוק אם השדה נמצא ברשימת השדות המותרים לעדכון
                if (!allowedFields.Contains(fieldName))
                {
                    throw new BO.BlWrongItemtException($"Field '{fieldName}' cannot be updated by the requester");
                }

                // שליפת ה-Property לפי השם
                var property = typeof(BO.Volunteer).GetProperty(fieldName);

                // בדיקת null ובדיקת שהשדה ניתן לכתיבה
                if (property != null && property.CanWrite)
                {
                    // עדכון הערך ב-existingVolunteer לערך מ-updatedVolunteer
                    var newValue = typeof(BO.Volunteer).GetProperty(fieldName)?.GetValue(boVolunteer);
                    property.SetValue(volunteerUpdate, newValue);
                }
            }
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
}
