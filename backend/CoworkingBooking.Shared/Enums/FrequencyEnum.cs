using System.Text.Json.Serialization;

namespace CoworkingBooking.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<Frequency>))]
    public enum Frequency
    {
        DAILY = 1,
        WEEKLY = 2,
    }
}