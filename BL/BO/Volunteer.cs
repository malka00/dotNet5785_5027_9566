

namespace BO;

using Helpers;
public class Volunteer
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public Distance TypeDistance { get; set; }
    public Role Job { get; set; }
    public bool Active { get; set; }
    public string? Password { get; set; }
    public string? FullAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? MaxReading { get; set; }
    public int SumCalls { get; set; }
    public int SumCanceled { get; set; }
    public int SumExpired { get; set; }
    public BO.CallInProgress CallIn { get; set; }
    public override string ToString() => this.ToStringProperty();

}

