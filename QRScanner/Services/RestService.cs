using System.Diagnostics;
using System.Text;
using System.Text.Json;
using QRShared;

namespace QRScanner.Services;

public class RestService
{
    HttpClient _client;
    JsonSerializerOptions _serializerOptions;

    public RestService()
    {
        _client = new HttpClient();
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
    
    public async Task<RoomInformation> GetRoomInformationAsync(Uri uri)
    {
        RoomInformation item = NullInformation.Information;
        try
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                item = JsonSerializer.Deserialize<RoomInformation>(content, _serializerOptions) ?? item;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return item;
    }
    
    public async Task AddRoomInformation(RoomInformation roomInformation,ConnectionSetting setting)
    {
        try
        {
            var json = JsonSerializer.Serialize<RoomInformation>(roomInformation, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _client.PostAsync(setting.Url, content);
            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tTodoItem successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }
}