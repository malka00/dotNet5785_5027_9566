

using DalApi;
using DO;
using System.Data.Common;

namespace Dal
{
    internal class CallImplemantation : ICall
    {

        public void Create(Call item)
        {
            // Load the existing list of calls from the XML file
            List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

            // Generate a new ID for the new call object (assuming ID is auto-generated)
            // If the ID field is auto-generated, ensure it's being set correctly here
            int newId = Config.NextCallId;
            Call copy = item with { Id = newId };


            // Add the new call to the list
            Calls.Add(copy);

            // Save the updated list back to the XML file
            XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
        }


        public void Update(Call item)
        {
            List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
            if (Calls.RemoveAll(it => it.Id == item.Id) == 0)
                throw new DalDeletImposible($"Call with ID={item.Id} does Not exist");
            Calls.Add(item);
            XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
        }

        public void Delete(int id)
        {
            List<Call> Courses = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
            if (Courses.RemoveAll(it => it.Id == id) == 0)
                throw new DalDeletImposible($"Call with ID={id} does Not exist");
            XMLTools.SaveListToXMLSerializer(Courses, Config.s_calls_xml);
        }
        public void DeleteAll()
        {
            XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
        }

        public Call? Read(int id)
        {
            // Load the existing list of calls from the XML file
            List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

            // Return the call with the specified ID, or null if not found
            return Calls.FirstOrDefault(call => call.Id == id);
        }

        public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
        {
            // Load the existing list of calls from the XML file
            List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

            // Apply the filter if provided, otherwise return all calls
            return filter != null ? Calls.Where(filter) : Calls;
        }

        
    }
}
