
namespace DalTest;
using DalApi;
using DO;
using global::DO;
using System;
using System.Data.Common;

public static class Initialization
{
    private static IVolunteer? s_volunteer; //stage 1
    private static ICall? s_call; //stage 1
    private static IAssignment? s_assignment; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();

    private static void CreateVolunteers()
    {
        string[] VolunteerNames = { "Ruth Cohen", "Yossi Levy", "Oren Alon", "Meirav Israeli", "Dan Mizrahi", "Ayelet Israeli", "Dana Cohen",
                                "Noam Brenner", "Ofer Mizrahi", "Ron Halevi", "Liron Abutbul", "Omer Katz", "Ronit Goldman", "Ilan Shemesh", "Galit Cohen" };
        string[] PhoneNumbers = { "050-1234567", "052-9876543", "053-4567890", "054-1237894", "055-6789123", "050-3456789", "052-2345678",
                              "053-9876543", "054-5678912", "055-1234569", "050-4561237", "052-7893451", "053-3214567", "054-7896543", "055-6547891" };
        string[] Emails = { "ruth@example.com", "yossi@example.com", "oren@example.com", "meirav@example.com", "dan@example.com", "ayelet@example.com", "dana@example.com",
                        "noam@example.com", "ofer@example.com", "ron@example.com", "liron@example.com", "omer@example.com", "ronit@example.com", "ilan@example.com", "galit@example.com" };

        // יצירת מתנדבים רגילים
        for (int i = 0; i < VolunteerNames.Length; i++)
        {

            int Id;
            do
                Id = s_rand.Next(700000000, 1000000000); // ת.ז אקראית עם 9 ספרות
            while (s_volunteer!.Read(Id) != null); // בדיקת ייחודיות של ת.ז.

            string Name = VolunteerNames[i];
            string Phone = PhoneNumbers[i];
            string Email = Emails[i];
            Distance DistanceType = Distance.Aerial; // מרחק ברירת מחדל
            Role Nrole = Role.Volunteer; // ברירת מחדל - מתנדב רגיל
            bool Active = true; // המתנדב פעיל כברירת מחדל
            double MaxReading = s_rand.Next(5, 100); // מרחק מקסימלי אקראי בין 5 ל-100

            s_volunteer!.Create(new Volunteer(Id, Name, Phone, Email, DistanceType, Nrole, Active, null, null, null, null, MaxReading));
        }

        // הוספת מנהל אחד לפחות
        int managerId;
        do
            managerId = s_rand.Next(100000000, 1000000000);
        while (s_volunteer!.Read(managerId) != null);

        s_volunteer!.Create(new Volunteer(managerId, "Admin Man", "050-1111111", "admin@example.com", Distance.Aerial, Role.Boss, true, "password123"));
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



        string[] Addresses = {
            "Main St 1", "King George St 22", "Dizengoff St 35", "Allenby St 10", "Herzl Blvd 7", "Ben Gurion Ave 18",
            "Jabotinsky St 14", "HaYarkon St 99", "Rothschild Blvd 12", "Nordau St 15", "Begin Rd 23", "Shenkin St 27",
            "Ibn Gvirol St 30", "Yehuda Halevi St 45", "Jerusalem Blvd 21"
        };

        // יצירת קריאות
        for (int i = 0; i < 50; i++)
        {



            CallType Ctype;  // הכרזה על המשתנה פעם אחת
            string ndescription;
            int P = 0, L = 0, C = 0;
            if (i % 2 == 0)
            {
                Ctype = CallType.Puncture;
                ndescription = DescriptionsP[P];
                P++;
            }
            else if (i % 3 == 0)
            {
                Ctype = CallType.LockedCar;
                ndescription = DescriptionsP[L];
                L++;
            }
            else
            {
                Ctype = CallType.Cables;  // ערך אחר אם לא מתקיים אף תנאי
                ndescription = DescriptionsP[C];
                C++;
            }


           
            //DateTime? maxTimeToClose =
            DateTime Start = s_dalConfig.Clock.AddDays(-1); // זמן התחלה יהיה לפני 24 שעות מהשעון הנוכחי
            int Range = (s_dalConfig.Clock - Start).Minutes; // חישוב מספר הדקות מאז זמן ההתחלה
            DateTime RndomStart = Start.AddMinutes(s_rand.Next(Range)); // הגרלת זמן פתיחה רנדומלי בתוך ה-24 שעות האחרונות
            DateTime? RandomEnd = null;
            if (i % 10 == 0)//5 that endtime- pag tokef

                RandomEnd = RndomStart.AddMinutes(new Random().Next(1, (int)(s_dalConfig.Clock - RndomStart).TotalMinutes));
            // כעת, ניצור זמן סיום מקסימלי בהתאם לזמן הפתיחה:
            else {
                // ערך ברירת מחדל, אין זמן סיום
                if (s_rand.Next(2) == 1) // החלטה רנדומלית אם לכלול זמן סיום או לא
                {
                    int maxDurationMinutes = s_rand.Next(1, 1441); // הגדרת טווח זמן סיום מקסימלי (עד 1440 דקות = 24 שעות)
                    RandomEnd = RndomStart.AddMinutes(maxDurationMinutes); // זמן סיום הוא זמן פתיחה + מספר דקות רנדומלי
                }
            }

            // זמן סיום לפני זמן ההתחלה



            



          //  s_call.Add(new Call( description, address, latitude, longitude, timeOpened, maxTimeToClose));
        }
    }
        

   

       



