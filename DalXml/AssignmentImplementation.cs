
using DalApi;
using DO;
using System.Data.Common;

namespace Dal
{
    internal class AssignmentImplementation : IAssignment
    {
        //public void Create(Assignment item)
        //{
        //    List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        //    //if (Assignments.Any(c => c.Id == item.Id))
        //    //    throw new DalAlreadyExistsException($"Course with ID={item.Id} does Not exist");
        //    int newId1 = Config.NextassignmentId;
        //    Assignment newItem = item with { Id = newId1 };
        //    Assignments.Add(item);
        //    XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignment_xml);
        //}
        public void Create(Assignment item)
        {
            List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
            int newId = Config.NextassignmentId;
            Assignment copy = item with { Id = newId };
            // Add the new assignment to the list
            Assignments.Add(copy);

            // Save the updated list back to the XML file
            XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignment_xml);
        }

        public void Update(Assignment item)
        {
            List<Assignment> Assignment = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
            if (Assignment.RemoveAll(it => it.Id == item.Id) == 0)
                throw new DalDeletImposible($"Assignment with ID={item.Id} does Not exist");
            Assignment.Add(item);
            XMLTools.SaveListToXMLSerializer(Assignment, Config.s_assignment_xml);
        }

        public void Delete(int id)
        {
            List<Assignment> Assignment = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
            if (Assignment.RemoveAll(it => it.Id == id) == 0)
                throw new DalDeletImposible($"Assignemt with ID={id} does Not exist");
            XMLTools.SaveListToXMLSerializer(Assignment, Config.s_assignment_xml);
        }
        public void DeleteAll()
        {
            XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignment_xml);
        }

        public Assignment? Read(int id)
        {
            // Load the existing list of assignments from the XML file
            List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);

            // Return the assignment with the specified ID, or null if not found
            return Assignments.FirstOrDefault(assignment => assignment.Id == id);
        }

        public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
        {
            // Load the existing list of assignmemts from the XML file
            List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);

            // Apply the filter if provided, otherwise return all assignment
            return filter != null ? Assignments.Where(filter) : Assignments;
        }


    }
}
