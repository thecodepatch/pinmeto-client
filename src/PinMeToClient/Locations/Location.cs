using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeToClient.Locations;

public record Location
{
    [JsonPropertyName("specialOpenHours")]
    public List<SpecialOpenHourModel> SpecialOpenHours { get; } = new();

    [JsonPropertyName("address")]
    public AddressModel Address { get; init; } = null!;

    [JsonPropertyName("contact")]
    public ContactModel Contact { get; init; } = null!;

    [JsonPropertyName("description")]
    public DescriptionModel Description { get; init; } = null!;

    [JsonPropertyName("locationDescriptor")]
    public string LocationDescriptor { get; init; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    [JsonPropertyName("network")]
    public NetworkModel Network { get; init; } = null!;

    [JsonPropertyName("openHours")]
    public OpenHoursModel OpenHours { get; init; } = null!;

    [JsonPropertyName("openingDate")]
    public string OpeningDate { get; init; } = null!;

    [JsonPropertyName("location")]
    public PositionModel Position { get; init; } = null!;

    [JsonPropertyName("storeId")]
    public string StoreId { get; init; } = null!;

    public record AddressModel
    {
        [JsonPropertyName("city")]
        public string City { get; init; } = null!;

        [JsonPropertyName("country")]
        public string Country { get; init; } = null!;

        [JsonPropertyName("street")]
        public string Street { get; init; } = null!;

        [JsonPropertyName("zip")]
        public string Zip { get; init; } = null!;
    }

    public record ContactModel
    {
        [JsonPropertyName("email")]
        public string Email { get; init; } = null!;

        [JsonPropertyName("homepage")]
        public string Homepage { get; init; } = null!;

        [JsonPropertyName("phone")]
        public string Phone { get; init; } = null!;
    }

    public record DescriptionModel
    {
        [JsonPropertyName("long")]
        public string Long { get; init; } = null!;

        [JsonPropertyName("short")]
        public string Short { get; init; } = null!;
    }

    public record Facebook
    {
        [JsonPropertyName("coverImage")]
        public string CoverImage { get; init; } = null!;

        [JsonPropertyName("link")]
        public string Link { get; init; } = null!;

        [JsonPropertyName("pageId")]
        public string PageId { get; init; } = null!;

        [JsonPropertyName("profileImage")]
        public string ProfileImage { get; init; } = null!;
    }

    public record Google
    {
        [JsonPropertyName("coverImage")]
        public string CoverImage { get; init; } = null!;

        [JsonPropertyName("link")]
        public string Link { get; init; } = null!;

        [JsonPropertyName("newReviewUrl")]
        public string NewReviewUrl { get; init; } = null!;

        [JsonPropertyName("placeId")]
        public string PlaceId { get; init; } = null!;

        [JsonPropertyName("profileImage")]
        public string ProfileImage { get; init; } = null!;
    }

    public record PositionModel
    {
        [JsonPropertyName("lat")]
        public double Lat { get; init; }

        [JsonPropertyName("lon")]
        public double Lon { get; init; }
    }

    public record NetworkModel
    {
        [JsonPropertyName("facebook")]
        public Facebook Facebook { get; init; } = null!;

        [JsonPropertyName("google")]
        public Google Google { get; init; } = null!;

        [JsonPropertyName("pendingChanges")]
        public PendingChanges PendingChanges { get; init; } = null!;

        [JsonPropertyName("wifiSsid")]
        public string WifiSsid { get; init; } = null!;
    }

    public record OpenHoursModel
    {
        [JsonPropertyName("fri")]
        public Day Fri { get; init; } = null!;

        [JsonPropertyName("mon")]
        public Day Mon { get; init; } = null!;

        [JsonPropertyName("sat")]
        public Day Sat { get; init; } = null!;

        [JsonPropertyName("sun")]
        public Day Sun { get; init; } = null!;

        [JsonPropertyName("thu")]
        public Day Thu { get; init; } = null!;

        [JsonPropertyName("tue")]
        public Day Tue { get; init; } = null!;

        [JsonPropertyName("wed")]
        public Day Wed { get; init; } = null!;
    }

    public record PendingChanges
    {
        [JsonPropertyName("address")]
        public AddressModel Address { get; init; } = null!;

        [JsonPropertyName("location")]
        public PositionModel Coordinate { get; init; } = null!;
    }

    public record Span
    {
        [JsonPropertyName("close")]
        public string Close { get; init; } = null!;

        [JsonPropertyName("open")]
        public string Open { get; init; } = null!;
    }

    public record SpecialOpenHourModel
    {
        [JsonPropertyName("closeTime")]
        public string CloseTime { get; init; } = null!;

        [JsonPropertyName("end")]
        public string End { get; init; } = null!;

        [JsonPropertyName("isClosed")]
        public bool IsClosed { get; init; }

        [JsonPropertyName("label")]
        public string Label { get; init; } = null!;

        [JsonPropertyName("openTime")]
        public string OpenTime { get; init; } = null!;

        [JsonPropertyName("start")]
        public string Start { get; init; } = null!;
    }

    public record Day
    {
        [JsonPropertyName("span")]
        public List<Span> Times { get; } = null!;

        [JsonPropertyName("state")]
        public string State { get; init; } = null!;
    }
}
