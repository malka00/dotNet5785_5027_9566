namespace Dal;
internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_assignment_xml = "assignment.xml";
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }
    internal static int NextassignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextassignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextassignmentId", value);
    }

    internal static TimeSpan RiskRange
    {
        //get => XMLTools.GetConfigDateVal(s_data_config_xml, "CRiskRangelock") ?? TimeSpan.FromHours(1);
        //set => XMLTools.SetConfigDateVal(s_data_config_xml, "RiskRange", value);
    }


    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    internal static void Reset()
    {
        NextCallId = 1000;
        NextassignmentId = 1000;
          
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
     
    }
}
