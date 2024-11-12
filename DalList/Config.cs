

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
    //...

    internal static void Reset()
    {
        nextCallId = startCallId;
        //...
        Clock = DateTime.Now;
        //...
    }

}
