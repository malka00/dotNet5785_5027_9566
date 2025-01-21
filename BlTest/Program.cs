namespace BlTest;
using BlApi;

using DalApi;

using System;
using System.Linq.Expressions;

public enum OPTION
{
    EXIT,
    ADMIN_MENUE,
    VOLUNTEER_MENUE,
    CALL_MENUE,
  
}
public enum IAdmin
{
    EXIT,
    GET_CLOCK,
    FORWARD_CLOCK,
    GET_MAX_RANGE,
    DEFENIATION,
    RESET,
    INITIALIZATION,
}
public enum IVolunteer
{
    EXIT,
    ENTER_SYSTEM,
    GET_VOlUNTEERLIST,
    READ,
    UPDATE,
    DELETE,
    CREATE,
}
public enum ICall
{
    EXIT,
    COUNT_CALL,
    GET_CALLINLIST,
    READ,
    UPDATE,
    DELETE,
    CREATE,
    GET_CLOSED_CALL,
    GET_OPEN_CALL,
    CLOSE_TREAT,
    CANCEL_TREAT,
    CHOSE_TOR_TREAT,
}

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();    //stage 4
    /// <summary>
    /// The function is responsible for displaying the main menu to the user, performing the appropriate actions based on their selection, 
    /// and handling any exceptions that may occur during execution.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        try //If there are any exceptions
        {
            OPTION option = showMainMenu();
            while (OPTION.EXIT != option)  //As long as you haven't chosen an exit
            {
                switch (option)
                {
                    case OPTION.ADMIN_MENUE:
                        handleAdminOptions();
                        break;
                    case OPTION.VOLUNTEER_MENUE:
                        handleVolunteerOptions();
                        break;
                    case OPTION.CALL_MENUE:
                        handleCallOptions();
                        break;
                }
                option = showMainMenu();
            }
        }
        catch (Exception ex)   //If any anomaly is detected
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// In summary, the function displays a menu to the user, receives input from the user,
    /// and returns the selection as a value from the `OPTION` enum.
    /// </summary>
    /// <returns></returns>
    private static OPTION showMainMenu()
    {
        int choice;
        do
        {
            Console.WriteLine
                (@"OPTION Options:
0 - Exit
1 - Admin
2 - Volunteer
3 - Call");


        }
        while (!int.TryParse(Console.ReadLine(), out choice));
        return (OPTION)choice;
    }

    /// <summary>
    /// the function displays a menu for the admin, receives input from the user, and returns the selection as a value from the `IAdmin` interface.
    /// </summary>
    /// <returns> IAdmin </returns>
    private static IAdmin showAdminMenu()
    {
        int choice;
        do
        {
            Console.WriteLine(@$"
Admin Options:
0 - Exit
1-  get clock
2 - Forward Clock 
3 - GetMaxRange
4 - Definition
5 - Reset
6 - initialization");

        }
        while (!int.TryParse(s: Console.ReadLine(), out choice));
        return (IAdmin)choice;
    }

    /// <summary>
    /// The function manages admin tasks based on the user's selection from a menu, handling various admin operations like getting the clock, 
    /// advancing the clock, resetting, and initialization. It also includes error handling for invalid user inputs.
    /// </summary>
    private static void handleAdminOptions()
    {
        try
        {
            switch (showAdminMenu())
            {
                case IAdmin.GET_CLOCK:
                    Console.WriteLine(s_bl.Admin.GetClock());
                    break;
                case IAdmin.FORWARD_CLOCK:
                    // Prompt the user to enter a time unit (e.g., MINUTE, HOUR, DAY, etc.)
                    Console.WriteLine("Enter unit time: MINUTE, HOUR, DAY, MONTH, YEAR");
                    string inputClock = Console.ReadLine()?.ToUpper(); // Convert input to uppercase for consistency

                    // Try to parse the user input into the BO.TimeUnit enum
                    if (!Enum.TryParse(typeof(BO.TimeUnit), inputClock, out object unitObj) || unitObj is not BO.TimeUnit unit)
                    {
                        // If parsing fails or the input is not valid, throw an exception
                        throw new BO.BlWrongInputException("Wrong input. Please enter a valid time unit.");
                    }

                    // Call the ForwardClock method with the parsed time unit
                    s_bl.Admin.ForwardClock(unit);
                    break;
                case IAdmin.GET_MAX_RANGE:
                    Console.WriteLine(s_bl.Admin.GetMaxRange());
                    break;
                case IAdmin.DEFENIATION:
                    Console.WriteLine("Enter time span in the format (days:hours:minutes:seconds):");
                    string inputTimeSpan = Console.ReadLine();

                    if (!TimeSpan.TryParse(inputTimeSpan, out TimeSpan time))
                    {
                        throw new BO.BlWrongInputException("Invalid input. Please enter a valid time span.");
                    }

                    s_bl.Admin.setMaxRange(time);
                    break;
                case IAdmin.RESET:
                    s_bl.Admin.Reset();
                    break;
                case IAdmin.INITIALIZATION:
                    s_bl.Admin.initialization();
                    break;
                case IAdmin.EXIT:
                    break;
                default:
                    Console.WriteLine("Invalid option selected.");
                    break;
            }
        }
        catch (BO.BlWrongInputException ex)
        {

            Console.WriteLine($"Error: {ex.Message}");
        }

    }

    /// <summary>
    /// The `showVolunteerMenu` function displays a menu with volunteer options and receives the user's selection. 
    /// It returns the chosen option as a value from the `IVolunteer` interface.
    /// </summary>
    /// <returns> IVolunteer </returns>
    private static IVolunteer showVolunteerMenu()
    {
        int choice;
        do
        {
            Console.WriteLine(@"
Volunteer Options:
0 - Exit
1 - EnterSystem
2 - Get volunteerInList
3 - Read
4 - Update
5 - Delete
6 - Create");

        }
        while (!int.TryParse(Console.ReadLine(), out choice));
        return (IVolunteer)choice;
    }

    /// <summary>
    /// The function manages volunteer-related tasks based on the user's menu selection, including actions like logging in, viewing, updating, deleting, 
    /// or creating volunteer records. It also handles errors from invalid inputs or system issues.
    /// </summary>
    private static void handleVolunteerOptions()
    {
        try
        {
            switch (showVolunteerMenu())
            {
                case IVolunteer.ENTER_SYSTEM:

                    Console.WriteLine("Please enter your username:");
                    if (!int.TryParse(Console.ReadLine(), out int username))
                    {
                        throw new BO.BlWrongInputException("Invalid username. It must be a numeric value.");
                    }

                    Console.WriteLine("Please enter your password:");
                    string password = Console.ReadLine();

                    // Call the EnterSystem method to verify credentials and get the user's role
                    BO.Role role = s_bl.Volunteers.EnterSystem(username, password);
                    Console.WriteLine($"Welcome to the system! Your role is: {role}");
                    break;
                case IVolunteer.GET_VOlUNTEERLIST:

                    Console.WriteLine("Would you like to filter by active status? (true/false/null):");
                    string activeInput = Console.ReadLine();

                    bool? activeFilter = activeInput.ToLower() == "true" ? true
                                        : activeInput.ToLower() == "false" ? false
                                        : (bool?)null;

                    Console.WriteLine("Please select a sort option (Id, FullName, Active, SumCalls, SumCanceled, SumExpired, IdCall, CType):");
                    string sortByInput = Console.ReadLine();

                    BO.EVolunteerInList? sortBy = Enum.TryParse<BO.EVolunteerInList>(sortByInput, true, out var sortOption)
                        ? sortOption : null;


                    IEnumerable<BO.VolunteerInList> volunteerList = s_bl.Volunteers.GetVolunteerList(activeFilter, sortBy);


                    Console.WriteLine("Volunteer List:");
                    foreach (var Volunteer in volunteerList)
                    {
                        Console.WriteLine(Volunteer.ToString());
                    }
                    break;
                case IVolunteer.READ:
                    Console.WriteLine("Please enter the ID of the volunteer:");
                    string idInput = Console.ReadLine();  // לוקחים את הקלט מהמשתמש

                    if (!int.TryParse(idInput, out int id))  // מנסים להמיר את הקלט למספר
                    {
                        throw new BO.BlWrongInputException($"Invalid ID{idInput} format");  // זורקים חריגה אם המזהה לא תקני
                    }

                    BO.Volunteer volunteer = s_bl.Volunteers.Read(id);
                    Console.WriteLine("Volunteer Details:");
                    Console.WriteLine(volunteer);
                    break;
                case IVolunteer.UPDATE:

                    Console.WriteLine("Please enter the ID of the volunteer you want to update:");
                    int idToUpdate;
                    if (!int.TryParse(Console.ReadLine(), out idToUpdate))
                    {
                        throw new BO.BlWrongInputException("Invalid ID. Please enter a valid number.");
                    }
                    Console.WriteLine("Please enter the your ID");
                    int idPersson;
                    if (!int.TryParse(Console.ReadLine(), out idPersson))
                    {
                        throw new BO.BlWrongInputException("Invalid ID. Please enter a valid number.");
                    }

                    // Try to retrieve the volunteer by ID


                    Console.WriteLine("Updating details for volunteer:");
                    //  Console.WriteLine(volunteerToUpdate);

                    // Full Name
                    Console.WriteLine("Full Name :");
                    string fullName = Console.ReadLine();
                    if (string.IsNullOrEmpty(fullName))
                    {
                        throw new BO.BlWrongInputException("Invalid fullName. Please enter a valid number.");
                    }
                    // Phone Number

                    Console.WriteLine("Phone Number :");
                    string phoneNumber = Console.ReadLine();

                    if (string.IsNullOrEmpty(phoneNumber))
                    {
                        throw new BO.BlWrongInputException("Invalid phoneNumber. Please enter a valid number.");
                    }

                    // Email
                    Console.WriteLine("Email:");
                    string email = Console.ReadLine();

                    // Distance Type
                    Console.WriteLine("Distance Type (Aerial, Walking, Driving) for Arial leav empty:");
                    string distanceTypeInput = Console.ReadLine();
                    //if (!string.IsNullOrEmpty(distanceTypeInput))
                    //{
                        BO.Distance distanceTypeUpdate;
                        if (!Enum.TryParse(distanceTypeInput, true, out distanceTypeUpdate))
                        {
                             distanceTypeUpdate=BO.Distance.Aerial;
                        }
                     
                    //}

                    // Role
                    Console.WriteLine("Role (Volunteer, Boss):");
                    //string roleInput = Console.ReadLine();
                    //if (!string.IsNullOrEmpty(roleInput))
                    //{
                        BO.Role roleUpdate;
                        if (!Enum.TryParse(Console.ReadLine(), true, out roleUpdate))
                        { 
                            throw new BO.BlWrongInputException("Invalid role.");
                        }
                    //}

                    // Active
                    Console.WriteLine("Active (true/false): defult is true");
                    
                        bool activeUP;
                        if (!bool.TryParse(Console.ReadLine(), out activeUP))
                         {
                             activeUP=true;
                        }
                    

                    // Password
                    Console.WriteLine("Password :");
                    string passwordNew = Console.ReadLine();


                    // Full Address
                    Console.WriteLine("Full Address:");
                    string fullAddress = Console.ReadLine();
                    if (string.IsNullOrEmpty(fullAddress))
                    {
                         throw new BO.BlWrongInputException("the addres not valid");
                    }



                    // Max Reading
                    Console.WriteLine("Max Reading:");
                   
                        int maxReadingUP;
                        if (!int.TryParse(Console.ReadLine(), out maxReadingUP))
                         { 
                            throw new BO.BlWrongInputException("Invalid input for Max Reading. Keeping current value.");
                        }
                    
                    BO.Volunteer volunteerToUpdate = new BO.Volunteer
                    {
                        Id = idToUpdate,
                        FullName = fullName,
                        PhoneNumber = phoneNumber,
                        Email = email,
                        TypeDistance = distanceTypeUpdate,
                        Job = roleUpdate,
                        Active = activeUP,
                        Password = passwordNew,
                        FullAddress = fullAddress,
                        Latitude = 0,
                        Longitude = 0,
                        MaxReading = maxReadingUP,
                        SumCalls = 0,
                        SumCanceled = 0,
                        SumExpired = 0,
                        CallIn = null,
                    };


                    // Update the volunteer in the system

                    s_bl.Volunteers.Update(idPersson, volunteerToUpdate);
                    break;
                case IVolunteer.DELETE:
                    Console.WriteLine("Volunteer id:");
                    string idInputDelete = Console.ReadLine();
                    if (!int.TryParse(idInputDelete, out int idDelete))
                    {
                        throw new BO.BlWrongInputException($"Invalid ID{idInputDelete} format");
                    }
                    s_bl.Volunteers.Delete(idDelete);
                    break;
                case IVolunteer.CREATE:
                    {

                        Console.WriteLine("Please enter volunteer details:");
                        Console.WriteLine("Please enter the ID :");
                        string idCreat = Console.ReadLine();  // לוקחים את הקלט מהמשתמש

                        if (!int.TryParse(idCreat, out int idC))  // מנסים להמיר את הקלט למספר
                        {
                            throw new BO.BlWrongInputException($"Invalid ID{idCreat} format");  // זורקים חריגה אם המזהה לא תקני
                        }
                        // Full Name
                        Console.WriteLine("Full Name:");
                        string fullNameUp = Console.ReadLine();

                        // Phone Number
                        Console.WriteLine("Phone Number:");
                        string phoneNumberUp = Console.ReadLine();

                        // Email
                        Console.WriteLine("Email:");
                        string emailUp = Console.ReadLine();

                        // Distance Type
                        Console.WriteLine("Distance Type (Aerial, Walking, Driving):");
                        string distanceTypeInputUp = Console.ReadLine();
                        BO.Distance distanceType;
                        if (!Enum.TryParse(distanceTypeInputUp, true, out distanceType))
                        {
                            throw new BO.BlWrongInputException("Invalid distance type. Defaulting to Aerial.");

                        }

                        // Role
                        Console.WriteLine("Role (Volunteer, Boss):");
                        string roleUp = Console.ReadLine();
                        BO.Role roleup;
                        if (!Enum.TryParse(roleUp, true, out roleup))
                        {
                            throw new BO.BlWrongInputException("Invalid role. Defaulting to Volunteer.");

                        }

                        // Active
                        Console.WriteLine("Active (true/false):");
                        bool active;
                        if (!bool.TryParse(Console.ReadLine(), out active))
                        {
                            throw new BO.BlWrongInputException("Invalid input for Active. Defaulting to false.");
                        }

                        // Password
                        Console.WriteLine("Password:");
                        string passwordUp = Console.ReadLine();

                        // Full Address
                        Console.WriteLine("Full Address:");
                        string fullAddressUp = Console.ReadLine();

                        // Max Reading
                        Console.WriteLine("Max Reading:");
                        int maxReading;
                        if (!int.TryParse(Console.ReadLine(), out maxReading))
                        {
                            throw new BO.BlWrongInputException("Invalid input for Max Reading. Defaulting to 0.");

                        }

                        // Create the new Volunteer object
                        BO.Volunteer newVolunteer = new BO.Volunteer
                        {
                            Id = idC,
                            FullName = fullNameUp,
                            PhoneNumber = phoneNumberUp,
                            Email = emailUp,
                            TypeDistance = distanceType,
                            Job = roleup,
                            Active = active,
                            Password = passwordUp,
                            FullAddress = fullAddressUp,
                            Latitude = 0,
                            Longitude = 0,
                            MaxReading = maxReading,
                            SumCalls = 0,
                            SumCanceled = 0,
                            SumExpired = 0,
                            CallIn = null,
                        };

                        // Call the Create method
                        s_bl.Volunteers.Create(newVolunteer);
                        break;
                        
                    }
            }
        }
        catch (BO.BlNullPropertyException ex)
        {
            // Handle the case where the volunteer does not exist
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (BO.BlWrongInputException ex)
        {
            // Handle the case where the password does not match
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (BO.BlWrongItemException ex)
        {
            // Handle the case where the password does not match
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Handle any other unexpected errors
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }

    }

    /// <summary>
    /// The `showCallMenu` function displays a menu with call-related options and receives the user's selection.
    /// It returns the chosen option as a value from the `ICall` interface.
    /// </summary>
    /// <returns> ICall </returns>
    private static ICall showCallMenu()
    {
        int choice;
        do
        {
            Console.WriteLine(@"
Call Options:
0 - Exit
1 - CountCall
2 - Get CallInLists
3 - Read
4 - update
5 - Delete
6 - Create
7 - Get ClosedCall
8 - Get OpenCall
9 - CloseTreat
10 - CancelTreat
11- ChoseForTreat");

        }
        while (!int.TryParse(Console.ReadLine(), out choice));
        return (ICall)choice;
    }

    /// <summary>
    /// The `handleCallOptions` method handles different call-related actions based on user input, such as reading, updating, creating, and deleting calls. 
    /// It includes error handling for invalid inputs and exceptions during the process.
    /// </summary>
    private static void handleCallOptions()
    {
        try
        {
            switch (showCallMenu())
            {
                case ICall.COUNT_CALL:

                    int[] counts = s_bl.Calls.CountCall();

                    // Gets all the status names from the Enum
                    string[] statusNames = Enum.GetNames(typeof(BO.StatusTreat));

                    // Loop to print status names with their counts
                    for (int i = 0; i < statusNames.Length; i++)
                    {
                        Console.WriteLine($"{statusNames[i]}: {counts[i]}");
                    }


                    break;
                case ICall.GET_CALLINLIST:
                    Console.Write("Enter filter field: ");
                    string? filterInput = Console.ReadLine();

                    // Try to parse the filter input
                    BO.ECallInList? filter = null;
                    if (!string.IsNullOrEmpty(filterInput) && Enum.TryParse(filterInput, out BO.ECallInList parsedFilter))
                    {
                        filter = parsedFilter;
                    }
                    Console.Write("Enter value for the filter field (or leave blank for no value): ");
                    string? filterValueInput = Console.ReadLine();
                    object? filterValue = string.IsNullOrEmpty(filterValueInput) ? null : filterValueInput;

                    Console.WriteLine("Please choose a sorting field from the following list (or leave blank for default sorting):");
                    foreach (var field in Enum.GetNames(typeof(BO.ECallInList)))
                    {
                        Console.WriteLine($"- {field}");
                    }
                    Console.Write("Enter sorting field: ");
                    string? sortInput = Console.ReadLine();

                    // Try to parse the sorting input
                    BO.ECallInList? sortBy = null;
                    if (!string.IsNullOrEmpty(sortInput) && Enum.TryParse(sortInput, out BO.ECallInList parsedSort))
                    {
                        sortBy = parsedSort;
                    }

                    // Step 4: Call the method and display the results
                    var callInLists = s_bl.Calls.GetCallInLists(filter, filterValue, sortBy);

                    Console.WriteLine("Filtered and sorted call-in list:");
                    foreach (var call in callInLists)
                    {
                        Console.WriteLine(call); // Assuming BO.CallInList has a meaningful ToString() implementation
                    }
                    break;
                case ICall.READ:
                    Console.WriteLine("Please enter the ID of the call:");
                    string idInput = Console.ReadLine();  // Taking the input from the user

                    if (!int.TryParse(idInput, out int id))  // Trying to convert the input to a number
                    {
                        throw new BO.BlWrongInputException($"Invalid ID{idInput} format");  // Throwing an exception if the ID format is invalid
                    }
                    Console.WriteLine(s_bl.Calls.Read(id));
                    break;
                case ICall.UPDATE:
                    s_bl.Calls.Update(getCall());
                    break;
                case ICall.DELETE:
                    Console.WriteLine("Please enter the ID of the call:");
                    string idDel = Console.ReadLine();  // Taking the input from the user

                    if (!int.TryParse(idDel, out int idd))  // Trying to convert the input to a number
                    {
                        throw new BO.BlWrongInputException($"Invalid ID{idDel} format");  // Throwing an exception if the ID format is invalid
                    }
                    s_bl.Calls.Delete(idd);
                    break;
                case ICall.CREATE:
                    {
                        //s_bl.Calls.Create(getCall());
                        Console.WriteLine("Enter call type (Puncture, Cables, LockedCar):");
                        string callTypeInput = Console.ReadLine();
                        if (!Enum.TryParse(callTypeInput, true, out BO.CallType callType) || !Enum.IsDefined(typeof(BO.CallType), callType))
                        {
                            throw new BO.BlWrongInputException("Invalid input. Please enter a valid call type (Puncture, Cables, LockedCar):");
                        }

                        Console.WriteLine("Enter description:");
                        string description = Console.ReadLine();

                        Console.WriteLine("Enter full address:");
                        string fullAddress = Console.ReadLine();

                        DateTime timeOpened;
                        Console.WriteLine("Enter time opened (YYYY-MM-DD HH:mm:ss):");
                        if (!DateTime.TryParse(Console.ReadLine(), out timeOpened))
                        {
                            throw new BO.BlWrongInputException("Invalid input. Please enter a valid date and time (YYYY-MM-DD HH:mm:ss):");
                        }

                        DateTime? maxTimeToClose = null;
                        Console.WriteLine("Enter max time to close (or leave empty):");
                        string maxTimeInput = Console.ReadLine();
                        if (!string.IsNullOrEmpty(maxTimeInput) && !DateTime.TryParse(maxTimeInput, out DateTime parsedMaxTime))
                        {
                            throw new BO.BlWrongInputException("Invalid input. Please enter a valid date and time for max time to close.");
                        }

                        BO.Call callToCreate= new BO.Call
                        {
                            Id = 0,
                            Type = callType,
                            Description = description,
                            FullAddress = fullAddress,
                            Latitude = 0,
                            Longitude = 0,
                            TimeOpened = timeOpened,
                            MaxTimeToClose = maxTimeToClose,
                            Status = 0,
                        };
                        s_bl.Calls.Create(callToCreate);

                    }
                    break;
                case ICall.GET_CLOSED_CALL:
                    {
                        // Ask the user for the volunteer's ID
                        Console.Write("Enter volunteer ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                        {
                            throw new BO.BlWrongInputException("Invalid ID format.");
                        }

                        // Function for receiving filter type
                        static BO.CallType? filterClose()
                        {
                            BO.CallType? filter = null;
                            Console.WriteLine("Do you want to filter the list? (1 for Yes, 0 for No): ");
                            if (int.TryParse(Console.ReadLine(), out int sortInput) && sortInput == 1)
                            {
                                // Sorting menu by FieldsOpenCallInList
                                Console.WriteLine("\nChoose a field to filter by:");
                                Console.WriteLine("1. Cables");
                                Console.WriteLine("2. Puncture");
                                Console.WriteLine("3. LockedCar");

                                // Receiving the user's choice
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        filter = BO.CallType.Cables;
                                        break;
                                    case "2":
                                        filter = BO.CallType.Puncture;
                                        break;
                                    case "3":
                                        filter = BO.CallType.LockedCar;
                                        break;
                                    default:
                                        Console.WriteLine("Invalid input. No filtering applied.");
                                        break;
                                }

                                // Return the selected value or null
                            }
                            return filter;
                        }


                        // Function for receiving sort type
                        static BO.EClosedCallInList? SortClose()
                        {
                            BO.EClosedCallInList? sortField = null;
                            Console.WriteLine("Do you want to sort the list? (1 for Yes, 0 for No): ");
                            if (int.TryParse(Console.ReadLine(), out int sortInput) && sortInput == 1)
                            {
                                // Sorting menu by FieldsOpenCallInList
                                Console.WriteLine("\nChoose a field to sort by:");
                                Console.WriteLine("1. Id");
                                Console.WriteLine("2. Type Of The Call");
                                Console.WriteLine("3. Address");
                                Console.WriteLine("4. Opening Time");
                                Console.WriteLine("5. Start Treat");
                                Console.WriteLine("6. Max Time To Close");
                                Console.WriteLine("7. Type End Treat");

                                // Receiving the user's choice
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        sortField = BO.EClosedCallInList.Id;
                                        break;
                                    case "2":
                                        sortField = BO.EClosedCallInList.CType;
                                        break;
                                    case "3":
                                        sortField = BO.EClosedCallInList.FullAddress;
                                        break;
                                    case "4":
                                        sortField = BO.EClosedCallInList.TimeOpen;
                                        break;
                                    case "5":
                                        sortField = BO.EClosedCallInList.StartTreat;
                                        break;
                                    case "6":
                                        sortField = BO.EClosedCallInList.TimeClose;
                                        break;
                                    case "7":
                                        sortField = BO.EClosedCallInList.TypeEndTreat;
                                        break;
                                    default:
                                        Console.WriteLine("Invalid input. No sorting applied.");
                                        break;
                                }

                                // Return the selected value or null
                            }
                            return sortField;
                        }

                        BO.CallType? filterToClose = filterClose();
                        BO.EClosedCallInList? sortToClose = SortClose();
                        var openCallList = s_bl.Calls.GetClosedCall(volunteerId, filterToClose, sortToClose);
                        Console.WriteLine("Closed Calls of the volunteer: ");
                        foreach (var call in openCallList)
                            Console.WriteLine(call);
                        break;
                    }
                case ICall.GET_OPEN_CALL:
                    {
                        // Ask the user for the volunteer's ID
                        Console.Write("Enter volunteer ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int volunteerID))
                        {
                            throw new BO.BlWrongInputException("Invalid ID format.");
                        }

                        // Function for receiving filter type
                        static BO.CallType? filterOpen()
                        {
                            BO.CallType? filter = null;
                            Console.WriteLine("Do you want to filter the list? (1 for Yes, 0 for No): ");
                            if (int.TryParse(Console.ReadLine(), out int sortInput) && sortInput == 1)
                            {
                                // Sorting menu by FieldsOpenCallInList
                                Console.WriteLine("\nChoose a field to filter by:");
                                Console.WriteLine("1. Cables");
                                Console.WriteLine("2. Puncture");
                                Console.WriteLine("3. LockedCar");

                                // Receiving the user's choice
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        filter = BO.CallType.Cables;
                                        break;
                                    case "2":
                                        filter = BO.CallType.Puncture;
                                        break;
                                    case "3":
                                        filter = BO.CallType.LockedCar;
                                        break;
                                    default:
                                        Console.WriteLine("Invalid input. No filtering applied.");
                                        break;
                                }

                                // Return the selected value or null
                            }
                            return filter;
                        }

                        // Function for receiving sort type
                        static BO.EOpenCallInList? SortOpen()
                        {
                            BO.EOpenCallInList? sortField = null;
                            Console.WriteLine("Do you want to sort the list? (1 for Yes, 0 for No): ");
                            if (int.TryParse(Console.ReadLine(), out int sortInput) && sortInput == 1)
                            {
                                // Sorting menu by FieldsOpenCallInList
                                Console.WriteLine("\nChoose a field to sort by:");
                                Console.WriteLine("1. Id");
                                Console.WriteLine("2. Call Type");
                                Console.WriteLine("3. Verbal Description");
                                Console.WriteLine("4. Address");
                                Console.WriteLine("5. Opening Time");
                                Console.WriteLine("6. Max Time To End");
                                Console.WriteLine("7. Distance");

                                // Receiving the user's choice
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        sortField = BO.EOpenCallInList.Id;
                                        break;
                                    case "2":
                                        sortField = BO.EOpenCallInList.CType;
                                        break;
                                    //case "3":
                                    //    sortField = BO.EOpenCallInList.Description;
                                    //    break;
                                    case "3":
                                        sortField = BO.EOpenCallInList.FullAddress;
                                        break;
                                    case "4":
                                        sortField = BO.EOpenCallInList.TimeOpen;
                                        break;
                                    case "5":
                                        sortField = BO.EOpenCallInList.MaxTimeToClose;
                                        break;
                                    case "6":
                                        sortField = BO.EOpenCallInList.distanceCallVolunteer;
                                        break;
                                    default:
                                        Console.WriteLine("Invalid input. No sorting applied.");
                                        break;
                                }

                                // Return the selected value or null
                            }
                            return sortField;
                        }

                        BO.CallType? filterToOpen = filterOpen();
                        BO.EOpenCallInList? sortToOpen = SortOpen();
                        var openCallList = s_bl.Calls.GetOpenCall(volunteerID, filterToOpen, sortToOpen);
                        Console.WriteLine("Call Open list for volunteer: ");
                        foreach (var call in openCallList)
                            Console.WriteLine(call);
                        break;
                    }
                case ICall.CLOSE_TREAT:
                    {
                        // Requesting the user to enter the values
                        Console.WriteLine("Enter the volunteer ID:");
                        string volInput = Console.ReadLine();
                        Console.WriteLine("Enter the assignment ID:");
                        string assigInput = Console.ReadLine();

                        // Variables to store the values entered by the user
                        int idVol, idAssig;

                        // Checking if it's possible to parse the input into integers
                        if (int.TryParse(volInput, out idVol) && int.TryParse(assigInput, out idAssig))
                        {
                            // If the parsing succeeded, call the CloseTreat function
                            s_bl.Calls.CloseTreat(idVol, idAssig);
                        }
                        else
                        {
                            // If parsing failed, display an error message
                            throw new BO.BlWrongInputException("Invalid input. Please ensure the IDs are numbers.");
                        }
                        break;
                    }
                case ICall.CANCEL_TREAT:
                    {
                        // Requesting the user to enter the values
                        Console.WriteLine("Enter the volunteer ID:");
                        string volInput = Console.ReadLine();
                        Console.WriteLine("Enter the assigmnent ID:");
                        string assigInput = Console.ReadLine();

                        // Variables to store the values entered by the user
                        int idVol, idAssig;

                        // Checking if it's possible to parse the input into integers
                        if (int.TryParse(volInput, out idVol) && int.TryParse(assigInput, out idAssig))
                        {
                            // If the parsing succeeded, call the CloseTreat function
                            s_bl.Calls.CancelTreat(idVol, idAssig);
                        }
                        else
                        {
                            // If parsing failed, display an error message
                            throw new BO.BlWrongInputException("Invalid input. Please ensure the IDs are numbers.");
                        }
                        break;
                    }
                case ICall.CHOSE_TOR_TREAT:
                    {
                        // Requesting the user to enter the values
                        Console.WriteLine("Enter the volunteer ID:");
                        string volInput = Console.ReadLine();
                        Console.WriteLine("Enter the call ID:");
                        string assigInput = Console.ReadLine();

                        // Variables to store the values entered by the user
                        int idVol, idAssig;

                        // Checking if it's possible to parse the input into integers
                        if (int.TryParse(volInput, out idVol) && int.TryParse(assigInput, out idAssig))
                        {
                            // If the parsing succeeded, call the CloseTreat function
                            s_bl.Calls.ChoseForTreat(idVol, idAssig);
                        }
                        else
                        {
                            // If parsing failed, display an error message
                            throw new BO.BlWrongInputException("Invalid input. Please ensure the IDs are numbers.");
                        }
                        break;
                    }
                default: break;
            }
        }
        catch (BO.BlWrongInputException ex)
        {
            // Handle the case where the password does not match
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (BO.BlWrongItemException ex)
        {
            // Handle the case where the password does not match
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Handle any other unexpected errors
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }

    }

    /// <summary>
    /// The function prompts the user for input in the console and creates a BO.Call object based on the provided details.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BO.BlWrongInputException"></exception>
    private static BO.Call getCall()
    {
        int id;
        Console.WriteLine("Enter call ID:");
        if (!int.TryParse(Console.ReadLine(), out id))
        {
            throw new BO.BlWrongInputException("Invalid input. Please enter a valid integer for the ID:");
        }
        Console.WriteLine("Enter call type (Puncture, Cables, LockedCar):");
        string callTypeInput = Console.ReadLine();
        if (!Enum.TryParse(callTypeInput, true, out BO.CallType callType) || !Enum.IsDefined(typeof(BO.CallType), callType))
        {
            throw new BO.BlWrongInputException("Invalid input. Please enter a valid call type (Puncture, Cables, LockedCar):");
        }

        Console.WriteLine("Enter description:");
        string description = Console.ReadLine();

        Console.WriteLine("Enter full address:");
        string fullAddress = Console.ReadLine();

        DateTime timeOpened;
        Console.WriteLine("Enter time opened (YYYY-MM-DD HH:mm:ss):");
        if (!DateTime.TryParse(Console.ReadLine(), out timeOpened))
        {
            throw new BO.BlWrongInputException("Invalid input. Please enter a valid date and time (YYYY-MM-DD HH:mm:ss):");
        }

        DateTime? maxTimeToClose = null;
        Console.WriteLine("Enter max time to close (or leave empty):");
        string maxTimeInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(maxTimeInput) && !DateTime.TryParse(maxTimeInput, out DateTime parsedMaxTime))
        {
            throw new BO.BlWrongInputException("Invalid input. Please enter a valid date and time for max time to close.");
        }

        return new BO.Call
        {
            Id = id,
            Type = callType,
            Description = description,
            FullAddress = fullAddress,
            Latitude = 0,
            Longitude = 0,
            TimeOpened = timeOpened,
            MaxTimeToClose = maxTimeToClose,
            Status = 0
        };
    }
}





