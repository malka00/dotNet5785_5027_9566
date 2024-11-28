

using DalApi;
using DO;
using System;
using System.Xml.Linq;

namespace Dal;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        if (volunteersRoot.Elements().Any(v => (int?)v.Element("Id") == item.Id))
            throw new DalExsitException($"Volunteer with ID={item.Id} already exists.");

        volunteersRoot.Add(new XElement("Volunteer", CreateVolunteerElement(item)));
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }
    private static XElement CreateVolunteerElement(Volunteer v)
    {
        return new XElement("Volunteer",
            new XElement("Id", v.Id),
            new XElement("Name", v.FullName),
            new XElement("Number_phone", v.PhoneNumber),
            new XElement("Email", v.Email),
            new XElement("Role", v.Job),
            new XElement("Distance_Type", v.TypeDistance),
            new XElement("FullCurrentAddress", v.FullAddress),
            new XElement("Latitude", v.Latitude),
            new XElement("Longitude", v.Longitude),
            new XElement("Active", v.Active),
            new XElement("distance", v.MaxReading)
        );
    }
    static Volunteer getVolunteer(XElement v)
    {
        return new DO.Volunteer()
        {
            Id = v.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            FullName = (string?)v.Element("Name") ?? "",
            Email = (string?)v.Element("Email") ?? "",
            Job = v.ToEnumNullable<Role>("Role") ?? Role.Volunteer,
            TypeDistance = v.ToEnumNullable<Distance>("Distance_Type") ?? Distance.Aerial,
            PhoneNumber = (string?)v.Element("PhoneNumber") ?? "",
            Active = (bool?)v.Element("IsActive") ?? false,
            Password = (string?)v.Element("Password"),
            FullAddress = (string?)v.Element("FullAddress"),
            Latitude = (double?)v.Element("Latitude"),
            Longitude = (double?)v.Element("Longitude"),
            MaxReading = (double?)v.Element("MaxReading")
        };
    }
    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(int id)
    {
        XElement? volunteerElem =
    XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return volunteerElem is null ? null : getVolunteer(volunteerElem);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(s => getVolunteer(v)).FirstOrDefault(filter);
    }

    public void Update(Volunteer item)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? volunteerElem = volunteersRoot.Elements().FirstOrDefault(v => (int?)v.Element("Id") == item.Id)
                                 ?? throw new DalDeletImposible($"Volunteer with ID={item.Id} does not exist.");

        volunteerElem.Remove();

        volunteersRoot.Add(new XElement("Volunteer", CreateVolunteerElement(item)));
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }

}
