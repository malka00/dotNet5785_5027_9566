
using DalApi;
using DO;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Dal
{
    internal class AssignmentImplementation : IAssignment
    {
        /// <summary>
        /// help func return the assignment that is in the XElement 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        static Assignment getAssignment(XElement s)
        {
            Assignment a = new DO.Assignment()
            {
                Id = int.TryParse((string?)s.Element("Id"), out var id) ? id : throw new FormatException("can't convert id"),
                CallId = int.TryParse((string?)s.Element("CallId"), out var callId) ? callId : throw new FormatException("can't convert id"),
                VolunteerId = int.TryParse((string?)s.Element("VolunteerId"), out var volunteerId) ? volunteerId : throw new FormatException("can't convert id"),
                TimeStart = DateTime.TryParse((string?)s.Element("TimeStart"), out DateTime timeStart) ? timeStart : throw new FormatException("Can't convert TimeStart"),
                TimeEnd = DateTime.TryParse((string?)s.Element("TimeEnd"), out DateTime timeEnd) ? timeEnd : null,
                TypeEndTreat = TypeEnd.TryParse<TypeEnd>((string?)s.Element("TypeEndTreat"), out TypeEnd typeEndTreat) ? typeEndTreat : null 
            };
            return a;
        }
        /// <summary>
        /// Creating a new task in an XML file
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Create(Assignment item)
        {
            List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
            int newId = Config.NextAssignmentId;
            Assignment copy = item with { Id = newId };
            // Add the new assignment to the list
            Assignments.Add(copy);

            // Save the updated list back to the XML file
            XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignment_xml);
        }

        /// <summary>
        /// update assignment
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Update(Assignment item)
        {
            List<Assignment> Assignment = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
            if (Assignment.RemoveAll(it => it.Id == item.Id) == 0)
                throw new DalDeleteImpossible($"Assignment with ID={item.Id} does Not exist");
            Assignment.Add(item);
            XMLTools.SaveListToXMLSerializer(Assignment, Config.s_assignment_xml);
        }

        /// <summary>
        /// Delete assignment from the xml file
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Delete(int id)
        {
            List<Assignment> Assignment = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
            if (Assignment.RemoveAll(it => it.Id == id) == 0)
                throw new DalDeleteImpossible($"Assignemt with ID={id} does Not exist");
            XMLTools.SaveListToXMLSerializer(Assignment, Config.s_assignment_xml);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteAll()
        {
            XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignment_xml);
        }

        /// <summary>
        /// read assignment from the xml file
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Assignment? Read(int id)
        {
            // Load the existing list of assignments from the XML file
            List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);

            // Return the assignment with the specified ID, or null if not found
            return Assignments.FirstOrDefault(assignment => assignment.Id == id);
        }

        /// <summary>
        /// Read All assignments from the xml file
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
        {
            // Load the existing list of assignments from the XML file
            List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);

            // Apply the filter if provided, otherwise return all assignment
            return filter != null ? Assignments.Where(filter) : Assignments;
        }

        /// <summary>
        /// The function returns the first assignment according to the filter parameter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Assignment? Read(Func<Assignment, bool> filter)
        {
            return XMLTools.LoadListFromXMLElement(Config.s_assignment_xml).Elements().Select(s => getAssignment(s)).FirstOrDefault(filter); 
        }

    }
}

