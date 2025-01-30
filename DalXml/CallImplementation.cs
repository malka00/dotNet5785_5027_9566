using DalApi;
using DO;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Dal
{
    internal class CallImplementation : ICall
    {
        /// <summary>
        /// help func return the call that is in the XElement 
        /// </summary>
        /// <param name="c">read call from xml</param>
        /// <returns> call </returns>
        /// <exception cref="FormatException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        static Call getCall(XElement s)
        {
            Call c = new DO.Call()
            {
                Id = int.TryParse((string?)s.Element("Id"), out var id) ? id : throw new FormatException("can't convert id"),
                Type = CallType.TryParse<CallType>((string?)s.Element("Type"), out CallType type) ? type : throw new FormatException("Can't convert Type"),
                Description = (string?)s.Element("Description") ?? "",
                FullAddress = (string?)s.Element("FullAddress") ?? throw new FormatException("Can't convert FullAddress"),
                Latitude = double.TryParse((string?)s.Element("Latitude"), out double latitude) ? latitude : null,/* throw new FormatException("Can't convert Latitude"),*/
                Longitude = double.TryParse((string?)s.Element("Longitude"), out double longitude) ? longitude : null/*throw new FormatException("Can't convert Longitude")*/,
                TimeOpened = DateTime.TryParse((string?)s.Element("TimeOpened"), out DateTime timeOpened) ? timeOpened : throw new FormatException("Can't convert TimeOpened"),
                MaxTimeToClose = DateTime.TryParse((string?)s.Element("MaxTimeToClose"), out DateTime maxTimeToClose) ? maxTimeToClose : null,
            };  
            return c;
        }

        /// <summary>
        /// Creating a new call in the XML file
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
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

        /// <summary>
        /// Update the call in the xml file
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Update(Call item)
        {
            List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
            if (Calls.RemoveAll(it => it.Id == item.Id) == 0)
                throw new DalDeleteImpossible($"Call with ID={item.Id} does Not exist");
            Calls.Add(item);
            XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
        }

        /// <summary>
        /// Delete call from the xml file
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Delete(int id)
        {
            List<Call> Courses = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
            if (Courses.RemoveAll(it => it.Id == id) == 0)
                throw new DalDeleteImpossible($"Call with ID={id} does Not exist");
            XMLTools.SaveListToXMLSerializer(Courses, Config.s_calls_xml);
        }

        /// <summary>
        /// delete all calls
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteAll()
        {
            XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
        }

        /// <summary>
        /// read call from the xml file
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Call? Read(int id)
        {
            // Load the existing list of calls from the XML file
            List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

            // Return the call with the specified ID, or null if not found
            return Calls.FirstOrDefault(call => call.Id == id);
        }

        /// <summary>
        /// read all calls
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
        {
            // Load the existing list of calls from the XML file
            List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

            // Apply the filter if provided, otherwise return all calls
            return filter != null ? Calls.Where(filter) : Calls;
        }

        /// <summary>
        /// The function returns the first call according to the filter parameter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Call? Read(Func<Call, bool> filter)
        {
            return XMLTools.LoadListFromXMLElement(Config.s_calls_xml).Elements().Select(s => getCall(s)).FirstOrDefault(filter);
        }
    }
}