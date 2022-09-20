using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeToClient.Locations;

public record LocationDetails
{
    [JsonPropertyName("address")]
    public AddressModel Address { get; set; }

    [JsonPropertyName("contact")]
    public ContactModel Contact { get; set; }

    [JsonPropertyName("description")]
    public DescriptionModel Description { get; set; }

    [JsonPropertyName("facebookName")]
    public string FacebookName { get; set; }

    [JsonPropertyName("googleName")]
    public string GoogleName { get; set; }

    [JsonPropertyName("isAlwaysOpen")]
    public bool IsAlwaysOpen { get; set; }

    [JsonPropertyName("locationDescriptor")]
    public string LocationDescriptor { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("network")]
    public NetworkModel Network { get; set; }

    [JsonPropertyName("openHours")]
    public OpenHoursModel OpenHours { get; set; }

    [JsonPropertyName("pendingChanges")]
    public PendingChangesModel PendingChanges { get; set; }

    [JsonPropertyName("permanentlyClosed")]
    public bool PermanentlyClosed { get; set; }

    [JsonPropertyName("location")]
    public PositionModel Position { get; set; }

    [JsonPropertyName("specialOpenHours")]
    public List<SpecialOpenHour> SpecialOpenHours { get; set; }

    [JsonPropertyName("storeId")]
    public string StoreId { get; set; }

    [JsonPropertyName("temporarilyClosedUntil")]
    public string TemporarilyClosedUntil { get; set; }

    [JsonPropertyName("wifiSsid")]
    public string WifiSsid { get; set; }

    public record Day
    {
        [JsonPropertyName("span")]
        public List<Span> Span { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }
    }

    public record Span
    {
        [JsonPropertyName("close")]
        public string Close { get; set; }

        [JsonPropertyName("open")]
        public string Open { get; set; }
    }

    public record SpecialOpenHour
    {
        [JsonPropertyName("closeTime")]
        public string CloseTime { get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }

        [JsonPropertyName("isClosed")]
        public bool IsClosed { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("openTime")]
        public string OpenTime { get; set; }

        [JsonPropertyName("start")]
        public string Start { get; set; }
    }

    public record AddressModel
    {
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("street")]
        public string Street { get; set; }

        [JsonPropertyName("zip")]
        public string Zip { get; set; }
    }

    public record ContactModel
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("homepage")]
        public string Homepage { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }
    }

    public record DescriptionModel
    {
        [JsonPropertyName("long")]
        public string Long { get; set; }

        [JsonPropertyName("short")]
        public string Short { get; set; }
    }

    public record Facebook
    {
        [JsonPropertyName("coverImage")]
        public string CoverImage { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("pageId")]
        public string PageId { get; set; }

        [JsonPropertyName("profileImage")]
        public string ProfileImage { get; set; }
    }

    public record Google
    {
        [JsonPropertyName("coverImage")]
        public string CoverImage { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("newReviewUrl")]
        public string NewReviewUrl { get; set; }

        [JsonPropertyName("placeId")]
        public string PlaceId { get; set; }

        [JsonPropertyName("profileImage")]
        public string ProfileImage { get; set; }
    }

    public record PositionModel
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }

    public record NetworkModel
    {
        [JsonPropertyName("facebook")]
        public Facebook Facebook { get; set; }

        [JsonPropertyName("google")]
        public Google Google { get; set; }
    }

    public record OpenHoursModel
    {
        [JsonPropertyName("fri")]
        public Day Fri { get; set; }

        [JsonPropertyName("mon")]
        public Day Mon { get; set; }

        [JsonPropertyName("sat")]
        public Day Sat { get; set; }

        [JsonPropertyName("sun")]
        public Day Sun { get; set; }

        [JsonPropertyName("thu")]
        public Day Thu { get; set; }

        [JsonPropertyName("tue")]
        public Day Tue { get; set; }

        [JsonPropertyName("wed")]
        public Day Wed { get; set; }
    }

    public record PendingChangesModel
    {
        [JsonPropertyName("address")]
        public AddressModel Address { get; set; }

        [JsonPropertyName("location")]
        public PositionModel Position { get; set; }
    }
}
