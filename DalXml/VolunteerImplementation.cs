

using DalApi;
using DO;
using System;
using System.Xml.Linq;

namespace Dal;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// help func return the volunteer that is in the XElement 
    /// </summary>
    /// <param name="s">n XElement vatrible </param>
    /// <returns>  the volunteer  from the XElement</returns>
    /// <exception cref="FormatException"></exception>
    static Volunteer getVolunteer(XElement s)
    {
        Volunteer v = new DO.Volunteer()
        {
            Id = int.TryParse((string?)s.Element("ID"), out var id) ? id : throw new FormatException("can't convert id"),
            FullName = (string?)s.Element("fullName") ?? "",
            PhoneNumber = (string?)s.Element("phone") ?? "",
            Email = (string?)s.Element("email") ?? "",
            Active = bool.TryParse((string?)s.Element("isActive"), out bool active) ? active : throw new FormatException("can't convert active"),
            Job = Role.TryParse((string?)s.Element("role"), out Role role) ? role : throw new FormatException("can't convert role "),
            TypeDistance = Distance.TryParse((string?)s.Element("distance"), out Distance dis) ? dis : throw new FormatException("can't convert distance "),
            //password = (string?)s.Element("password") ?? null,
            FullAddress = (string?)s.Element("address") ?? null,
            Longitude = double.TryParse((string?)s.Element("longitude"), out double longitude) ? longitude : null,
            Latitude = double.TryParse((string?)s.Element("latitude"), out double latitude) ? latitude : null,
            MaxReading = double.TryParse((string?)s.Element("maxDistance"), out double maxDis) ? maxDis : null,
        };
        return v;
    }
    /// <summary>
    ///help func return the volunteer in XElement.
    /// </summary>
    /// <param name="volunteer"> volunteer from the user </param>
    /// <returns></returns>
    static XElement createVolunteerElement(Volunteer volunteer)
    {
        XElement volunteerXml = new XElement("Volunteer",
        new XElement("ID", volunteer.Id),
                              new XElement("fullName", volunteer.FullName),
                              new XElement("phone", volunteer.PhoneNumber),
                                new XElement("email", volunteer.Email),
                                 new XElement("isActive", volunteer.Active),
                                  new XElement("role", volunteer.Job),
                                   new XElement("distance", volunteer.TypeDistance),
                                  //  (volunteer.password != null ? new XElement("password", volunteer.password!) : null),
                                    (volunteer.FullAddress != null ? new XElement("address", volunteer.FullAddress!) : null),
                                     (volunteer.Longitude != null ? new XElement("longitude", volunteer.Longitude!) : null),
                                      (volunteer.Latitude != null ? new XElement("latitude", volunteer.Latitude!) : null),
                                       (volunteer.MaxReading != null ? new XElement("maxDistance", volunteer.MaxReading) : null)


                              );
        return volunteerXml;


    }
    /// <summary>
    /// add the volunteer to the Xml file 
    /// </summary>
    /// <param name="item"> volunteer to add to the </param>
    /// <exception cref="DO.DalAlreadyExistsException"></exception>
    public void Create(Volunteer item)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        if ((volunteerRootElem.Elements().FirstOrDefault(st => (int?)st.Element("ID") == item.Id)) != null)
            throw new DO.DalExsitException($"Student with ID={item.Id} already  exist");

        volunteerRootElem.Add(new XElement(createVolunteerElement(item)));
        
        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_volunteers_xml);
    }
    
    //public void Create(Volunteer item)
    //{
    //    XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

    //    if (volunteersRoot.Elements().Any(v => (int?)v.Element("Id") == item.Id))
    //        throw new DalExsitException($"Volunteer with ID={item.Id} already exists.");

    //    volunteersRoot.Add(new XElement("Volunteer", CreateVolunteerElement(item)));
    //    XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    //}

    //private static XElement CreateVolunteerElement(Volunteer v)
    //{
    //    return new XElement("Volunteer",
    //        new XElement("Id", v.Id),
    //        new XElement("Name", v.FullName),
    //        new XElement("Number_phone", v.PhoneNumber),
    //        new XElement("Email", v.Email),
    //        new XElement("Role", v.Job),
    //        new XElement("Distance_Type", v.TypeDistance),
    //        new XElement("FullCurrentAddress", v.FullAddress),
    //        new XElement("Latitude", v.Latitude),
    //        new XElement("Longitude", v.Longitude),
    //        new XElement("Active", v.Active),
    //        new XElement("distance", v.MaxReading)
    //    );
    //}
    //static Volunteer getVolunteer(XElement v)
    //{
    //    return new DO.Volunteer()
    //    {
    //        Id = int.TryParse((string?) v.Element("ID"), out var id) ? id : throw new FormatException("can't convert id"),
    //        //        Id = v.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
    //        FullName = (string?)v.Element("Name") ?? "",
    //        Email = (string?)v.Element("Email") ?? "",
    //        Job = v.ToEnumNullable<Role>("Role") ?? Role.Volunteer,
    //        TypeDistance = v.ToEnumNullable<Distance>("Distance_Type") ?? Distance.Aerial,
    //        PhoneNumber = (string?)v.Element("PhoneNumber") ?? "",
    //        Active = bool.TryParse((string?)v.Element("isActive"), out bool active) ? active : throw new FormatException("can't convert active"),
    //       // Active = (bool?)v.Element("IsActive") ?? false,
    //        Password = (string?)v.Element("Password"),
    //        FullAddress = (string?)v.Element("FullAddress"),
    //        Latitude = (double?)v.Element("Latitude"),
    //        Longitude = (double?)v.Element("Longitude"),
    //        MaxReading = (double?)v.Element("MaxReading")
    //    };
    //}

    public void Delete(int id)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? volunteerElem = volunteersRoot.Elements()
                                                 .FirstOrDefault(v => (int?)v.Element("Id") == id)
                             ?? throw new DalDeletImposible($"Volunteer with ID={id} does not exist.");

        volunteerElem.Remove();

        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }

    public void DeleteAll()
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        volunteersRoot.RemoveAll();

        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }

    public Volunteer? Read(int id)
    {
        XElement? volunteerElem =
    XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return volunteerElem is null ? null : getVolunteer(volunteerElem);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v => getVolunteer(v)).FirstOrDefault(filter);
    }

    public void Update(Volunteer item)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? volunteerElem = volunteersRoot.Elements().FirstOrDefault(v => (int?)v.Element("Id") == item.Id)
                                 ?? throw new DalDeletImposible($"Volunteer with ID={item.Id} does not exist.");

        volunteerElem.Remove();

        volunteersRoot.Add(new XElement("Volunteer", createVolunteerElement(item)));
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        var volunteers = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)
                                            .Elements()
                                            .Select(v => getVolunteer(v));
        return filter == null ? volunteers : volunteers.Where(filter);
    }

}
