namespace BlTest;
using BlApi;


public enum OPTION
{
    EXIT,
    ADMIN_MENUE,
    VOLUNTEER_MENUE,
    CALL_MENUE,
    //SHOW_ALL_DB,
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
    EnterSystem,
    GETVOlUNTEERLIST,
    READ,
    UPDATE,
    DELETE,
    CREATE,
}
public enum ICall
{
    EXIT,
    CountCall,
    GetCallInLists,
    Read,
    Update,
    Delete,
    Create,
    GetClosedCall,
    GetOpenCall,
    CloseTreat,
    CancelTreat,
    ChoseForTreat,
}

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
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

    private static OPTION showMainMenu()
    {
        int choice;
        do
        {
            Console.WriteLine(@"
OPTION Options:
0 - Exit
1 - Admin
2 - Volunteer
3 - Call

");

        }
        while (!int.TryParse(Console.ReadLine(), out choice));
        return (OPTION)choice;
    }
    private static IAdmmin showAdminMenu()
    {
        int choice;
        do
        {
            Console.WriteLine(@$"
Config Options:
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
                    break;
            }
        }
        catch { }
    }
}
