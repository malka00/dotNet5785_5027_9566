using static System.Formats.Asn1.AsnWriter;

namespace DalTest;


using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;
/// <summary>
/// options to choose from in the menu
/// </summary>
public enum OPTION   
{
    EXIT,
    VOLUNTEER,
    CALL,
    ASSIGNMENT,
    INIT_DB,
    SHOW_ALL_DB,
    CONFIG_MENU,
    RESET_DB
}
/// <summary>
/// Optional actions
/// </summary>
public enum CRUD
{
    EXIT,
    CREATE,
    READ,
    READ_ALL,
    UPDATE,
    DELETE,
    DELETE_ALL
}
/// <summary>
/// Actions on the watch
/// </summary>
public enum CONFIG
{
    EXIT,
    FORWARD_CLOCK_ONE_MINUTE,
    FORWARD_CLOCK_ONE_HOUR,
    FORWARD_CLOCK_ONE_DAY,
    FORWARD_CLOCK_ONE_MONTH,
    FORWARD_CLOCK_ONE_YEAR,
    GET_CLOCK,
    UPDATE,
    GET_MAX_RANGE,
    RESET_CONFIG,
}

/// <summary>
/// Clock update operations
/// </summary>
public enum CLOCKCHOICE
{ 
    CLOCK,
    RISK_RANGE,
}

internal class Program
{
    //private static IVolunteer? s_dal.Volunteer = new VolunteerImplementation(); //stage 1
    //private static ICall? s_dal.Call = new CallImplementation(); //stage 1
    //private static IAssignment? s_dal.Assignment = new AssignmentImplementation(); //stage 1
    //private static IConfig? s_dal = new ConfigImplementation(); //stage 1

    //static readonly IDal s_dal = new DalList(); //stage 2
    //static readonly IDal s_dal = new DalXml(); //stage 3
    static readonly IDal s_dal = Factory.Get; //stage 4

