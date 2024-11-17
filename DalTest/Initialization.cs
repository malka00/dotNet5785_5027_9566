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


public static class Initialization
{
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static IAssignment? s_dalAssignment; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();
//    public static readonly string[] addresses = new string[]
// {
//     "Pick Street 2, Jerusalem, Israel",
//     "Gat Street 17, Jerusalem, Israel",
//     "Yitzhak Rabin Boulevard, Jerusalem, Israel",
//     "Rav Tza'eir Street 11, Jerusalem, Israel",
//     "Hayyim Nahman Byalik Street 3, Jerusalem, Israel",
//     "He'halutz Street 38, Jerusalem, Israel",
//     "Ha'kham Shim'on Agassi Street 25, Jerusalem, Israel",
//     "HaRav Aron Kotler Street 12, Jerusalem, Israel",
//     "Hame'iri Boulevard 9–17, Jerusalem, Israel",
//     "Ma'lot Sar Shalom, Jerusalem, Israel",
//     "Herzl Boulevard 18, Jerusalem, Israel",
//     "Shoshana Polaikov Street 2, Jerusalem, Israel",
//     "Zev Vilnay Street 4, Jerusalem, Israel",
//     "Kiryat Moshe Street 3, Jerusalem, Israel",
//     "HaRav H I Kosovski Street 6, Jerusalem, Israel",
//     "Ma'agal Bet HaMidrash 20, Jerusalem, Israel",
//     "Karmon Street 6, Jerusalem, Israel",
//     "HaBanay Street 34, Jerusalem, Israel",
//     "HaSolelim 6, Jerusalem, Israel",
//     "Eli'ezer HaLevi Street 21, Jerusalem, Israel",
//     "Ben Ziyon Street 17, Jerusalem, Israel",
// };


//    //array of latitudes
//    public static readonly double[] latitudes = new double[]
//{
//     31.7854, 31.7865, 31.7834, 31.7796, 31.7786, 31.7829, 31.7906, 31.7841, 31.7866, 31.7859,
//     31.7858, 31.7848, 31.7857, 31.7846, 31.7809, 31.7798, 31.7778, 31.7764, 31.7861, 31.7895
//};

//    //array of longitudes
//    public static readonly double[] longitudes = new double[]
//    {
//     31.7854, 35.1969, 35.1931, 35.1897, 35.1898, 35.1749, 35.1942, 35.1947, 35.1944, 35.1979,
//     35.1954, 35.1978, 35.1968, 35.1950, 35.1903, 35.1931, 35.1919, 35.1910, 35.1932, 35.1941
//    };

