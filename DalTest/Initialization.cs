
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



    }



    private static void CreateCalls()
    {
        string[] descriptions = {
            "Fire in a residential area", "Medical emergency at school", "Robbery in a store", "Traffic accident on highway",
            "Animal rescue", "Suspicious package found", "Flooding in basement", "Fall from height", "Lost child in park", "Minor fire in kitchen",
            "Power outage", "Domestic incident", "Injury in workplace", "Public disturbance", "Gas leak reported"
        };

        string[] addresses = {
            "Main St 1", "King George St 22", "Dizengoff St 35", "Allenby St 10", "Herzl Blvd 7", "Ben Gurion Ave 18",
            "Jabotinsky St 14", "HaYarkon St 99", "Rothschild Blvd 12", "Nordau St 15", "Begin Rd 23", "Shenkin St 27",
            "Ibn Gvirol St 30", "Yehuda Halevi St 45", "Jerusalem Blvd 21"
        };

        // יצירת קריאות
        for (int i = 0; i < 50; i++)
        {
            int id;
            do
                id = Config.NextCallId;
            if (id % 2 == 0)
                CallType type = FoodPreparation;
            else
                CallType type = FoodPreparation; 
            string description = descriptions[i % descriptions.Length];
            string address = addresses[i % addresses.Length];
            double latitude = ;
            double longitude = ;

           //DateTime stimeOpened = new DateTime(s_dalConfig.Clock.Year, s_dalConfig.Clock.Month, s_dalConfig.Clock.Day-7);
            DateTime timeOpened = s_dalConfig.Clock.AddHours(-24);

            DateTime? maxTimeToClose = (s_rand.Next(0, 3) == 0) ? null : GenerateRandomCloseDate(timeOpened);

            // קריאות לא מוקצות
            if (i < 10)
            {

                //    description = "Unassigned Call";
                //CallType type = CallType.Unassigned;
            }

            // קריאות שפג תוקפן
            if (i >= 10 && i < 15)
            {
                maxTimeToClose = timeOpened.AddHours(-1); // זמן סיום שפג בעבר
            }

            s_call.Add(new Call(id,, description, address, latitude, longitude, timeOpened, maxTimeToClose));
        }


    }

       