    /// <summary>
    /// main program
    /// </summary>
    static void Main(string[] args)
    {
        try //If there are any exceptions
        {
            OPTION option = showMainMenu();
            while (OPTION.EXIT != option)  //As long as you haven't chosen an exit
            {
                switch (option)
                {
                    case OPTION.RESET_DB:
                        //s_dal.Volunteer.DeleteAll(); //stage 1
                        //s_dal.Call.DeleteAll(); //stage 1 
                        //s_dal.Assignment.DeleteAll();//stage 1 
                        //s_dal.Reset(); //stage 1
                        s_dal.Config.Reset();
                        break;
                    case OPTION.INIT_DB:  //initialize the variables
                                          //Initialization.Do(s_dal.Volunteer, s_dal.Call, s_dal.Assignment, s_dal);
                        //Initialization.Do(s_dal); //stage 2
                            Initialization.Do(); //stage 4

                        break;
                    case OPTION.CONFIG_MENU:   //Clock options
                        handleConfigOptions();
                        break;
                    case OPTION.SHOW_ALL_DB:  //Viewing the databases
                        showAllDB();
                        break;
                    default:
                        handleCRUDOptions(option);  //Optional actions
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
    /// Optional actions
    /// </summary>
    private static void handleCRUDOptions(OPTION entity)    
    {
        try
        {
            switch (showCrudMenu(entity))
            {
                case CRUD.CREATE:
                    handleCreate(entity);
                    break;
                case CRUD.READ:
                    handleRead(entity);
                    break;
                case CRUD.READ_ALL:
                    handleReadAll(entity);
                    break;
                case CRUD.UPDATE:
                    handleUpdate(entity);
                    break;
                case CRUD.DELETE:
                    handleDelete(entity);
                    break;
                case CRUD.DELETE_ALL:
                    handleDeleteAll(entity);
                    break;

                default:
                    return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// clock operations
    /// </summary>
    private static void handleConfigOptions()
    {
        try
        {
            switch (showConfigMenu())
            {
                case CONFIG.FORWARD_CLOCK_ONE_MINUTE:   //Added a minute
                    {
                        s_dal.Config.Clock = new (s_dal.Config.Clock.Minute+1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_HOUR:     //Add an hour
                    {
                        s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_DAY:    //Add an day
                    {
                        s_dal.Config.Clock = s_dal.Config.Clock.AddDays(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_MONTH:    //Add an month
                    {
                        s_dal.Config.Clock = s_dal.Config.Clock.AddMonths(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_YEAR:     //Add an year
                    {
                        s_dal.Config.Clock = s_dal.Config.Clock.AddYears(1);
                        break;
                    }
                case CONFIG.GET_CLOCK:      //show clock
                    {
                        Console.WriteLine(s_dal.Config.Clock);
                        break;
                    }
                case CONFIG.UPDATE:       //update the clock
                    {
                        Console.WriteLine(@"Press 0 for clock id,
                        Press 1 for risk range\n");
                        CLOCKCHOICE c = (CLOCKCHOICE)Console.Read();
                        Console.WriteLine("Write update value:\n");
                        switch (c)
                        {
                            case CLOCKCHOICE.CLOCK:
                              string newClock= Console.ReadLine();
                                if (!DateTime.TryParse(newClock, out DateTime dateTimeValue))
                                    throw new DalWrongInput("Wrong input");
                                s_dal.Config.Clock = dateTimeValue;
                                break;
                            case CLOCKCHOICE.RISK_RANGE:
                                Console.Write("enter Range: ");
                                if (!int.TryParse(Console.ReadLine(), out int maxRange))
                                    throw new DalWrongInput("Wrong input");
                                int newRisk = Console.Read();
                                s_dal.Config.RiskRange = new(newRisk);
                                break;
                            default:
                                throw new DalWrongInput("Wrong input");
                        }
                        break;
                    }
                case CONFIG.GET_MAX_RANGE:     //get risk range
                    {
                        Console.WriteLine(s_dal.Config.RiskRange);
                        break;
                    }
                case CONFIG.RESET_CONFIG:     //reset the clock
                    s_dal.Config.Reset();
                    break;
                default:
                    return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    /// <summary>
    /// //all the options
    /// </summary>
    private static OPTION showMainMenu()    
    {
        int choice;
        do
        {
            Console.WriteLine(@"
OPTION Options:
0 - Exit
1 - Volunteer
2 - Call
3 - Assignment
4 - Init DB
5 - Show all database
6 - Config Menu
7 - Reset DB & Config");
        }
        while (!int.TryParse(Console.ReadLine(), out choice));
        return (OPTION)choice;
    }
    private static CRUD showCrudMenu(OPTION entity)
    {
        int choice;
        do
        {
            Console.WriteLine(@$"
{entity} CRUD Options:
0 - Exit
1 - Create 
2 - Read
3 - ReadAll
4 - Update 
5 - Delete
6 - Delete All");
        }
        while (!int.TryParse(s: Console.ReadLine(), out choice));
        return (CRUD)choice;
    }

    private static CONFIG showConfigMenu()
    {
        int choice;
        do
        {
            Console.WriteLine(@$"
Config Options:
0 - Exit
1 - Forward Clock One Minute
2 - Forward Clock One Hour
3 - Forward Clock One Day
4 - Forward Clock One Month
5 - Forward Clock One Year
6 - get clock
7 - update
8 - Get riskRange 
9 - ResetConfig Config");
        }
        while (!int.TryParse(s: Console.ReadLine(), out choice));
        return (CONFIG)choice;
    }
    /// <summary>
    /// Creating new calls/tasks/volunteers
    /// </summary>
    private static void handleCreate(OPTION entity)
    {
        switch (entity)
        {
            case OPTION.VOLUNTEER:
                createVolunteer(out Volunteer st);
                s_dal.Volunteer.Create(st);
                break;
            case OPTION.CALL:
                createCall(out Call cr);
                s_dal.Call.Create(cr);
                break;
            case OPTION.ASSIGNMENT:
                createAssigmnet(out Assignment assi);
                s_dal.Assignment.Create(assi);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Reading one object
    /// </summary>
    private static void handleRead(OPTION entity)
    {
        Console.WriteLine("Enter an id");
        if (false == int.TryParse(Console.ReadLine(), out int id))
            Console.WriteLine("Wrong input");

        switch (entity)
        {
            case OPTION.VOLUNTEER:
                Console.WriteLine(s_dal.Volunteer.Read(id));
                break;
            case OPTION.CALL:
                Console.WriteLine(s_dal.Call.Read(id));
                break;
            case OPTION.ASSIGNMENT:
                Console.WriteLine(s_dal.Assignment.Read(id));
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Reading all data
    /// </summary>
    private static void handleReadAll(OPTION entity)
    {
        switch (entity)
        {
            case OPTION.VOLUNTEER:
                foreach (var item in s_dal.Volunteer.ReadAll())
                    Console.WriteLine(item);
                break;
            case OPTION.CALL:
                foreach (var item in s_dal.Call.ReadAll())
                    Console.WriteLine(item);
                break;
            case OPTION.ASSIGNMENT:
                foreach (var item in s_dal.Assignment.ReadAll())
                    Console.WriteLine(item);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// update data
    /// </summary>
    private static void handleUpdate(OPTION entity)
    {
        Console.WriteLine("Enter an id");
        if (false == int.TryParse(Console.ReadLine(), out int id))
            Console.WriteLine("Wrong input");

        switch (entity)
        {
            case OPTION.VOLUNTEER:
                Console.WriteLine(s_dal.Volunteer.Read(id));
                createVolunteer(out Volunteer st, id);
                s_dal.Volunteer.Update(st);
                break;
            case OPTION.CALL:
                Console.WriteLine(s_dal.Call.Read(id));
                createCall(out Call cr, id);
                s_dal.Call.Update(cr);
                break;
            case OPTION.ASSIGNMENT:
                Console.WriteLine(s_dal.Assignment.Read(id));
                createAssigmnet(out Assignment lk, id);
                s_dal.Assignment.Update(lk);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// data deletion 
    /// </summary>
    private static void handleDelete(OPTION entity)
    {
        Console.WriteLine("Enter an id");
        if (false == int.TryParse(Console.ReadLine(), out int id))
            Console.WriteLine("Wrong input");

        switch (entity)
        {
            case OPTION.VOLUNTEER:
                s_dal.Volunteer.Delete(id);
                break;
            case OPTION.CALL:
                s_dal.Call.Delete(id);
                break;
            case OPTION.ASSIGNMENT:
                s_dal.Assignment.Delete(id);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// delete all the data
    /// </summary>
    private static void handleDeleteAll(OPTION entity)
    {
        switch (entity)
        {
            case OPTION.VOLUNTEER:
                s_dal.Volunteer.DeleteAll();
                break;
            case OPTION.CALL:
                s_dal.Call.DeleteAll();
                break;
            case OPTION.ASSIGNMENT:
                s_dal.Assignment.DeleteAll();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// show all data base
    /// </summary>
    private static void showAllDB()    
    {
        Console.WriteLine("--------------- List of Calls ------------------------------------------");
        foreach (var item in s_dal.Call.ReadAll())
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("--------------- List of Volunteers ------------------------------------------");
        foreach (var item in s_dal.Volunteer.ReadAll())
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("--------------- List of Assignments ------------------------------------------");
        foreach (var item in s_dal.Assignment.ReadAll())
        {
            Console.WriteLine(item);
        }
    }
    /// <summary>
    /// create new Assigmnet
    /// </summary>
    private static void createAssigmnet(out Assignment assi, int id = 0)
    {
        Console.Write("enter VolunteerId of the Assignment: ");
        if (!int.TryParse(Console.ReadLine(), out int volId))
            throw new DalWrongInput("Wrong input");

        Console.Write("enter CallId of the Assignment: ");
        if (!int.TryParse(Console.ReadLine(), out int cId))
            throw new DalWrongInput("Wrong input");

        Console.WriteLine("");

        assi = new Assignment(id, volId, cId, s_dal.Config.Clock);
    }

    /// <summary>
    /// create new Call
    /// </summary>
    private static void createCall(out Call cr, int id = 0)
    {
        
        Console.Write("enter Type of the Call:  1.Puncture    2.Cables   3.LockedCar");
        int callTypeInput = int.Parse(Console.ReadLine() ?? "0");
        DO.CallType type= (DO.CallType)callTypeInput;

        Console.Write("enter Description of the Call: ");
        string? description = Console.ReadLine() ?? throw new DalWrongInput("Wrong input");

        Console.Write("enter FullAddress of the Call: ");
        string address= Console.ReadLine() ?? throw new DalWrongInput("Wrong input");
        
        cr = new Call(0, type, description, address, 0,0,s_dal.Config.Clock);
    }

    /// <summary>
    /// create new Volunteer
    /// </summary>
    private static void createVolunteer(out Volunteer st, int id = 0)
    {
        if (id == 0)
        {
            Console.Write("enter volunteer Id: ");
            if (!int.TryParse(Console.ReadLine(), out int _id))
                throw new DalWrongInput("Wrong input");
            else
                id = _id;
        }

        Console.Write("enter Name of the Volunteer: ");
        string? name = Console.ReadLine() ?? throw new DalWrongInput("Wrong input");
           

        Console.WriteLine("Enter Volunteer Phone Number:");
        string? numberPhone = Console.ReadLine() ?? throw new DalWrongInput("Wrong input");

        Console.Write("enter email of the Volunteer: ");
        string? email = Console.ReadLine() ?? throw new DalWrongInput("Wrong input");

        Console.WriteLine("Enter Role (0 = Volunteer, 1 = Manager):");
        Role role = (Role)int.Parse(Console.ReadLine() ?? "0");

        Console.Write("enter true/false if the Volunteer is active: ");
        if (!bool.TryParse(Console.ReadLine(), out bool active))
            throw new DalWrongInput("Wrong input");

        Console.Write("enter address of the Volunteer: ");
        string? address = Console.ReadLine() ?? throw new DalWrongInput("Wrong input");

       
        st = new Volunteer(id, name, numberPhone, email, Distance.Aerial, role, active, address);
    }
}







