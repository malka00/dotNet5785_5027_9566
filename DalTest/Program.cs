using static System.Formats.Asn1.AsnWriter;

namespace DalTest;


using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;

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
public enum CLOCKCHOICE
{ 
    CLOCK,
RISK_RANGE,
}

internal class Program
{
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1

    static void Main(string[] args)
    {
        try
        {
            OPTION option = showMainMenu();
            while (OPTION.EXIT != option)
            {
                switch (option)
                {
                    case OPTION.RESET_DB:
                        s_dalVolunteer.DeleteAll(); //stage 1
                        s_dalCall.DeleteAll(); //stage 1 
                        s_dalAssignment.DeleteAll();//stage 1 
                        s_dalConfig.Reset(); //stage 1
                        break;
                    case OPTION.INIT_DB:
                        Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
                        break;
                    case OPTION.CONFIG_MENU:
                        handleConfigOptions();
                        break;
                    case OPTION.SHOW_ALL_DB:
                        showAllDB();
                        break;
                    default:
                        handleCRUDOptions(option);
                        break;
                }
                option = showMainMenu();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

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


    private static void handleConfigOptions()
    {
        try
        {
            switch (showConfigMenu())
            {
                case CONFIG.FORWARD_CLOCK_ONE_MINUTE:
                    {
                        s_dalConfig.Clock = new (s_dalConfig.Clock.Minute+1);
                        break;
                    }

                case CONFIG.FORWARD_CLOCK_ONE_HOUR:
                    {
                        s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_DAY:
                    {
                        s_dalConfig.Clock = s_dalConfig.Clock.AddDays(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_MONTH:
                    {
                        s_dalConfig.Clock = s_dalConfig.Clock.AddMonths(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_YEAR:
                    {
                        s_dalConfig.Clock = s_dalConfig.Clock.AddYears(1);
                        break;
                    }
                case CONFIG.GET_CLOCK:
                    {
                        Console.WriteLine(s_dalConfig.Clock);
                        break;
                    }
                case CONFIG.UPDATE:
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
                                    throw new FormatException("Wrong input");
                                s_dalConfig.Clock = dateTimeValue;
                                break;
                            case CLOCKCHOICE.RISK_RANGE:
                                Console.Write("enter Range: ");
                                if (!int.TryParse(Console.ReadLine(), out int maxRange))
                                    throw new FormatException("Wrong input");
                                int newRisk = Console.Read();
                                s_dalConfig.RiskRange = new(newRisk);
                                break;
                            default:
                                throw new FormatException("Wrong input");
                        }
                        break;
                    }
                case CONFIG.GET_MAX_RANGE:
                    {
                        Console.WriteLine(s_dalConfig.RiskRange);
                        break;
                    }
                case CONFIG.RESET_CONFIG:
                    s_dalConfig.Reset();
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

    private static void handleCreate(OPTION entity)
    {
        switch (entity)
        {
            case OPTION.VOLUNTEER:
                createVolunteer(out Volunteer st);
                s_dalVolunteer.Create(st);
                break;
            case OPTION.CALL:
                createCall(out Call cr);
                s_dalCall.Create(cr);
                break;
            case OPTION.ASSIGNMENT:
                createAssigmnet(out Assignment assi);
                s_dalAssignment.Create(assi);
                break;
            default:
                break;
        }
    }
    private static void handleRead(OPTION entity)
    {
        Console.WriteLine("Enter an id");
        if (false == int.TryParse(Console.ReadLine(), out int id))
            Console.WriteLine("Wrong input");

        switch (entity)
        {
            case OPTION.VOLUNTEER:
                Console.WriteLine(s_dalVolunteer.Read(id));
                break;
            case OPTION.CALL:
                Console.WriteLine(s_dalCall.Read(id));
                break;
            case OPTION.ASSIGNMENT:
                Console.WriteLine(s_dalAssignment.Read(id));
                break;
            default:
                break;
        }
    }
    private static void handleReadAll(OPTION entity)
    {
        switch (entity)
        {
            case OPTION.VOLUNTEER:
                foreach (var item in s_dalVolunteer.ReadAll())
                    Console.WriteLine(item);
                break;
            case OPTION.CALL:
                foreach (var item in s_dalCall.ReadAll())
                    Console.WriteLine(item);
                break;
            case OPTION.ASSIGNMENT:
                foreach (var item in s_dalAssignment.ReadAll())
                    Console.WriteLine(item);
                break;
            default:
                break;
        }
    }
    private static void handleUpdate(OPTION entity)
    {
        Console.WriteLine("Enter an id");
        if (false == int.TryParse(Console.ReadLine(), out int id))
            Console.WriteLine("Wrong input");

        switch (entity)
        {
            case OPTION.VOLUNTEER:
                Console.WriteLine(s_dalVolunteer.Read(id));
                createVolunteer(out Volunteer st, id);
                s_dalVolunteer.Update(st);
                break;
            case OPTION.CALL:
                Console.WriteLine(s_dalCall.Read(id));
                createCall(out Call cr, id);
                s_dalCall.Update(cr);
                break;
            case OPTION.ASSIGNMENT:
                Console.WriteLine(s_dalAssignment.Read(id));
                createAssigmnet(out Assignment lk, id);
                s_dalAssignment.Update(lk);
                break;
            default:
                break;
        }
    }
    private static void handleDelete(OPTION entity)
    {
        Console.WriteLine("Enter an id");
        if (false == int.TryParse(Console.ReadLine(), out int id))
            Console.WriteLine("Wrong input");

        switch (entity)
        {
            case OPTION.VOLUNTEER:
                s_dalVolunteer.Delete(id);
                break;
            case OPTION.CALL:
                s_dalCall.Delete(id);
                break;
            case OPTION.ASSIGNMENT:
                s_dalAssignment.Delete(id);
                break;
            default:
                break;
        }
    }

    private static void handleDeleteAll(OPTION entity)
    {
        switch (entity)
        {
            case OPTION.VOLUNTEER:
                s_dalVolunteer.DeleteAll();
                break;
            case OPTION.CALL:
                s_dalCall.DeleteAll();
                break;
            case OPTION.ASSIGNMENT:
                s_dalAssignment.DeleteAll();
                break;
            default:
                break;
        }
    }
    private static void showAllDB()
    {
        Console.WriteLine("--------------- List of Calls ------------------------------------------");
        foreach (var item in s_dalCall.ReadAll())
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("--------------- List of Volunteers ------------------------------------------");
        foreach (var item in s_dalVolunteer.ReadAll())
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("--------------- List of Assignments ------------------------------------------");
        foreach (var item in s_dalAssignment.ReadAll())
        {
            Console.WriteLine(item);
        }
    }
    private static void createAssigmnet(out Assignment assi, int id = 0)
    {
        Console.Write("enter VolunteerId of the Assignment: ");
        if (!int.TryParse(Console.ReadLine(), out int volId))
            throw new FormatException("Wrong input");

        Console.Write("enter CallId of the Assignment: ");
        if (!int.TryParse(Console.ReadLine(), out int cId))
            throw new FormatException("Wrong input");

        Console.WriteLine("");

        assi = new Assignment(id, volId, cId, s_dalConfig.Clock);
    }
    private static void createCall(out Call cr, int id = 0)
    {
        
        Console.Write("enter Type of the Call:  1.Puncture    2.Cables   3.LockedCar");
        int callTypeInput = int.Parse(Console.ReadLine() ?? "0");
        DO.CallType type= (DO.CallType)callTypeInput;

        Console.Write("enter Description of the Call: ");
        string? description = Console.ReadLine() ?? throw new FormatException("Wrong input");

        Console.Write("enter FullAddress of the Call: ");
        string address= Console.ReadLine() ?? throw new FormatException("Wrong input");
        
        cr = new Call(0, type, description, address, 0,0,s_dalConfig.Clock);
    }
    private static void createVolunteer(out Volunteer st, int id = 0)
    {
        if (id == 0)
        {
            Console.Write("enter volunteer Id: ");
            if (!int.TryParse(Console.ReadLine(), out int _id))
                throw new FormatException("Wrong input");
            else
                id = _id;
        }

        Console.Write("enter Name of the Volunteer: ");
        string? name = Console.ReadLine() ?? throw new FormatException("Wrong input");

        Console.WriteLine("Enter Volunteer Phone Number:");
        string? numberPhone = Console.ReadLine() ?? throw new FormatException("Wrong input");

        Console.Write("enter email of the Volunteer: ");
        string? email = Console.ReadLine() ?? throw new FormatException("Wrong input");

        Console.WriteLine("Enter Role (0 = Volunteer, 1 = Manager):");
        Role role = (Role)int.Parse(Console.ReadLine() ?? "0");

        Console.Write("enter true/false if the Volunteer is active: ");
        if (!bool.TryParse(Console.ReadLine(), out bool active))
            throw new FormatException("Wrong input");

        Console.Write("enter address of the Volunteer: ");
        string? address = Console.ReadLine() ?? throw new FormatException("Wrong input");

       
        st = new Volunteer(id, name, numberPhone, email, Distance.Aerial, role, active, address);
    }
}







