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
        string[] phoneNumbers = { "050-1234567", "052-9876543", "053-4567890", "054-1237894", "055-6789123", "050-3456789", "052-2345678",
                              "053-9876543", "054-5678912", "055-1234569", "050-4561237", "052-7893451", "053-3214567", "054-7896543", "055-6547891" };
        string[] emails = { "ruth@example.com", "yossi@example.com", "oren@example.com", "meirav@example.com", "dan@example.com", "ayelet@example.com", "dana@example.com",
                        "noam@example.com", "ofer@example.com", "ron@example.com", "liron@example.com", "omer@example.com", "ronit@example.com", "ilan@example.com", "galit@example.com" };

        string[] addresses =
        {
        "King George St 20, Jerusalem, Israel",
        "bit hadfus 7  ,Jerusalem, Israel",
        "Agripas St 10, Jerusalem, Israel",
        "Hapalmach St 25, Jerusalem, Israel",
        "Emek Refaim St 43, Jerusalem, Israel",
        "Hapisga St 18, Jerusalem, Israel",
        "Hillel St 7, Jerusalem, Israel",
        "Derech Hebron 105, Jerusalem, Israel",
        "Bezalel St 12, Jerusalem, Israel",
        "HaNeviim St 29, Jerusalem, Israel",
        "Shivtei Israel St 15, Jerusalem, Israel",
        "Azza St 50, Jerusalem, Israel",
        "Kriyat Hayuvel St 5, Jerusalem, Israel",
        "Prophets St 23, Jerusalem, Israel",
        "Ben Yehuda St 1, Jerusalem, Israel"
        };

        double[] longitudes = new double[]
        {
        35.2193, 35.189689, 35.2129, 35.2065, 35.2117,
        35.212416, 35.2142, 35.2156, 35.2150, 35.2175,
        35.2214, 35.2123, 35.18130622417343, 35.2191, 35.2203
        };

        double[] latitudes = new double[]
        {
        31.7784, 31.78542, 31.7801, 31.7642, 31.7655,
        31.751709, 31.7809, 31.7515, 31.7812, 31.7837,
        31.7849, 31.7698, 31.770868836791788, 31.7815, 31.7822
        };

        string[] passwords =
        {
    "P@ssw0rd",
    "A1b2C3",
    "Hello5",
    "Pa55word",
    "C0ding",
    "1234Aq",
    "Secur3",
    "Qw3rty",
    "L0gin9",
    "T3stMe",
    "Adm1n!",
    "C#1234",
    "P4ssMe",
    "C0ffee",
    "Choco7"
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

        s_dal!.Volunteer.Create(new Volunteer(managerId, "Admin Man", "050-1111111", "admin@example.com",Distance.Aerial, Role.Boss, true, "A1234", "Harounoff  street 2, Jerusalem, Israel", 31.796294, 35.218994));
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
            "Engine won’t start in a heavy rainstorm, requires jump-start cables."
        };
  
    string[] addresses = new string[]
    {
        "King David St 15, Jerusalem, Israel",
        "Keren Hayesod St 24, Jerusalem, Israel",
        "HaRav Kook St 8, Jerusalem, Israel",
        "Strauss St 3, Jerusalem, Israel",
        "Radak St 9, Jerusalem, Israel",
        "Bezalel St 30, Jerusalem, Israel",
        "Shmuel HaNagid St 16, Jerusalem, Israel",
        "David HaMelech St 4, Jerusalem, Israel",
        "Shivtei Israel St 28, Jerusalem, Israel",
        "HaMesila Park, Jerusalem, Israel",
        "Rabbi Akiva St 12, Jerusalem, Israel",
        "Haneviim St 47, Jerusalem, Israel",
        "Yemin Moshe St 10, Jerusalem, Israel",
        "Yoel Moshe Salomon St 18, Jerusalem, Israel",
        "HaOren St 5, Jerusalem, Israel",
        "Beit Hakerem St 20, Jerusalem, Israel",
        "Givat Shaul St 19, Jerusalem, Israel",
        "Shlomo Zalman Shragai St 8, Jerusalem, Israel",
        "Emek Refaim St 9, Jerusalem, Israel",
        "Azza St 45, Jerusalem, Israel",
        "Derech Har HaTsofim 15, Jerusalem, Israel",
        "Mount Scopus Campus, Jerusalem, Israel",
        "Nablus Rd 15, Jerusalem, Israel",
        "Hebron Rd 76, Jerusalem, Israel",
        "HaPalmach St 15, Jerusalem, Israel",
        "Lincoln St 7, Jerusalem, Israel",
        "Duvdevani St 6, Jerusalem, Israel",
        "Diskin St 16, Jerusalem, Israel",
        "Alkalai St 10, Jerusalem, Israel",
        "Ramban St 13, Jerusalem, Israel",
        "Mordechai Ben Hillel St 10, Jerusalem, Israel",
        "HaRav Herzog St 52, Jerusalem, Israel",
        "Gershon Agron St 10, Jerusalem, Israel",
        "Givon St 3, Jerusalem, Israel",
        "Golda Meir Blvd 75, Jerusalem, Israel",
        "Lev Ram Blvd 2, Jerusalem, Israel",
        "Harav Shach St 9, Jerusalem, Israel",
        "Kiryat HaLeom, Jerusalem, Israel",
        "Shaarei Tsedek St 1, Jerusalem, Israel",
        "Givat Mordechai St 12, Jerusalem, Israel",
        "Bayit Vegan St 8, Jerusalem, Israel",
        "Sanhedria St 14, Jerusalem, Israel",
        "Bar Ilan St 27, Jerusalem, Israel",
        "Shmuel Hanavi St 50, Jerusalem, Israel",
        "Malha Rd 7, Jerusalem, Israel",
        "Pisgat Ze'ev Blvd 6, Jerusalem, Israel",
        "Teddy Stadium, Jerusalem, Israel",
        "Zahal St 5, Jerusalem, Israel",
        "Ha'Arazim Blvd 3, Jerusalem, Israel",
        "Ramot Forest, Jerusalem, Israel",
        "Yirmiyahu Street, Jerusalem,Israel"
    };
        double[] longitudes = new double[]
        {
        35.2252, 35.2168, 35.2150, 35.2174, 35.2132,
        35.2165, 35.2138, 35.2245, 35.2241, 35.2206,
        35.2123, 35.2202, 35.2214, 35.2155, 35.2198,
        35.2025, 35.1965, 35.1942, 35.2151, 35.2124,
        35.2387, 35.2421, 35.2315, 35.2092, 35.2064,
        35.2237, 35.2208, 35.2103, 35.2127, 35.2081,
        35.2079, 35.2145, 35.2045, 35.2120, 35.1963,
        35.2112, 35.2008, 35.2159, 35.2235, 35.2210,
        35.2342, 35.2149, 35.2236, 35.2087, 35.2381,
        35.2201, 35.2357, 35.2289, 35.2325, 35.207448
        };




        double[] latitudes = new double[]
    {
      31.7767, 31.7745, 31.7839, 31.7843, 31.7785,
      31.7803, 31.7809, 31.7763, 31.7825, 31.7694,
      31.7812, 31.7828, 31.7695, 31.7823, 31.7604,
      31.7687, 31.7945, 31.7893, 31.7684, 31.7699,
      31.8005, 31.8017, 31.7915, 31.7473, 31.7687,
      31.7802, 31.7805, 31.7772, 31.7794, 31.7801,
      31.7821, 31.7741, 31.7897, 31.7685, 31.7784,
      31.7695, 31.7982, 31.7623, 31.7598, 31.7981,
      31.7742, 31.8012, 31.7591, 31.7545, 31.7695,
      31.7901, 31.7947, 31.8014, 31.7903, 31.792351
    };


        ///created 50 readings
        for (int i = 0; i < 50; i++)
        {
            CallType cType; 
            string nDescription;
            //The rest of the division is a dish that came out diverse..
            int p = 0, l = 0, c = 0;
            if (i % 3 == 0)
            {
                cType = CallType.Puncture;
                nDescription = DescriptionsP[p];
                p++;
            }
            else if (i % 4 == 0)
            {
                cType = CallType.LockedCar;
                nDescription = DescriptionsP[l];
                l++;
            }
            else
            {
                cType = CallType.Cables;            // else value if no condition is met
                nDescription = DescriptionsP[c];
                c++;
            }
            DateTime start = s_dal!.Config.Clock.AddDays(-1);

            // Calculate the number of minutes since the start time until now
            int totalMinutesInLastDay = (int)(s_dal!.Config.Clock - start).TotalMinutes;
            // Random opening time within the last 24 hours
            DateTime RandomStart = start.AddMinutes(s_rand.Next(0, totalMinutesInLastDay));
            DateTime? RandomEnd = null;

            if (i % 10 == 0)
            {
                //calls that have passed the time
                int maxRange = (int)(s_dal!.Config.Clock - RandomStart).TotalMinutes;
                if (maxRange > 0) 
                    RandomEnd = RandomStart.AddMinutes(s_rand.Next(1, maxRange + 1));
            }
            else
            {
                if (s_rand.Next(2) == 1)
                {
     
                    int maxDurationMinutes = s_rand.Next(1, 1441);
                    RandomEnd = RandomStart.AddMinutes(maxDurationMinutes);
                }
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
                    case 1: finish = TypeEnd.SelfCancel; break;
                    case 2: finish = TypeEnd.ManagerCancel; break;
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



