namespace Dal;

internal static class Config
{
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId++; }

    internal const int startAssignmentID=1000;
    private static int nextAssignmentID = startAssignmentID;
    internal static int NextAssignmentID { get => nextAssignmentID++; }
 
    internal static DateTime Clock { get; set; } = DateTime.Now;
 

    /// <summary>
    /// "Risk time" for calls approaching termination time
    /// </summary>
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// A function to reset the initial values
    /// </summary>
    internal static void Reset()
    {
        nextCallId = startCallId;
        // nextVolunteerId = startVolunteerId;
        nextAssignmentID = startAssignmentID;

        // Additional configuration variables to reset
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }
}
