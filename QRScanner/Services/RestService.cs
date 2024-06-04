using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using QRShared;

namespace QRScanner.Services;

public class RestService
{
    private readonly DebugService _debug;
    HttpClient _client = new();
    JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public RestService(DebugService debug)
    {
        _debug = debug;
    }
    
    public async Task<T?> GetAsync<T>(Uri uri)
    {
        try
        {
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _debug.Debug($"SUCCESSFUL GET CONTENT {nameof(T)} URL {uri}");
                return JsonSerializer.Deserialize<T>(content, _serializerOptions);
            }

            _debug.Error("ERROR " + response.StatusCode );
        }
        catch (Exception ex)
        {
           _debug.Error("ERROR WHILE CONNECTION " + ex.Message);
        }

        return default;
    }

    public async Task<T> GetAsyncDefault<T>(Uri uri, T defaultValue)
    {
        return await GetAsync<T>(uri) ?? defaultValue;
    }
    
    public async Task<bool> AddAsync<T>(T information,Uri url, Guid? token = null) where T: BaseInformation
    {
        if (token is not null)
        {
            var builder = new UriBuilder(url);
            builder.Query = "?token=" + token;
            url = builder.Uri;
        }
        
        try
        {
            var json = JsonSerializer.Serialize(information, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
                return true;
        }
        catch (Exception ex)
        {
            _debug.Debug("ERROR " + ex.Message);
        }

        return false;
    }
}