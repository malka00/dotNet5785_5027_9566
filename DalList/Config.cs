namespace Dal;

internal static class Config
{
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId++; }

    internal const int startAssignmenteID=1000;
    private static int nextAssignmenteID = startAssignmenteID;
    internal static int NextAssignmenteID { get => nextAssignmenteID++; }
    //...

    internal static DateTime Clock { get; set; } = DateTime.Now;

    // "זמן סיכון" עבור קריאות מתקרבות לזמן סיום
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);

    // פונקציה לאיפוס הערכים להתחלתיים
    internal static void Reset()
    {
        nextCallId = startCallId;
        // nextVolunteerId = startVolunteerId;
        nextAssignmenteID = startAssignmenteID;

        // משתני תצורה נוספים לאיפוס
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }
}
