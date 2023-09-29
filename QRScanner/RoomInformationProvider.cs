using System.Net.Http.Json;
using QRShared;

namespace QRScanner;

public class RoomInformationProvider
{
    public static async Task<RoomInformation> GetRoomInformation(long id)
    {
        HttpClient client = new()
        {
            BaseAddress = new Uri(Config.QRServerAdress),
        };
        var response = await client.GetAsync($"RoomInformation/{id}");
        return Deserialize(await response.Content.ReadAsStringAsync());
    }

    public static async Task AddRoomInformation(RoomInformation roomInformation)
    {
        HttpClient client = new()
        {
            BaseAddress = new Uri(Config.QRServerAdress),
        };
        await client.PostAsync("CreateRoomInformation",JsonContent.Create(roomInformation));
    }
    public static RoomInformation Deserialize(string input)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<RoomInformation>(input)!;
    }
}