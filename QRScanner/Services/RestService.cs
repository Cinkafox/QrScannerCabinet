using System.Net;
using System.Text;
using System.Text.Json;
using QRScanner.Utils;
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
    
    public async Task<RestResult<T>> GetAsync<T>(Uri uri,CancellationToken cancellationToken, Guid? token = null)
    {
        if (token is not null)
            uri = uri.AddParameter(nameof(token), token.ToString());
        
        try
        {
            var response = await _client.GetAsync(uri,cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _debug.Debug($"SUCCESSFUL GET CONTENT {typeof(T)} URL {uri} {content}");
                return new RestResult<T>(JsonSerializer.Deserialize<T>(content, _serializerOptions),null,response.StatusCode);
            }

            _debug.Error("ERROR " + response.StatusCode + " " + uri);
            return new RestResult<T>(default, "response code:" + response.StatusCode,response.StatusCode);
        }
        catch (Exception ex)
        {
           _debug.Error("ERROR WHILE CONNECTION " + uri + ": "  + ex.Message );
           return new RestResult<T>(default, ex.Message,HttpStatusCode.RequestTimeout);
        }
    }

    public async Task<T> GetAsyncDefault<T>(Uri uri, T defaultValue,CancellationToken cancellationToken)
    {
        var result = await GetAsync<T>(uri,cancellationToken);
        return result.Value ?? defaultValue;
    }
    
    public async Task<bool> AddAsync<T>(T information,Uri uri,CancellationToken cancellationToken, Guid? token = null) where T: BaseInformation
    {
        if (token is not null)
            uri = uri.AddParameter(nameof(token), token.ToString());
        
        try
        {
            var json = JsonSerializer.Serialize(information, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _client.PostAsync(uri, content,cancellationToken);
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

public class RestResult<T>
{
    public T? Value;
    public string? Message;
    public HttpStatusCode StatusCode;
    public bool IsAuthRequire;

    public RestResult(T? value, string? message, HttpStatusCode statusCode)
    {
        Value = value;
        Message = message;
        StatusCode = statusCode;
        IsAuthRequire = statusCode == HttpStatusCode.Unauthorized;
    }

    public static implicit operator T?(RestResult<T> result) => result.Value;
}