    private static void CreateVolunteers()
    {
        string[] VolunteerNames = { "Ruth Cohen", "Yossi Levy", "Oren Alon", "Meirav Israeli", "Dan Mizrahi", "Ayelet Israeli", "Dana Cohen",
                                "Noam Brenner", "Ofer Mizrahi", "Ron Halevi", "Liron Abutbul", "Omer Katz", "Ronit Goldman", "Ilan Shemesh", "Galit Cohen" };
        string[] PhoneNumbers = { "050-1234567", "052-9876543", "053-4567890", "054-1237894", "055-6789123", "050-3456789", "052-2345678",
                              "053-9876543", "054-5678912", "055-1234569", "050-4561237", "052-7893451", "053-3214567", "054-7896543", "055-6547891" };
        string[] Emails = { "ruth@example.com", "yossi@example.com", "oren@example.com", "meirav@example.com", "dan@example.com", "ayelet@example.com", "dana@example.com",
                        "noam@example.com", "ofer@example.com", "ron@example.com", "liron@example.com", "omer@example.com", "ronit@example.com", "ilan@example.com", "galit@example.com" };

        // מערך כתובות
        /* public static readonly*/
        string[] addresses =
{
        "King George St 20, Jerusalem, Israel",
        "Jaffa St 45, Jerusalem, Israel",
        "Agripas St 10, Jerusalem, Israel",
        "HaPalmach St 25, Jerusalem, Israel",
        "Emek Refaim St 43, Jerusalem, Israel",
        "Shlomzion HaMalka St 18, Jerusalem, Israel",
        "Hillel St 7, Jerusalem, Israel",
        "Derech Hebron 105, Jerusalem, Israel",
        "Bezalel St 12, Jerusalem, Israel",
        "HaNeviim St 29, Jerusalem, Israel",
        "Shivtei Israel St 15, Jerusalem, Israel",
        "Azza St 50, Jerusalem, Israel",
        "Yitzhak Kariv St 4, Jerusalem, Israel",
        "Prophets St 23, Jerusalem, Israel",
        "Ben Yehuda St 1, Jerusalem, Israel"
        };

        // מערך קווי האורך
        /*   public static readonly*/
        double[] Longitudes = new double[]
{
        35.2193, 35.2137, 35.2129, 35.2065, 35.2117,
        35.2205, 35.2142, 35.2156, 35.2150, 35.2175,
        35.2214, 35.2123, 35.2241, 35.2191, 35.2203
};

        // מערך קווי הרוחב
        /*public static readonly*/
        double[] Latitudes = new double[]
{
        31.7784, 31.7834, 31.7801, 31.7642, 31.7655,
        31.7798, 31.7809, 31.7515, 31.7812, 31.7837,
        31.7849, 31.7698, 31.7714, 31.7815, 31.7822
};


        // יצירת מתנדבים רגילים
        for (int i = 0; i<VolunteerNames.Length; i++)
        {

            int Id;
            do
                Id = s_rand.Next(700000000, 1000000000); // ת.ז אקראית עם 9 ספרות
            while (s_dalVolunteer!.Read(Id) != null); // בדיקת ייחודיות של ת.ז.

            string Name = VolunteerNames[i];
            string Phone = PhoneNumbers[i];
            string Email = Emails[i];
            string Address = addresses[i];
            double NLatitude = Latitudes[i];
            double NLongitude = Longitudes[i];
            Distance DistanceType = Distance.Aerial; // מרחק ברירת מחדל
            Role Nrole = Role.Volunteer; // ברירת מחדל - מתנדב רגיל
            bool Active = true; // המתנדב פעיל כברירת מחדל
            double MaxReading = s_rand.Next(5, 100); // מרחק מקסימלי אקראי בין 5 ל-100

            s_dalVolunteer!.Create(new Volunteer(Id, Name, Phone, Email, DistanceType, Nrole, Active, null, Address, NLatitude, NLongitude, MaxReading));
        }

        // הוספת מנהל אחד לפחות
        int managerId;
        do
            managerId = s_rand.Next(100000000, 1000000000);
        while (s_dalVolunteer!.Read(managerId) != null);

        s_dalVolunteer!.Create(new Volunteer(managerId, "Admin Man", "050-1111111", "admin@example.com", Distance.Aerial, Role.Boss, true, "password123", "HaPega Street 16, Jerusalem, Israel", 31.771959, 35.217018));
    }





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



