using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
namespace Helpers;

internal static class Tools
{
   

    
    public static async Task<double[]> GetCoordinatesAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be empty or null.", nameof(address));
        }

        string apiKey = "pk.51229b895167367503aba7d1c5dd9afc"; // מפתח API
        string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

           Thread.Sleep(500);

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Error in request: {response.StatusCode}");
                }

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string jsonResponse = reader.ReadToEnd();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var results = JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, options);

                    if (results == null || results.Length == 0)
                    {
                        throw new BO.BlWrongInputException("No coordinates found for the given address.");
                    }
                    if (results.Length > 1)
                     throw new BO.BlWrongInputException("No spesific address."); 
                 
                    return new double[] { double.Parse(results[0].Lat), double.Parse(results[0].Lon) };
                }
            }
        }

        catch (WebException ex) when (ex.Response is HttpWebResponse httpResponse)
        {
            throw new Exception($"HTTP Error: {(int)httpResponse.StatusCode} {httpResponse.StatusDescription}");
            // throw new BO.BlWrongInputException ("The address is not good");
        }
        //catch (Exception ex)
        //{
        //    throw new Exception($"General error: {ex.Message}");
        //}
    }
    private class LocationResult
    {
        // Latitude as string in the JSON response
        public string Lat { get; set; }
        // Longitude as string in the JSON response
        public string Lon { get; set; }
    }
    /// <summary>
    /// The ToStringProperty function is an extension method extended on any object (T),
    /// which converts the object to a string. Its purpose is to convert the values of all the properties of the 
    /// object into a string, so that each property and its corresponding value are included in the final string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in typeof(T).GetProperties())
        {
            var value = item.GetValue(t, null);
            str += item.Name + ": ";
            if (value is not string && value is IEnumerable)
            {
                str += "\n";
                foreach (var it in (IEnumerable<object>)value)
                {
                    str += it.ToString() + '\n';
                }
            }
            else
                str += value?.ToString() + '\n';
            //str += "\n" + item.CourseName + ": " + item.GetValue(t, null);
        }
        return str;
    }
}




