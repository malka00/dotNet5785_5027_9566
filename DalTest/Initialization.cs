using DalApi;
using DO;

namespace DalTest;
using DalApi;
using DO;
using System;
using System.Data;
using System.Net;
using System.Numerics;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public static class Initialization
{
    //private static IVolunteer? s_dal.Volunteer; //stage 1
    //private static ICall? s_dal.Call; //stage 1
    //private static IAssignment? s_dalAssignment; //stage 1
    //private static IConfig? s_dal!.Config; //stage 1
    private static readonly Random s_rand = new();
    private static IDal? s_dal; //stage 2
    private const int MIN_ID = 200000000;
    private const int MAX_ID = 400000000;


    /// <summary>
    ///func for adding 15 volunteers and a manager
    ///In the function we added an array for names, a cell phone and addresses
    ///(with longitude and street lines) to create a swing we each time take the appropriate values ​​from the exponents
    /// </summary>
    private static void CreateVolunteers()
    {
        string[] volunteerNames = { "Ruth Cohen", "Yossi Levy", "Oren Alon", "Meirav Israeli", "Dan Mizrahi", "Ayelet Israeli", "Dana Cohen",
                                "Noam Brenner", "Ofer Mizrahi", "Ron Halevi", "Liron Abutbul", "Omer Katz", "Ronit Goldman", "Ilan Shemesh", "Galit Cohen" };
        string[] phoneNumbers = { "0501234567", "0529876543", "0534567890", "0541237894", "0556789123", "0503456789", "0522345678",
                              "0539876543", "0545678912", "0551234569", "0504561237", "0527893451", "0533214567", "0547896543", "0556547891" };
        string[] emails = { "ruth@example.com", "yossi@example.com", "oren@example.com", "meirav@example.com", "dan@example.com", "ayelet@example.com", "dana@example.com",
                        "noam@example.com", "ofer@example.com", "ron@example.com", "liron@example.com", "omer@example.com", "ronit@example.com", "ilan@example.com", "galit@example.com" };

        string[] addresses =
        {
        "King George  20, Jerusalem, Israel",
        "מכון טל ",
        "Agripas  10, Jerusalem, Israel",
        "Hapalmach  25, Jerusalem, Israel",
        "Emek Refaim 43, Jerusalem, Israel",
        "hapisga 6,  Jerusalem, Israel",
        "Hillel  7, Jerusalem, Israel",
        "Derech Hebron 105, Jerusalem, Israel",
        "Bezalel 12, Jerusalem, Israel",
        "HaNeviim 29, Jerusalem, Israel",
        "Shivtei Israel  15, Jerusalem, Israel",
        "Aza 50, Jerusalem, Israel",
        "כנפי נשרים 15  ירושלים",
        "בית וגן 20 ירושלים",
        "Shivtei Israel  10,  Jerusalem, Israel"
        };

        double[] longitudes = new double[]
        {
        35.2193, 35.189689, 35.2129, 35.2065, 35.2117,
        35.212416, 35.2142, 35.2156, 35.2150, 35.2175,
        35.2214, 35.2123,  35.1837336,  35.1838085, 35.2250006
        };

        double[] latitudes = new double[]
        {
        31.7784, 31.78542, 31.7801, 31.7642, 31.7655,
        31.751709, 31.7809, 31.7515, 31.7812, 31.7837,
        31.7849, 31.7698, 31.7873914, 31.7707007, 31.7804057
        };

        string[] passwords =
        {
"SCvvz3ug",
    "D4e5F6",
    "Khoor8",
    "Sd88zrug",
    "F3glqj",
    "4567Dt",
    "Vhfxu6",
    "Tz6uw|",
    "O3jlq<",
    "W6vwPh",
    "Dgp4q$",
    "F&4567",
    "S7vvPh",
    "F3iihh",
    "Fkrfr:"
        };
        int[] volunteerId = {207488065,207669508,207733817,207862384,207926692,208317958,208448738,
            208731323,208749978,209184217,209307123,209314277,209567445,209567890,209606318,209668409,
};

        for (int i = 0; i < volunteerNames.Length; i++)
        {

            //    int Id;
            //    do
            //        Id = (int)s_rand.Next(700000000, 1000000000);    // Random 9-digit id code
            //    while (s_dal!.Volunteer.Read(Id) != null);           // Checking the uniqueness of id

            string name = volunteerNames[i];
            string phone = phoneNumbers[i];
            int id = volunteerId[i];
            string email = emails[i];
            string address = addresses[i];
            string password = passwords[i];
            double nLatitude = latitudes[i];
            double nLongitude = longitudes[i];
            Distance distanceType = Distance.Aerial;             // Default distance
            Role nRole = Role.Volunteer;                         // Default - regular volunteer
            bool active = true;                                  // The volunteer is active by default
            double maxReading = s_rand.Next(5, 100);             // Random maximum distance between 5 and 100

            s_dal!.Volunteer.Create(new Volunteer(id, name, phone, email ,distanceType, nRole, active, password, address, nLatitude, nLongitude, maxReading));
        }

        // Added at least one manager
        int managerId = 330824459;

        //do
        //    managerId = s_rand.Next(100000000, 1000000000);
        //while (s_dal!.Volunteer.Read(managerId) != null);

        s_dal!.Volunteer.Create(new Volunteer(326209566, "Malka Haupt", "0501111111", "admin@example.com",Distance.Aerial, Role.Boss, true, "D4567", "hapisga 16,  Jerusalem, Israel", 31.7692558, 35.1824528));
        s_dal!.Volunteer.Create(new Volunteer(214425027, "Efrat Sharabi ", "0501111111", "admin@example.com", Distance.Aerial, Role.Boss, true, "D4567", "hapisga 16,  Jerusalem, Israel", 31.7692558, 35.1824528));

    }


    /// <summary>
    /// A function that creates 50 diverse readings according to the requirements.
    ///In the function we created arrays for addresses, longitudes and street, as well as arrays for describing the case according to the type of case
    ///For each reading we took variables accordingly
    /// </summary>
    private static void CreateCalls()
{
    string[] DescriptionsP = {
   "Flat tire on the driver's side front wheel, car parked on a narrow roadside.",
    "Severe puncture in the rear left tire, located in a high-traffic area.",
    "Tire burst on a major highway, requiring immediate assistance.",
    "Front right tire has a slow leak; customer noticed deflation after parking.",
    "Flat tire detected while parked in a shopping center parking lot.",
    "Vehicle with a blown rear right tire near a school zone.",
    "Multiple punctures in both rear tires, stranded in an industrial area.",
    "Emergency - tire went flat on a rainy day in an unlit area.",
    "Flat front tire, customer has no spare available.",
    "Rear tire appears punctured from debris, located near a construction site.",
    "Customer suspects a nail puncture, vehicle stopped by a rural road.",
    "Slow deflation on both front tires, customer requests immediate help.",
    "Rear left tire went flat while driving, customer safely parked.",
    "Tire deflation due to hitting a pothole, in a crowded downtown area.",
    "Front tire completely flat, vehicle blocking entrance to an office complex.",
    "Flat tire discovered after vehicle was parked overnight.",
    "Large puncture on rear tire, parked near a recreational park.",
    "Customer reports a flat front tire, low visibility area.",
    "Severe tire damage on front left, car located in a residential area.",
    "Flat tire needing urgent assistance, no repair shops nearby."};

    string[] DescriptionsL = {
            "Keys locked inside the vehicle; customer is at a gas station.",
            "Child accidentally locked inside the car; urgent assistance needed.",
            "Locked car with keys inside at a remote parking lot.",
            "Customer left keys in trunk, car is locked in a shopping mall lot.",
            "Keys locked in vehicle with engine running, parked at an office complex.",
            "Customer locked out of car after leaving keys on the seat.",
            "Locked vehicle outside a stadium, customer unable to access the car.",
            "Customer left keys inside, vehicle parked near a hospital entrance.",
            "Car locked with pet inside, parked in direct sunlight.",
            "Locked car in basement parking, customer needs urgent entry.",
            "Keys locked in a car near a hiking trail entrance.",
            "Customer locked out of vehicle in a high-security parking area.",
            "Customer’s car locked with all items, including phone, inside.",
            "Vehicle locked outside a hotel, customer unable to enter.",
            "Keys locked in car at airport drop-off area, customer stranded.",
            "Customer accidentally locked keys inside while filling gas.",
            "Locked car parked at a convention center, urgent entry needed.",
            "Customer reports keys left inside, parked at a university campus.",
            "Vehicle locked near a busy restaurant; customer waiting for help.",
            "Car locked at beach parking, urgent help required for entry." };

    string[] DescriptionsC = {
              "Battery drained in the middle of a long bridge, customer needs immediate help.",
    "Dead battery at a theme park parking lot, family stranded with children.",
    "Battery failed at a rural farm, customer requires assistance urgently.",
    "Engine won’t start in an underground garage after leaving lights on.",
    "Vehicle stranded on a snowy mountain pass, jump-start cables required.",
    "Customer left the air conditioning running, now needs cable assistance in a desert area.",
    "Dead battery at a hotel driveway, guest stranded before checkout.",
    "Battery failure during a road trip, customer stuck at a scenic overlook.",
    "Vehicle won’t start after fishing trip, stranded near a remote lake.",
    "Car battery died at a golf course parking lot, jump-start needed urgently.",
    "Customer’s battery failed at a local park during a picnic, needs help.",
    "Dead battery after a football game, stranded near the stadium exit.",
    "Battery drained in heavy fog, car stuck on a rural highway.",
    "Vehicle won’t start after leaving hazard lights on, parked on a quiet street.",
    "Dead battery at a university campus, urgent assistance requested.",
    "Battery failure at a movie theater parking lot, customer stranded late at night.",
    "Car won’t start after a rainy night, parked near a construction site.",
    "Battery died while waiting in a ferry line, assistance needed immediately.",
    "Engine won’t start after customer left headlights on in a supermarket parking lot.",
    "Dead battery on a remote hiking trail parking area, customer stranded with no signal.",
            "Battery dead, requires jump-start cables near a busy intersection.",
            "Customer stranded with a weak battery, needs cables urgently.",
            "Dead battery in a dark area, customer requests cable assistance.",
            "Battery failure near a crowded shopping area, needs jump-start.",
            "Engine won’t start due to battery; customer needs cable connection.",
            "Battery drained after leaving lights on; car located in open field.",
            "Dead battery in extreme cold, urgent jump-start assistance needed.",
            "Customer needs jump-start cables in a remote parking lot.",
            "Battery died while waiting in traffic, customer stuck at roadside.",
            "Car needs a battery boost at a highway rest area.",
            "Battery issue due to long inactivity, vehicle located in private garage.",
            "Customer left radio on, now needs cables to start car.",
            "Dead battery, urgent assistance needed in a dark parking structure.",
            "Car won’t start in a forested camping area; needs cable assistance.",
            "Customer needs cable assistance after draining battery at concert.",
            "Battery died after parking overnight, assistance needed in suburban area.",
            "Vehicle requires jump-start, stranded near a major shopping complex.",
            "Customer requesting cable assistance after battery failure on beach.",
            "Battery needs a jump-start, parked near a train station.",
            "Engine won’t start in a heavy rainstorm, requires jump-start cables.",
            "Customer left interior lights on, needs jump-start in a hospital parking lot.",
"Dead battery on a busy downtown street, urgent cable assistance required.",
"Car won’t start after a camping trip, stranded near a mountain trail.",
"Battery failure after overnight snowfall, customer needs jump-start in a neighborhood driveway.",
"Vehicle stalled with a dead battery at a school pickup zone, urgent assistance needed.",
"Battery drained at a gas station, customer requesting cable help.",
"Customer stranded in a remote village, requires urgent jump-start assistance.",
"Car battery died after music system was left on, parked near a community center.",
"Engine won’t start at a marina parking lot, cable assistance needed urgently.",
"Battery failure on a scenic route, customer stranded with no nearby help."

        };

        string[] addresses = new string[]
      {
    " David Hamelekh 30, Jerusalem, Israel",
    "Keren Hayesod 10, Jerusalem, Israel",
    "HaRav Kook 8, Jerusalem, Israel",
    "Strauss 3, Jerusalem, Israel",
    "Radak 9, Jerusalem, Israel",
    "Bezalel 30, Jerusalem, Israel",
    "Shmuel HaNagid 16, Jerusalem, Israel",
    "רבי ישראל נאג'רה 26 ירושלים ישראל",
    "Shivtei Israel 28, Jerusalem, Israel",
    "עוזיאל 35 ירושלים ישראל",/*10*/
    "Rabbi Akiva 12, Jerusalem, Israel",
    "Haneviim 47, Jerusalem, Israel",
    "Yemin Moshe 10, Jerusalem, Israel",
    "Yoel Moshe Salomon 15, Jerusalem, Israel",
    "באזל 10 ירושלים",
    "Beit Hakerem 20, Jerusalem, Israel",
    "Givat Shaul 19, Jerusalem, Israel",
    "אליעזר הלוי 7, ירושלים ",
    "Emek Refaim 9, Jerusalem, Israel",
    "Aza 45, Jerusalem, Israel",/*20*/
    "Har HaTsofim 15, Jerusalem, Israel",
    "שמריהו לוין 55 ירושלים",
    "Nablus Rd 15, Jerusalem, Israel",
    "חברון 51 ירושלים",/*25*/
    "HaPalmach 15, Jerusalem, Israel",
    "Lincoln 7, Jerusalem, Israel",
    "ברוך דובדבני 15 ירושלים",
    "דיסקין 7 ירושלים ישראל",
    "Alkalai 10, Jerusalem, Israel",/*30*/
    "Ramban 13, Jerusalem, Israel",
    "מרדכי בן הלל 10 ירושלים",
    "HaRav Herzog 5, Jerusalem, Israel",
    "אגרון 18 ירושלים",
    "Givon 3, Jerusalem, Israel",/*35*/
    "בצלאל 19 ירושלים",
    "בית וגן 14 ירושלים",
    "Harav Shach 9, Jerusalem, Israel",
    "מגרש הרוסים ירושלים",
    "Shaarei Tsedek 1, Jerusalem, Israel",/*40*/
    "Givat Mordechai 12, Jerusalem, Israel",
    "יצחק רפאל 15 ירושלים",
    "Sanhedria 14, Jerusalem, Israel",
    "Bar Ilan 27, Jerusalem, Israel",
    "Shmuel Hanavi 50, Jerusalem, Israel",/*45*/
    "Malha  7, Jerusalem, Israel",
    "Pisgat Ze'ev Blvd 6, Jerusalem, Israel",
    "מלחה 13 ירושלים",
    "צפת 15 ירושלים",
    "Ha'Arazim 3, Jerusalem, Israel",
    "Ramot Forest, Jerusalem, Israel",
    "Yirmiyahu Street, Jerusalem, Israel"
      };

        double[] longitudes = new double[]
        {
        35.2222988,  35.1420228, 35.2150, 35.2174, 35.2132,
        35.2165, 35.2138,  35.1911328, 35.2241, 35.1850035,
        35.2123, 35.2202, 35.2214,  35.2198634, 35.1927668,
        35.2025, 35.1965, 35.193053, 35.2151, 35.2124,
       35.2123254,  35.1732123, 35.2315, 35.2255189, 35.2064,/*25*/
        35.2237, 35.2208,  35.1917498,  35.2103608, 35.2081,
        35.2079, 35.2174742, 35.2110343, 35.2200697, 35.1963,/*35*/
        35.2114904,35.1837672, 35.2159,  35.223003, 35.2210,
        35.2342, 35.2158382, 35.2236, 35.2087, 35.2381,/*45*/
         35.1829753,  35.2485718, 35.1829753,  35.2116742, 35.1873538
        };




        double[] latitudes = new double[]
    {
       31.7733536, 31.8001486, 31.7839, 31.7843, 31.7785,
      31.7803, 31.7809, 31.7894182, 31.7825,  31.7700112,
      31.7812, 31.7828, 31.7695, 31.7809598, 31.7840322,
      31.7687, 31.7945, 31.7844766, 31.7684, 31.7699,
      31.7706324, 31.7683803, 31.7915, 31.7643213, 31.7687,/*25*/
      31.7802, 31.7805,31.761432, 31.7760007, 31.7801,
      31.7821, 31.7817481, 31.7698338, 31.7766198, 31.7784,/*35*/
      31.7814544, 31.7712662, 31.7623, 31.7813648, 31.7981,
      31.7742, 31.7219788, 31.7591, 31.7545, 31.7695,
      31.7514658, 31.7947, 31.7514658, 31.7795649, 31.7803494
    };

        int p = 0, l = 0, c = 0;
        ///created 50 readings
        for (int i = 0; i < 50; i++)
        {
            CallType cType; 
            string nDescription;
            //The rest of the division is a dish that came out diverse..

            if (i % 3 == 0)
            {
                cType = CallType.Puncture;
                nDescription = DescriptionsP[p];
                p++;
            }
            else if (i % 4 == 0)
            {
                cType = CallType.LockedCar;
                nDescription = DescriptionsL[l];
                l++;
            }
            else
            {
             
                cType = CallType.Cables;            // else value if no condition is met
                nDescription = DescriptionsC[c];
                c++;
               
            }
            DateTime start = s_dal!.Config.Clock.AddDays(-1);

            // Calculate the number of minutes since the start time until now
            int totalMinutesInLastDay = (int)(s_dal!.Config.Clock - start).TotalMinutes;
            // Random opening time within the last 24 hours
            DateTime RandomStart = start.AddMinutes(s_rand.Next(0, totalMinutesInLastDay));
            DateTime? RandomEnd = null;

            //if (i % 5 == 0)
            //{
            //    //calls that have passed the time
            //    int maxRange = (int)(s_dal!.Config.Clock - RandomStart).TotalMinutes;
            //    if (maxRange > 0) 
            //        RandomEnd = RandomStart.AddMinutes(s_rand.Next(1, maxRange + 1));
            //}
            //else
            if(i% 5==0) 
            {
                int maxDurationMinutes = s_rand.Next(1, 180);
                RandomEnd = RandomStart.AddMinutes(maxDurationMinutes);
            }
            else
                if (s_rand.Next(2) == 1)
                {
     
                    int maxDurationMinutes = s_rand.Next(1, 1441);
                    RandomEnd = RandomStart.AddMinutes(maxDurationMinutes);
                }
            
            s_dal.Call.Create(new Call(0, cType, nDescription, addresses[i], latitudes[i], longitudes[i], RandomStart, RandomEnd));
        }

    }

    /// <summary>
    /// Function for creating readings We have created diverse readings as required
    /// </summary>
    private static void CreateAssignment()
    {
        for (int i = 0; i < 60; i++)
        {
            //Assigning a volunteer to a task
            int randVolunteer = s_rand.Next(s_dal!.Volunteer.ReadAll().Count());
             Volunteer volunteerToAssign = s_dal.Volunteer.ReadAll().OrderBy(v => s_rand.Next()).First();
            //call number ID
            int randCAll = s_rand.Next(s_dal.Call!.ReadAll().Count() - 15);
            Call callToAssig = s_dal.Call.ReadAll().OrderBy(v => s_rand.Next()).First();
            while (callToAssig.TimeOpened > s_dal!.Config!.Clock)
            {
                randCAll = s_rand.Next(s_dal.Call!.ReadAll().Count() - 15);
                callToAssig = s_dal.Call.ReadAll().OrderBy(v => s_rand.Next()).First();
            }
            TypeEnd? finish = null;
            DateTime? finishTime = null;
            if (callToAssig.MaxTimeToClose != null && callToAssig.MaxTimeToClose >= s_dal!.Config?.Clock)
            {
                finish = TypeEnd.ExpiredCancel;
                finishTime = s_dal.Config.Clock;
            }
            else
            {
                int randFinish = s_rand.Next(0, 4);
                switch (randFinish)
                {
                    case 0:
                        finish = TypeEnd.Treated;
                        finishTime = s_dal!.Config!.Clock;
                        break;
                    case 1: finish = TypeEnd.SelfCancel;
                        finishTime = s_dal.Config.Clock;
                        break;
                    case 2: finish = TypeEnd.ManagerCancel;
                        finishTime = s_dal.Config.Clock; break;
                }
            }
            s_dal.Assignment?.Create(new Assignment(0, callToAssig.Id, volunteerToAssign.Id, s_dal!.Config!.Clock, finishTime, finish));
        }
    }

    //public static void Do(IStudent? dalStudent, ICourse? dalCourse, ILink? dalStudentInCourse, IConfig? dalConfig) // stage 1
    //public static void Do(IDal dal)  //stage 2
    public static void Do() //stage 4
    {
        //s_dal.Volunteer = dal_volunteer ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dal.Call = dal_call ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalAssignment = dal_assignment ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dal!.Config = dal_Config ?? throw new NullReferenceException("DAL object can not be null!");
       
        //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2                                                                                //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); //stage 2
        s_dal = DalApi.Factory.Get; //stage 4
        Console.WriteLine("Reset Configuration values and List values...");
        //s_dal!.Config.Reset(); //stage 1
        //s_dal.Volunteer.DeleteAll(); //stage 1
        //s_dal.Call.DeleteAll(); //stage 1
        //s_dalAssignment.DeleteAll(); //stage 1

        s_dal.ResetDB();//stage 2

        Console.WriteLine("Initializing Volunteers list");
        CreateVolunteers();
        Console.WriteLine("Initializing Calls list");
        CreateCalls();
        Console.WriteLine("Initializing Assignments list");
        CreateAssignment();
    }
}