    // מערך כתובות
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
        "Eliyahu Bashan St 4, Jerusalem, Israel"
    };

    // מערך קווי האורך
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
        35.2201, 35.2357, 35.2289, 35.2325, 35.2291
   };

        // מערך כתובות
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
        "Eliyahu Bashan St 4, Jerusalem, Israel"
        };

        // מערך קווי האורך
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
        35.2201, 35.2357, 35.2289, 35.2325, 35.2291
       };

        // מערך קווי הרוחב
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
        31.7901, 31.7947, 31.8014, 31.7903, 31.7885
       };



        // יצירת קריאות
        for (int i = 0; i < 50; i++)
        {



            CallType ctype;  // הכרזה על המשתנה פעם אחת
            string ndescription;
            int p = 0, l = 0, c = 0;
            if (i % 3 == 0)
            {
                ctype = CallType.Puncture;
                ndescription = DescriptionsP[p];
                p++;
            }
            else if (i % 4 == 0)
            {
                ctype = CallType.LockedCar;
                ndescription = DescriptionsP[l];
                l++;
            }
            else
            {
                ctype = CallType.Cables;  // ערך אחר אם לא מתקיים אף תנאי
                ndescription = DescriptionsP[c];
                c++;
            }



            //DateTime? maxTimeToClose =
            //DateTime start = s_dalConfig.Clock.AddDays(-1); // זמן התחלה יהיה לפני 24 שעות מהשעון הנוכחי
            //int range = (s_dalConfig.Clock - start).Minutes; // חישוב מספר הדקות מאז זמן ההתחלה
            //DateTime RndomStart = start.AddMinutes(s_rand.Next(range)); // הגרלת זמן פתיחה רנדומלי בתוך ה-24 שעות האחרונות
            //DateTime? RandomEnd = null; 
            //if (i % 10 == 0)//5 that endtime- pag tokef

            //    RandomEnd = RndomStart.AddMinutes(new Random().Next(, (int)(s_dalConfig.Clock - RndomStart).TotalMinutes));
            //// כעת, ניצור זמן סיום מקסימלי בהתאם לזמן הפתיחה:
            //else
            //{
            //    // ערך ברירת מחדל, אין זמן סיום
            //    if (s_rand.Next(2) == 1) // החלטה רנדומלית אם לכלול זמן סיום או לא
            //    {
            //        int maxDurationMinutes = s_rand.Next(1, 1441); // הגדרת טווח זמן סיום מקסימלי (עד 1440 דקות = 24 שעות)
            //        RandomEnd = RndomStart.AddMinutes(maxDurationMinutes); // זמן סיום הוא זמן פתיחה + מספר דקות רנדומלי
            //    }
            //}
            // הגדרת זמן התחלה - 24 שעות אחורה מהשעה הנוכחית
            DateTime start = s_dalConfig.Clock.AddDays(-1);

            // חישוב מספר הדקות מאז זמן ההתחלה ועד עכשיו
            int totalMinutesInLastDay = (int)(s_dalConfig.Clock - start).TotalMinutes;

            // זמן פתיחה רנדומלי בתוך ה-24 שעות האחרונות
            DateTime RndomStart = start.AddMinutes(s_rand.Next(0, totalMinutesInLastDay));

            // זמן סיום אופציונלי
            DateTime? RandomEnd = null;

            // אם i מתחלק ב-10
            if (i % 10 == 0)
            {
                // זמן סיום רנדומלי בתוך החלון בין RndomStart לזמן הנוכחי
                int maxRange = (int)(s_dalConfig.Clock - RndomStart).TotalMinutes;
                if (maxRange > 0) // רק אם יש טווח אפשרי
                {
                    RandomEnd = RndomStart.AddMinutes(s_rand.Next(1, maxRange + 1));
                }
            }
            else
            {
                // הסתברות של 50% לכלול זמן סיום
                if (s_rand.Next(2) == 1)
                {
                    // זמן סיום יהיה בין דקה אחת ל-24 שעות (1440 דקות) אחרי זמן הפתיחה
                    int maxDurationMinutes = s_rand.Next(1, 1441);
                    RandomEnd = RndomStart.AddMinutes(maxDurationMinutes);
                }
            }


            // נשאר להתעסק עם הכתובות...
            s_dalCall.Create(new Call(0, ctype, ndescription, addresses[i], latitudes[i], longitudes[i], RndomStart, RandomEnd));
        }

    }
    private static void CreateAssignment()
    {
        for (int i = 0; i < 50; i++)
        {
            var Calls = s_dalCall.ReadAll();
            Random RandomC = new Random();
            int RandomNumberC = RandomC.Next(0, 51);
            int CallId = Calls[RandomNumberC].Id;

            //DateTime? EndTime= Calls[RandomNumberC].MaxTimeToClose;
            //TypeEnd TypeEnd;
            //DateTime? NTimeEnd;
            //if (EndTime < s_dalConfig.Clock)
            //{//אם זה קטן מזמן מערכת אז עבר הזמן ולא טופל
            //    TypeEnd = TypeEnd.ExpiredCancel;
            //  NTimeEnd = null;
            //}

            //else if (i % 4 == 0)
            //{
            //    TypeEnd = TypeEnd.SelfCancel;
            //    NTimeEnd = null;
            //}
            //else if (i % 5 == 0)
            //{
            //    TypeEnd = TypeEnd.ManagerCancel;
            //    NTimeEnd = null;
            //}
            //else if (i % 3 == 0)
            //{ 

            //TypeEnd = TypeEnd.Treated;
            //    {

            //        TimeSpan TimeRange = EndTime - TimeStart;
            //        NTimeEnd = TimeStart.AddMinutes(Random.Next(0,(int)TimeRange.TotalMinutes));


            var Vol = s_dalVolunteer.ReadAll();
            Random RandomV = new Random();
            int RandomNumberV;
            do
                RandomNumberV = RandomV.Next(0, 17);

        while (RandomNumberV == 14 || RandomNumberV == 5);  /// מתנדבים שלו טיפלו בכלל
        int VolId = Vol[RandomNumberV].Id;


        }
    }
    private static void createAssignment()
    {
        for (int i = 0; i < 60; i++)
        {

            int randVolunteer = s_rand.Next(s_dalVolunteer!.ReadAll().Count);
            Volunteer volunteerToAssig = s_dalVolunteer.ReadAll()[randVolunteer];
            int randCAll = s_rand.Next(s_dalCall!.ReadAll().Count - 15);
            Call callToAssig = s_dalCall.ReadAll()[randCAll];
            while (callToAssig.TimeOpened > s_dalConfig!.Clock)
            {
                randCAll = s_rand.Next(s_dalCall!.ReadAll().Count - 15);
                callToAssig = s_dalCall.ReadAll()[randCAll];
            }
            TypeEnd? finish = null;
            DateTime? finishTime = null;
            if (callToAssig.MaxTimeToClose != null && callToAssig.MaxTimeToClose >= s_dalConfig?.Clock)
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
                        finishTime = s_dalConfig!.Clock;
                        break;
                    case 1: finish = TypeEnd.SelfCancel; break;
                    case 2: finish = TypeEnd.ManagerCancel; break;




                }
            }
            s_dalAssignment?.Create(new Assignment(0, callToAssig.Id, volunteerToAssig.Id, s_dalConfig!.Clock, finishTime, finish));
        }
    }
    //method that do all the initializations
    public static void Do(IVolunteer? dal_volunteer, ICall? dal_call, IAssignment? dal_assignment, IConfig? dal_Config)
    {
        s_dalVolunteer = dal_volunteer ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalCall = dal_call ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalAssignment = dal_assignment ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalConfig = dal_Config ?? throw new NullReferenceException("DAL object can not be null!");

        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset(); //stage 1
        s_dalVolunteer.DeleteAll(); //stage 1
        s_dalCall.DeleteAll(); //stage 1
        s_dalAssignment.DeleteAll(); //stage 1

        Console.WriteLine("Initializing Volunteers list");
        CreateVolunteers();
        Console.WriteLine("Initializing Calls list");
        CreateCalls();
        Console.WriteLine("Initializing Assignments list");
        CreateAssignment();
    }
}




        

            }
        }
        s_dalAssignment?.Create(new Assignment(0, callToAssig.Id, volunteerToAssig.Id, s_dalConfig!.Clock, finishTime, finish));
    }
}
//method that do all the initializations
public static void Do(IVolunteer? dal_volunteer, ICall? dal_call, IAssignment? dal_assignment, IConfig? dal_Config)
{
    s_dalVolunteer = dal_volunteer ?? throw new NullReferenceException("DAL object can not be null!");
    s_dalCall = dal_call ?? throw new NullReferenceException("DAL object can not be null!");
    s_dalAssignment = dal_assignment ?? throw new NullReferenceException("DAL object can not be null!");
    s_dalConfig = dal_Config ?? throw new NullReferenceException("DAL object can not be null!");

    Console.WriteLine("Reset Configuration values and List values...");
    s_dalConfig.Reset(); //stage 1
    s_dalVolunteer.DeleteAll(); //stage 1
    s_dalCall.DeleteAll(); //stage 1
    s_dalAssignment.DeleteAll(); //stage 1







