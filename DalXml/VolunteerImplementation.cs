

using DalApi;
using DO;
using System;
using System.Xml.Linq;

namespace Dal;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }






           static Volunteer getStudent(XElement s)
        {
            return new DO.Volunteer()
            {
                Id = s.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
                FullName = (string?)s.Element("Name") ?? "",
                Email = (string?)s.Element("Email") ?? "",
                PhoneNumber = (string?)s.Element("PhoneNumber") ?? "",
               // TypeDistance = Enum.TryParse<Distance>((string?)s.Element("TypeDistance"), true, out var typeDistance) ? typeDistance : (Distance?)null,
               // Job = Enum.TryParse<Role>((string?)s.Element("Job"), true, out var job) ? job : (Role?)null,
                Active = (bool?)s.Element("IsActive") ?? false,
                Password = (string?)s.Element("Password"),
                FullAddress = (string?)s.Element("FullAddress"),
                Latitude = (double?)s.Element("Latitude"),
                Longitude = (double?)s.Element("Longitude"),
                MaxReading = (double?)s.Element("MaxReading")
            };
        }

   

    public Volunteer? Read(int id)
    {
        XElement? studentElem =
    XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return studentElem is null ? null : getStudent(studentElem);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(s => getStudent(s)).FirstOrDefault(filter);
    }

    public void Update(Volunteer item)
    {
        XElement studentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (studentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDeletImposible($"Volunteer with ID={item.Id} does Not exist"))
                .Remove();

        studentsRootElem.Add(new XElement("Volunteer", createStudentElement(item)));

        XMLTools.SaveListToXMLElement(studentsRootElem, Config.s_volunteers_xml);
    }

}
