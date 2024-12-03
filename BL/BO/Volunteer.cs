
using DO;

namespace BO;

//using Helpers;
public class Volunteer
{
    int Id { get; init; }
    string FullName { get; set; }
    string PhoneNumber { get; set; }
    string Email { get; set; }
    Distance TypeDistance { get; set; }
    Role Job { get; set; }
    bool Active { get; set; }
    string? Password { get; set; }
    string? FullAddress { get; set; }
    double? Latitude { get; set; }
    double? Longitude { get; set; }
    double? MaxReading { get; set; }
    int SunCalls { get; set; }
    int Sumcanceled {  get; set; }
    int SunExpired { get; set; }
    BO.CallInProgress callIn { get; set; }
    //public override string ToString() => this.ToStringProperty();
}
