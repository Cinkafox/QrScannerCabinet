using System.Diagnostics;
using System.Text;
using System.Text.Json;
using QRShared;

namespace QRScanner.Services;

public class RestService
{
    HttpClient _client = new();
    JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    
    public async Task<List<T>> GetInformationAsync<T>(Uri uri) where T: BaseInformation
    {
        var items = new List<T>();
        try
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                items = JsonSerializer.Deserialize<List<T>>(content, _serializerOptions) ?? items;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return items;
    }
    
    public async Task AddInformation<T>(BaseInformation information,ConnectionSetting setting) where T: BaseInformation
    {
        try
        {
            var json = JsonSerializer.Serialize<T>((T)information, _serializerOptions);
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