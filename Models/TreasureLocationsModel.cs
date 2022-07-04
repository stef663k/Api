namespace LoginTreasureApi.Models;

public class TreasureLocationsModel
{
    public int Id { get; set; }
    public Decimal Longitude { get; set; }
    public Decimal Latitude { get; set; }
    public string Description { get; set; }
}
