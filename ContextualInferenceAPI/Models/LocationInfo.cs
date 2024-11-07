namespace ContextualInferenceAPI.Models;

public class LocationInfo
{
    public required string CountryName { get; set; }
    public required string CountryCode { get; set; }
    public required string City { get; set; }
    public required string Location { get; set; }
    public required string Property { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
