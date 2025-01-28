namespace Helpers;
using BlImplementation;
using BO;
using DalApi;
using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;



/// <summary>
/// Auxiliary functions for the implementation of the volunteer
/// </summary>
internal class VolunteerManager
{
    private static IDal s_dal = Factory.Get;   //stage 4
    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;

    internal static void SimulateVolunteerActivity() //stage 7
    {
        // var volunteerImplementation = new VolunteerImplementation();
        Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";

        // var volunteerList = volunteerImplementation.GetVolunteerList(true,null);
        var volunteerlist = GetVolunteerListHelp(true, null).ToList();
        double probability = 0.2;

        // יצירת מספר אקראי בטווח 0 עד 1
        double randomValue = s_rand.NextDouble(); // מספר בין 0.0 ל-1.0

        // בדיקה אם המספר האקראי קטן מההסתברות

        foreach (var volunteer in volunteerlist)
        {
            if (volunteer.IdCall == null && randomValue < probability)
            {
                var openCallInListsToChose = CallManager.GetOpenCallHelp(volunteer.Id, null, null).ToList();

                if (openCallInListsToChose != null && openCallInListsToChose.Count > 0)
                {
                    //choose random call for volunteer
                    var randomIndex = s_rand.Next(openCallInListsToChose.Count);
                    var chosenCall = openCallInListsToChose[randomIndex];

                    CallManager.ChoseForTreatHelp(volunteer.Id, chosenCall.Id);
                }
            }

            else if (volunteer.IdCall != null)    //there is call in treat
            {
                var callin = readHelp(volunteer.Id).CallIn!;
                if ((AdminManager.Now - callin.StartTreat) >= TimeSpan.FromHours(3))
                {
                    CallManager.ChoseForTreatHelp(volunteer.Id, callin.IdCall);
                }
                else
                {
                    int probability1 = s_rand.Next(1, 101); // מספר אקראי בין 1 ל-100

                    if (probability1 <= 10) // הסתברות של 10%
                    {
                        // ביטול הטיפול
                        CallManager.CancelTreatHelp(volunteer.Id, callin.Id);
                    }
                }
            }
            
        }
    }

    internal static IEnumerable<BO.VolunteerInList> GetVolunteerListHelp(bool? active, BO.EVolunteerInList? sortBy)
        {
            IEnumerable<DO.Volunteer> volunteers;
            lock (AdminManager.BlMutex)
                // Retrieve all volunteers from the data layer
                volunteers = s_dal.Volunteer.ReadAll().ToList() ?? throw new BO.BlNullPropertyException("There are not volunteers int database");

            // Convert IEnumerable<DO.Volunteer> to IEnumerable<BO.VolunteerInList>
            // Using the 'convertDOToBOInList' method to map each DO.Volunteer to BO.VolunteerInList
            IEnumerable<BO.VolunteerInList> boVolunteersInList = volunteers
                .Select(doVolunteer => VolunteerManager.convertDOToBOInList(doVolunteer));

            // If an 'active' filter is provided, filter the volunteers based on their active status
            // Otherwise, keep all volunteers without filtering
            var filteredVolunteers = active.HasValue
                  ? boVolunteersInList.Where(v => v.Active == active)
                  : boVolunteersInList;

            // If a 'sortBy' criteria is provided, sort the filtered volunteers by the selected property
            var sortedVolunteers = sortBy.HasValue
                ? filteredVolunteers.OrderBy(v =>
                    sortBy switch
                    {
                        // Sorting by different properties of the volunteer (ID, Full Name, etc.)
                        BO.EVolunteerInList.Id => (object)v.Id, // Sorting by ID (T.Z)
                        BO.EVolunteerInList.FullName => v.FullName, // Sorting by full name
                        BO.EVolunteerInList.Active => v.Active, // Sorting by active status
                        BO.EVolunteerInList.SumCalls => v.SumCalls, // Sorting by total number of calls
                        BO.EVolunteerInList.SumCanceled => v.SumCanceled, // Sorting by total number of cancellations
                        BO.EVolunteerInList.SumExpired => v.SumExpired, // Sorting by total number of expired calls
                        BO.EVolunteerInList.IdCall => v.IdCall ?? null, // Sorting by call ID (nullable)
                        BO.EVolunteerInList.CType => v.CType.ToString(), // Sorting by call type (converted to string)
                    })
                : filteredVolunteers.OrderBy(v => v.Id); // Default sorting by ID (T.Z) if no 'sortBy' is provided

            // Return the sorted and filtered list of volunteers
            return sortedVolunteers;
        }
    


    internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// func for convert DO.volunteer for BO.VolunteerInList
    /// </summary>
    /// <param name="doVolunteer"></param>
    /// <returns> BO.VolunteerInList </returns>
    internal static BO.VolunteerInList convertDOToBOInList(DO.Volunteer doVolunteer)
    {
        List<DO.Assignment> assignments;
        lock (AdminManager.BlMutex) //stage 7
            assignments = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();
        int sumCalls = assignments.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated);
        int sumCanceld = assignments.Count(ass => ass.TypeEndTreat == DO.TypeEnd.SelfCancel);
        int sumExpired = assignments.Count(ass => ass.TypeEndTreat == DO.TypeEnd.ExpiredCancel);
        // idCall = call.Count(ass => ass.TimeEnd == null);
        var assignmentToCallID = assignments.Find(ass => ass.TimeEnd == null);
        int? idCall;
        BO.CallType ctype;
        if (assignmentToCallID != null && assignmentToCallID.TimeEnd == null)
        {
            idCall = assignmentToCallID.CallId;
            DO.Call call;
            lock (AdminManager.BlMutex) //stage 7
                call = s_dal.Call.Read(assignmentToCallID.CallId);
            ctype = (BO.CallType)call.Type;
        }
        else

        {
            idCall = null;
            ctype = BO.CallType.None;
        }
        return new()
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.FullName,
            Active = doVolunteer.Active,
            SumCalls = sumCalls,
            SumCanceled = sumCanceld,
            SumExpired = sumExpired,
            IdCall = idCall,
            CType = ctype
        };
    }
    internal static BO.Volunteer readHelp(int id)
    {
        DO.Volunteer doVolunteer;
        lock (AdminManager.BlMutex)//stage 7
            doVolunteer = s_dal.Volunteer.Read(id) ?? throw new BO.BlWrongInputException($"Volunteer with ID={id} does Not exist");
        lock (AdminManager.BlMutex)//stage 7
            return new()
        {
            Id = id,
            Email = doVolunteer.Email,
            MaxReading = doVolunteer.MaxReading,
            FullName = doVolunteer.FullName,
            PhoneNumber = doVolunteer.PhoneNumber,
            TypeDistance = (BO.Distance)doVolunteer.TypeDistance,
            Job = (BO.Role)doVolunteer.Job,
            Active = doVolunteer.Active,
            Password = VolunteerManager.DecryptPassword(doVolunteer.Password),
            FullAddress = doVolunteer.FullAddress,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            SumCalls = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.TypeEndTreat == DO.TypeEnd.Treated),
            SumCanceled = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id &&
                (a.TypeEndTreat == DO.TypeEnd.ManagerCancel || a.TypeEndTreat == DO.TypeEnd.SelfCancel)), // ביטול עצמי או מהנל
            SumExpired = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.TypeEndTreat == DO.TypeEnd.ExpiredCancel),

            CallIn = VolunteerManager.GetCallIn(doVolunteer),
        };
    }

    internal static string EncryptPassword(string password)
    {
        int shift = 3;
        StringBuilder encryptedPassword = new StringBuilder();

        foreach (char c in password)
        {
            // Shift every character by the given value (ASCII manipulation)
            char encryptedChar = (char)(c + shift);
            encryptedPassword.Append(encryptedChar);
        }

        return encryptedPassword.ToString();
    }

    internal static string DecryptPassword(string encryptedPassword)
    {
        int shift = 3;

        StringBuilder decryptedPassword = new StringBuilder();

        foreach (char c in encryptedPassword)
        {
            // Shift the character back to its original value
            char decryptedChar = (char)(c - shift);
            decryptedPassword.Append(decryptedChar);
        }

        return decryptedPassword.ToString();
    }

    /// <summary>
    /// The GetCallIn function takes an object of type DO.Volunteer and returns an object of type BO.CallInProgress. 
    /// It is responsible for searching and creating information about a call in which the volunteer is currently involved in treatment, 
    /// and it returns the information in a structured format.
    /// </summary>
    /// <param name="doVolunteer"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlWrongInputException"></exception>
    internal static BO.CallInProgress GetCallIn(DO.Volunteer doVolunteer)
    {
        List<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)//stage 7
            assignments = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();
        DO.Assignment? assignmentTreat = assignments.Find(ass => /*ass.TimeEnd == null ||*/ ass.TypeEndTreat == null);
        if (assignmentTreat == null) { return null; }
        DO.Call? callTreat;
        lock (AdminManager.BlMutex)//stage 7
            callTreat = s_dal.Call.Read(assignmentTreat.CallId);
        if (callTreat == null) { throw new BO.BlWrongInputException($"there is no call with this DI {assignmentTreat.CallId}"); }
        //double[] cordinate = GetCoordinatesAsync(doVolunteer.FullAddress);
        //double latitude = cordinate[0];
        //double longitude = cordinate[1];
        AdminImplementation admin = new AdminImplementation();
        BO.StatusTreat status = CallManager.GetCallStatus(callTreat);

        return new()
        {
            Id = assignmentTreat.Id,
            IdCall = assignmentTreat.CallId,
            Type = (BO.CallType)callTreat.Type,
            Description = callTreat.Description,
            FullCallAddress = callTreat.FullAddress,
            TimeOpen = callTreat.TimeOpened,
            MaxTimeToClose = callTreat.MaxTimeToClose,
            StartTreat = assignmentTreat.TimeStart,
            distanceCallVolunteer = CalculateDistance(callTreat.Latitude,   callTreat.Longitude,doVolunteer.Latitude,doVolunteer.Longitude),
            //Status = status,
            Status = (callTreat.MaxTimeToClose - AdminManager.Now <= s_dal.Config.RiskRange ? BO.StatusTreat.TreatInRisk : BO.StatusTreat.TreatInRisk),
        };
    }

    /// <summary>
    /// Checks the format of a Volunteer object to ensure all fields are valid.
    /// </summary>
    /// <param name="boVolunteer">The Volunteer object to validate.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when one or more fields in the Volunteer object are invalid.
    /// </exception>
    internal static void CheckFormat(BO.Volunteer boVolunteer)
    {
        /// <summary>
        /// Validate the ID of the volunteer.
        /// The ID must be a positive integer and consist of 8 to 9 digits.
        /// </summary>
        if (boVolunteer.Id <= 0 || boVolunteer.Id.ToString().Length < 8 || boVolunteer.Id.ToString().Length > 9)
        {
            throw new BO.BlWrongItemException($"Invalid ID {boVolunteer.Id}. It must be 8-9 digits.");
        }
        /// <summary>
        /// Validate the FullName field.
        /// The name must not be null, empty, or consist of only whitespace.
        /// </summary>
        if (string.IsNullOrWhiteSpace(boVolunteer.FullName) || !Regex.IsMatch(boVolunteer.FullName, @"^[a-zA-Z\s]+$"))
        {
            throw new BO.BlWrongItemException($"FullName {boVolunteer.FullName} cannot be null, empty, or contain invalid characters.");
        }

        /// <summary>
        /// Validate the PhoneNumber field.
        /// The phone number must be exactly 10 digits and start with 0.
        /// </summary>
        if (string.IsNullOrWhiteSpace(boVolunteer.FullName))
        {
            throw new BO.BlWrongItemException($"FullName {boVolunteer.FullName} cannot be null or empty.");
        }

        /// <summary>
        /// it checks if the FullName property of the boVolunteer object contains invalid characters.
        /// </summary>
        if (boVolunteer.FullName.Any(c => !Char.IsLetter(c) && !Char.IsWhiteSpace(c)))
        {
            throw new BO.BlWrongItemException($"FullName {boVolunteer.FullName} contains invalid characters.");
        }
        /// <summary>
        /// Validate the Email field.
        /// The email must match the standard email format.
        /// </summary>
        if (!Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new BO.BlWrongItemException("Invalid Email format.");
        }

        /// <summary>
        /// Validate the MaxReading field.
        /// If provided, it must be a positive number.
        /// </summary>
        if (boVolunteer.MaxReading.HasValue)
        {
            if (!double.TryParse(boVolunteer.MaxReading.Value.ToString(), out double maxReadingValue) || maxReadingValue <= 0)
            {
                throw new BO.BlWrongItemException($"MaxReading {boVolunteer.MaxReading} must be a positive number.");
            }
        }

        /// <summary>
        /// Validate the Latitude field.
        /// If provided, it must be between -90 and 90 (inclusive).
        /// </summary>
        if (boVolunteer.Latitude.HasValue && (boVolunteer.Latitude.Value < -90 || boVolunteer.Latitude.Value > 90))
        {
            throw new BO.BlWrongItemException("Latitude must be between -90 and 90.");
        }

        /// <summary>
        /// Validate the Longitude field.
        /// If provided, it must be between -180 and 180 (inclusive).
        /// </summary>
        if (boVolunteer.Longitude.HasValue && (boVolunteer.Longitude.Value < -180 || boVolunteer.Longitude.Value > 180))
        {
            throw new BO.BlWrongItemException($"Longitude {boVolunteer.Longitude} must be between -180 and 180.");
        }

        /// <summary>
        /// Add any additional validation checks here if needed in the future.
        /// </summary>
    }

    /// <summary>
    /// The CheckLogic function checks the logic of an object of type BO.Volunteer. It performs a series of checks on the volunteer's properties, 
    /// and if any of them fail, it throws an exception with an appropriate message
    /// </summary>
    /// <param name="boVolunteer"></param>
    /// <exception cref="BO.BlWrongItemException"></exception>
    internal static void CheckLogic(BO.Volunteer boVolunteer)
    {
        //try
        //{
        CheckId(boVolunteer.Id);
        CheckPhoneNumber(boVolunteer.PhoneNumber);
        CheckEmail(boVolunteer.Email);
        CheckPassword(boVolunteer.Password);
        // CheckAddress(boVolunteer);
        CheckActive(boVolunteer);

        //}
        //catch (BO.BlWrongItemException ex)
        //{
        //    throw new BO.BlWrongItemException($"the item have logic problem", ex);
        //}
    }

    internal static void CheckActive(BO.Volunteer volunteer)
    {
        if (!volunteer.Active && (volunteer.CallIn != null))
            throw new BO.BlWrongItemException($"Volunteer canot be unActive with call in treat");
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
            throw new BO.BlWrongItemException($"this ID {id} does not posssible");
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

        //check id digit
        if (sum % 10 != 0)
        {
            throw new BO.BlWrongItemException($"this ID {id} does not posssible");
        }
    }

    /// <summary>
    /// Validates if a given phone number represents a valid mobile number.
    /// The number must be exactly 10 digits long, consist only of digits, 
    /// and start with the digit '0'.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate, as a string.</param>
    /// <exception cref="ArgumentException">Thrown if the phone number is not valid.</exception>
    internal static void CheckPhoneNumber(string phoneNumber)
    {
        // Check if the string length is exactly 10 characters
        if (phoneNumber.Length != 10)
        {
            throw new BO.BlWrongItemException($"The phone number {phoneNumber}must contain exactly 10 digits.");
        }

        // Check if all characters are digits
        foreach (char c in phoneNumber)
        {
            if (!char.IsDigit(c))
            {
                throw new BO.BlWrongItemException($"The phone number {phoneNumber} must contain digits only.");
            }
        }

        // Check if the first character is '0'
        if (phoneNumber[0] != '0')
        {
            throw new BO.BlWrongItemException($"A valid mobile phone number{phoneNumber} must start with '0'.");
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
            throw new BO.BlWrongItemException($"The provided email {email} address is not valid.");
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
            throw new BO.BlWrongItemException($"Password {password} must be at least 5 characters long.");

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
            throw new BO.BlWrongItemException($"Password {password} must contain at least one letter.");
        if (!hasDigit)
            throw new BO.BlWrongItemException($"Password{password} must contain at least one digit.");
    }

    internal static async Task updateCoordinatesForVolunteerAddressAsync(DO.Volunteer doVolunteer, string address)
    {
        if (doVolunteer.FullAddress is not null)
        {
            double[] coordinates;
            //try
            //{
                coordinates = await Tools.GetCoordinatesAsync(address);
            //}
            //catch(BO.BlWrongInputException ex)
            //{ throw new BO.BlWrongInputException(ex.Message); }
          
            if (coordinates == null || coordinates.Length != 2)
            {
                throw new BO.BlWrongInputException("Failed to update coordinates due to invalid address.");
            }

            if (coordinates is not null)
            {
                doVolunteer = doVolunteer with { Latitude = coordinates[0], Longitude = coordinates[1] };
               
                lock (AdminManager.BlMutex)
                    s_dal.Volunteer.Update(doVolunteer);
                Observers.NotifyListUpdated();
                Observers.NotifyItemUpdated(doVolunteer.Id);
            }
        }
    }

    /// <summary>
    /// This method takes an address as input and returns an array with the latitude and longitude.
    /// The request is synchronous, meaning it waits for the response before continuing.
    /// </summary>
    /// <param name="address">The address to be geocoded</param>
    /// <returns>A double array containing the latitude and longitude</returns>
    /// 


   

    /// <summary>
    /// Class to represent the structure of the geocoding response(latitude and longitude)
    /// </summary>
  

    /// <summary>
    /// Calculates the distance between two points (latitude and longitude) in meters.
    /// </summary>
    /// <param name="lat1">Latitude of the first point</param>
    /// <param name="lon1">Longitude of the first point</param>
    /// <param name="lat2">Latitude of the second point</param>
    /// <param name="lon2">Longitude of the second point</param>
    /// <returns>Distance in meters between the two points</returns>
    internal static double CalculateDistance(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        if (lat1 == null || lon1 == null || lat2 == null || lon2 == null)
            return 0;

        //  // Convert latitude and longitude from degrees to radians
        double lat1Rad = (double)lat1 * (Math.PI / 180);
        double lon1Rad = (double)lon1 * (Math.PI / 180);
        double lat2Rad = (double)lat2 * (Math.PI / 180);
        double lon2Rad = (double)lon2 * (Math.PI / 180);


        const double R = 6371; // Radius of the Earth in kilometers

        double dLat = lat2Rad - lat1Rad;
        double dLon = lon2Rad - lon1Rad;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // Distance in kilometers
    }
}




