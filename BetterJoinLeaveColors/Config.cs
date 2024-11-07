using System.Text.Json.Serialization;

namespace BetterJoinLeaveColors;

public class Config {
    [JsonInclude] public string Join = "038a07";
    [JsonInclude] public string Leave = "8a0310";
}
