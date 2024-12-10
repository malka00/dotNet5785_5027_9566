
using DalApi;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Helpers;

internal class VolunteerManager
{
    private static IDal s_dal = Factory.Get;

    internal static void InputIntegrity(BO.Volunteer volunteer)
    {

    }
    /// <summary>
    /// func for convert DO.volunteer for BO.VolunteerInList
    /// </summary>
    /// <param name="doVolunteer"></param>
    /// <returns></returns>
    internal static BO.VolunteerInList convertDOToBOInList(DO.Volunteer doVolunteer)
    {
        var call = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();
        int sumCalls = call.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated);
        int sumCanceld = call.Count(ass => ass.TypeEndTreat == DO.TypeEnd.SelfCancel);
        int sumExpired = call.Count(ass => ass.TypeEndTreat == DO.TypeEnd.ExpiredCancel);
        int? idCall = call.Count(ass => ass.TimeEnd == null);
        return new()
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.FullName,
            Active = doVolunteer.Active,
            SunCalls = sumCalls,
            Sumcanceled = sumCanceld,
            SumExpired = sumExpired,
            IdCall = idCall,
        };
    }
    internal static BO.CallInProgress GetCallIn(DO.Volunteer doVolunteer)
    {

        var call = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();
        DO.Assignment? assignmentTreat = call.Find(ass => ass.TimeEnd == null);
        DO.Call? callTreat = s_dal.Call.Read(assignmentTreat.CallId);
        return new()
        {
            Id = assignmentTreat.Id,
            IdCall = assignmentTreat.CallId,
            Type = (BO.CallType)callTreat.Type,
            Description = callTreat.Description,
            FullCallAddress = callTreat.FullAddress,
            TimeOpen = callTreat.TimeOpened,
            MaxTimeToClose = callTreat.MaxTimeToClose,
            StertTreet = assignmentTreat.TimeStart,
            distanceCallVolunteer = CalculateDistance(callTreat.Latitude, callTreat.Longitude, doVolunteer.Latitude, doVolunteer.Longitude),
            Status = (callTreat.MaxTimeToClose - ClockManager.Now <= GetMaxRange())
         ? BO.StatusTreat.RiskOpen : BO.StatusTreat.Treat,
        };
    }

    internal static void CheckLogic(BO.Volunteer boVolunteer)
    {
        try
        {
            CheckId(boVolunteer.Id);
            CheckPhonnumber(boVolunteer.PhoneNumber);
            CheckEmail(boVolunteer.Email);
            CheckPassword(boVolunteer.Password);
            CheckAddress(boVolunteer);

        }
        catch (BO.BlWrongItemtException ex)
        {
            throw new BO.BlWrongItemtException($"the item have logic problem", ex);
        }

    }
    /// <summary>
    /// Validates an Israeli ID number.
    /// Throws an exception if the ID is invalid.
    /// </summary>
    /// <param name="id">The ID number as an integer.</param>
    /// <exception cref="ArgumentException">Thrown if the ID is not valid.</exception>
    internal static void CheckId(int id)
    {
        // Convert the integer ID to a string to process individual digits
        string idString = id.ToString();

        // Ensure the ID is exactly 9 digits long
        if (idString.Length != 9)
        {
            throw new BO.BlWrongItemtException($"this ID {id} does not posssible");
        }

        int sum = 0;

        // Iterate through each digit in the ID
        for (int i = 0; i < 9; i++)
        {
            // Convert the character to its numeric value
            int digit = idString[i] - '0';

            // Determine the multiplier: 1 for odd positions, 2 for even positions
            int multiplier = (i % 2) + 1;

            // Multiply the digit by the multiplier
            int product = digit * multiplier;

            // If the result is two digits, sum the digits (e.g., 14 -> 1 + 4)
            if (product > 9)
            {
                product = product / 10 + product % 10;
            }

            // Add the processed digit to the total sum
            sum += product;
        }

        // תעודת זהות תקינה אם סכום ספרות הביקורת מתחלק ב-10
        if (sum % 10 != 0)
        {
            throw new BO.BlWrongItemtException($"this ID {id} does not posssible");
        }
    }
    /// <summary>
    /// Validates if a given phone number represents a valid mobile number.
    /// The number must be exactly 10 digits long, consist only of digits, 
    /// and start with the digit '0'.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate, as a string.</param>
    /// <exception cref="ArgumentException">Thrown if the phone number is not valid.</exception>
    internal static void CheckPhonnumber(string phoneNumber)
    {
        // Check if the string length is exactly 10 characters
        if (phoneNumber.Length != 10)
        {
            throw new BO.BlWrongItemtException($"The phone number {phoneNumber}must contain exactly 10 digits.");
        }

        // Check if all characters are digits
        foreach (char c in phoneNumber)
        {
            if (!char.IsDigit(c))
            {
                throw new BO.BlWrongItemtException($"The phone number {phoneNumber} must contain digits only.");
            }
        }

        // Check if the first character is '0'
        if (phoneNumber[0] != '0')
        {
            throw new BO.BlWrongItemtException($"A valid mobile phone number{phoneNumber} must start with '0'.");
        }

    }
    /// <summary>
    /// Validates whether the given string is a valid email address.
    /// The email address must match a standard email format (e.g., username@domain.com).
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <exception cref="ArgumentException">Thrown if the email address is not valid.</exception>
    internal static void CheckEmail(string email)
    {
        // Regular expression pattern for a valid email address
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // Check if the email matches the pattern
        if (!Regex.IsMatch(email, emailPattern))
        {
            throw new BO.BlWrongItemtException($"The provided email {email} address is not valid.");
        }
    }
    /// <summary>
    /// Validates if a given password is strong.
    /// A strong password must:
    /// - Be at least 5 characters long.
    /// - Contain at least one letter.
    /// - Contain at least one digit.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the password is not strong (does not meet the criteria).
    /// </exception>
    internal static void CheckPassword(string password)
    {
        // Check if the password is at least 5 characters long
        if (password.Length < 5)
            throw new BO.BlWrongItemtException($"Password {password} must be at least 5 characters long.");

        // Flags to check for at least one letter and one digit
        bool hasLetter = false;
        bool hasDigit = false;

        // Iterate over each character in the password
        foreach (char c in password)
        {
            if (char.IsLetter(c))
                hasLetter = true;
            if (char.IsDigit(c))
                hasDigit = true;

            // If both conditions are met, the password is strong
            if (hasLetter && hasDigit)
                return;
        }

        // If the password does not contain both a letter and a digit, throw an exception
        if (!hasLetter)
            throw new BO.BlWrongItemtException($"Password {password} must contain at least one letter.");
        if (!hasDigit)
            throw new BO.BlWrongItemtException($"Password{password} must contain at least one digit.");
    }

    /// <summary>
    /// This method takes an address as input and returns an array with the latitude and longitude.
    /// The request is synchronous, meaning it waits for the response before continuing.
    /// </summary>
    /// <param name="address">The address to be geocoded</param>
    /// <returns>A double array containing the latitude and longitude</returns>
    internal static double[] GetCoordinates(string address)//לטפל בחריגות!!!!!
    {
        // Checking if the address is null or empty
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be empty or null.", nameof(address));
        }

        // Constructing the URL for the geocoding service with the provided address
        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}";

        // Creating a synchronous HTTP request
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        try
        {
            // Sending the request and getting the response synchronously
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // Checking if the response status is OK
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Error in request: {response.StatusCode}");
                }

                // Reading the response body as a string
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string jsonResponse = reader.ReadToEnd();

                    // Deserializing the JSON response to extract location data
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var results = JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, options);

                    // If no results are found, throwing an exception
                    if (results == null || results.Length == 0)
                    {
                        throw new Exception("No coordinates found for the given address.");
                    }

                    // Returning the latitude and longitude as an array
                    return new double[] { double.Parse(results[0].Lat), double.Parse(results[0].Lon) };
                }
            }
        }
        catch (WebException ex)
        {
            // Handling web exceptions (e.g., network issues)
            throw new Exception("Error sending web request: " + ex.Message);
        }
        catch (Exception ex)
        {
            // Handling general exceptions
            throw new Exception("General error: " + ex.Message);
        }
    }

    /// <summary>
    /// Class to represent the structure of the geocoding response (latitude and longitude)
    /// </summary>
    private class LocationResult
    {
        // Latitude as string in the JSON response
        public string Lat { get; set; }
        // Longitude as string in the JSON response
        public string Lon { get; set; }
    }
    internal static void CheckAddress(BO.Volunteer volunteer)
    {
        double[] cordinates = GetCoordinates(volunteer.FullAddress);
        if (cordinates[0] != volunteer.Latitude || cordinates[1] == volunteer.Longitude)
            throw new BO.BlWrongItemtException($"not math cordinates");
    }


    /// <summary>
    /// מחשבת את המרחק בין שתי נקודות (קו אורך ורוחב) במטרים
    /// </summary>
    /// <param name="lat1">קו רוחב של הנקודה הראשונה</param>
    /// <param name="lon1">קו אורך של הנקודה הראשונה</param>
    /// <param name="lat2">קו רוחב של הנקודה השנייה</param>
    /// <param name="lon2">קו אורך של הנקודה השנייה</param>
    /// <returns>מרחק במטרים בין שתי הנקודות</returns>
    internal static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadius = 6371000; // רדיוס כדור הארץ במטרים

        // המרת קווי רוחב ואורך מרדיאנים למעלות
        double lat1Rad = lat1 * Math.PI / 180;
        double lon1Rad = lon1 * Math.PI / 180;
        double lat2Rad = lat2 * Math.PI / 180;
        double lon2Rad = lon2 * Math.PI / 180;

        // הפרשי קווי הרוחב והאורך
        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        // נוסחת ההאברסין
        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // המרחק הסופי במטרים
        return EarthRadius * c;
    }
}




